using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.DPT_BE.UI.Model
{
    public class PlotModel
    {
        public List<string> PlotIds { get; set; }

        public PlotProfiling PlotProfiling { get; set; }

    }

    public class PlotProfiling
    {
        List<PlotTypeCounter> plotTypeCounters { get; set; }
        List<NewExperimentType> newExperimentTypes { get; set; }
    }


    public class PlotTypeCounter
    {
        public PlotType PlotType { get; set; }
        public int Number{ get; set; }

    }
  
    public enum PlotType
    {
        GP,
        Ep,
        Mip,
        Vip
    }

    public enum NewExperimentType
    {
        REX1,
        REX2,
        LUX,
        FOX,
        None
    }

}