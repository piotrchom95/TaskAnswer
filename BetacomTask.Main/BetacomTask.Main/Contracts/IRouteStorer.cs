﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetacomTask.Main.Contracts
{
    public interface IRouteStorer
    {
        List<Route> Routes { get; set; }
    }
}
