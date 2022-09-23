using BetacomTask.Main.Contracts;
using BetacomTask.Main.ExtensionMethods;


namespace BetacomTask.Main.Infrastructure
{
    public class GpsIdFinder : IGpsIdFinder
    {
        List<Route> Routes { get; set; }

        List<GpsTrace> GpsTraces { get; set; }
    
        public GpsIdFinder(IRouteStorer routeStorer, IGpsTraceStorer gpsTraceStorer)
        {
            Routes = routeStorer.Routes;
            GpsTraces = gpsTraceStorer.Traces;        
        }


        /// <summary>
        /// Method tries to match carId of vehicle with gpsId of gpsDevice mount in vehicle
        /// </summary>
        /// <param name="carId">examined car Id</param>
        /// <param name="recordsToExamineCount">how many record we want to analize (less better performance lower accuracy) </param>
        /// <param name="accuracy">threshhold for result to be found as matched (0 <-> 1) </param>
        /// <param name="stopTime">time of vehicle stoping after arrival (minutes)</param>
        /// <param name="gpsOffset">accuracy of gps </param>
        /// <returns>null if gpsId was not found or data are corrupted or gpsId of matching device </returns>
        public int? FindGpsIdByCarId(int carId, int recordsToExamineCount, float accuracy, float stopTime, double gpsOffset) // oprócz inta jako tuptle (int?,string) można zwrócić również powód nie znalezienia wyniku
        {
            if (AreInputsInvalid(carId, recordsToExamineCount, accuracy, stopTime, gpsOffset)) return null; // można zrobić jako osobny validator 

            DateTime startGpsTracesTimestamp = TakeStartTimestampOfGpsTraces();  // gps traces są tylko z jednego dnia warto wiedzieć kiedy się zaczynają

            var routesAfterStartTime = GetSpecifiedCountOfRoutesAfterStartTimestamp(carId, recordsToExamineCount, startGpsTracesTimestamp);

            if (AreRoutesNullsOrEmpty(routesAfterStartTime)) return null;

            Dictionary<int, int> tracesMatchingRouteByGpsIdKey = new Dictionary<int, int>();

            Dictionary<int, int> allTracesFromRangeByGpsIdKey = new Dictionary<int, int>();

            foreach (var routeAfterStart in routesAfterStartTime)
            {
                var allTracesInTimeRangeAfterArrivalTime = GpsTraces.Where(trace => TraceIsInTimeRange(stopTime, trace, routeAfterStart));

                if (!AreTracesNullsOrEmpty(allTracesInTimeRangeAfterArrivalTime))
                {
                    var groupByIdTracesInTimeRangeAfterArrivalTime = GroupRecordsByGpsId(allTracesInTimeRangeAfterArrivalTime);

                    foreach (var group in groupByIdTracesInTimeRangeAfterArrivalTime)
                    {
                        int findedTracesCount = GetCountOfRecordsInDistanceRange(gpsOffset, routeAfterStart, group);

                        tracesMatchingRouteByGpsIdKey.AddRecordToDictionaryOrUpdateExistingOne<int>(group.Key, findedTracesCount);

                        allTracesFromRangeByGpsIdKey.AddRecordToDictionaryOrUpdateExistingOne<int>(group.Key, group.Count());
                   
                    }
                }
            }

            Dictionary<int, float> resultOfMatchingByGpsId = new Dictionary<int, float>();

            foreach (var trace in tracesMatchingRouteByGpsIdKey)
            {
                if (allTracesFromRangeByGpsIdKey.ContainsKey(trace.Key))
                {
                    float traceResult = (float)trace.Value / (float)allTracesFromRangeByGpsIdKey[trace.Key];

                    resultOfMatchingByGpsId.Add(trace.Key, traceResult);
                }
            }

            float maxValue = resultOfMatchingByGpsId.GetMaxValueFromDictionary();

            int? keyOfMaxValue = resultOfMatchingByGpsId.GetUniqueKeyOfMaxValueFromDictionary(); // jak dwa rekordy o tej samej wartości to null bo nie możemy wyłonić gpsId

            if (keyOfMaxValue == null) return null;

            if (IsResultNoAcurateEnough(accuracy, maxValue)) return null;

            return keyOfMaxValue;
        }

       
        #region Validation 

        private bool AreInputsInvalid(int carId, int recordsToExamineCount, float accuracy, float stopTime, object gpsOffsetW)
        {         
            // dodatkowa validacja pól w zależności od założonych przedziałów.

            // can use generic method for validation of lists
            if (AreTracesNullsOrEmpty(GpsTraces)) return true;
            if (AreRoutesNullsOrEmpty(Routes)) return true;

            return false;
        }

        private bool AreRoutesNullsOrEmpty(IEnumerable<Route> routes)
        {
            return routes == null || (routes != null && !routes.Any());
        }

        private bool AreTracesNullsOrEmpty(IEnumerable<GpsTrace> gpsTraces)
        {
            return gpsTraces == null || (gpsTraces != null && !gpsTraces.Any());
        }

        private static bool IsResultNoAcurateEnough(float accuracy, float valueToExamine)
        {
            return valueToExamine < accuracy;
        }

        #endregion

        #region LinqQueries

        private static IEnumerable<IGrouping<int, GpsTrace>> GroupRecordsByGpsId(IEnumerable<GpsTrace> allTracesInTimeRangeForVehicleArrivalTime)
        {
            return allTracesInTimeRangeForVehicleArrivalTime.GroupBy(trace => trace.GpsDeviceId);
        }
     
        private IEnumerable<Route> GetSpecifiedCountOfRoutesAfterStartTimestamp(int carId, int recordsToExamineCount, DateTime startTableDate)
        {
            return Routes.Take(recordsToExamineCount).Where(route => route.WaypointArrivalTimestamp > startTableDate && route.CarId == carId);
        }

        private DateTime TakeStartTimestampOfGpsTraces()
        {
            return GpsTraces.Min(x => x.Timestamp);
        }

        private static bool TraceIsInTimeRange(float stopTime, GpsTrace trace, Route routeAfterStart)
        {
            return trace.Timestamp > routeAfterStart.WaypointArrivalTimestamp && trace.Timestamp < routeAfterStart.WaypointArrivalTimestamp.AddMinutes(stopTime);
        }

        private static int GetCountOfRecordsInDistanceRange(double gpsOffset, Route routeAfterStart, IGrouping<int, GpsTrace> group)
        {
            return group.Where(trace =>
                           CoordinateDataCalculator.CalculateDistanceByTwoCoordinates
                           (trace.Longtitude, trace.Latitude, routeAfterStart.Longtitude, routeAfterStart.Latitude) < gpsOffset).Count();
        }

        #endregion

    }
}
