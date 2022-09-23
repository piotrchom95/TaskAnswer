using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetacomTask.Main.Infrastructure
{
    public static class CoordinateDataCalculator
    {
        private const int EarthRadiusInMeters = 6371000;

		public static double CalculateDistanceByTwoCoordinates(double long1 , double lat1 , double long2 , double lat2)
        {
			double phi1 = lat1 * UnitConverter.DEG_TO_RAD;

			double phi2 = lat2 * UnitConverter.DEG_TO_RAD;

			double deltaPhi = (lat2 - lat1) * UnitConverter.DEG_TO_RAD;
			double deltaLambda = (long2 - long1) * UnitConverter.DEG_TO_RAD;

			double a = Math.Sin(deltaPhi / 2) * Math.Sin(deltaPhi / 2)
					+ Math.Cos(phi1) * Math.Cos(phi2) * Math.Sin(deltaLambda / 2)
							* Math.Sin(deltaLambda / 2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			double distance = EarthRadiusInMeters * c;

			return (float)distance;


		}
    }
}
