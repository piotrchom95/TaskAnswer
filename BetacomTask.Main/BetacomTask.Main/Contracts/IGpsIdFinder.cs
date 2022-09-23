using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetacomTask.Main.Contracts
{
    interface IGpsIdFinder
    {
        public int? FindGpsIdByCarId(int carId,int recordsToExamineCount, float accuracy, float stopTime, double gpsOffset);
    }
}
