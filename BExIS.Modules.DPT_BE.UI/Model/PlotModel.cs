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

        public List<string> NotVaildPlotIds { get; set; }

        public PlotModel()
        {
            PlotIds = new List<string>();
            PlotProfiling = new PlotProfiling();
            NotVaildPlotIds = new List<string>();
        }

    }

    public class PlotProfiling
    {
        public List<PlotTypeCounter> PlotTypeCounters { get; set; }
        public  List<NewExperimentType> NewExperimentTypes { get; set; }

        public PlotProfiling()
        {
            PlotTypeCounters = new List<PlotTypeCounter>();
            NewExperimentTypes = new List<NewExperimentType>();
        }
    }


    public class PlotTypeCounter
    {
        public string PlotType { get; set; }
        public int Number{ get; set; }

        public PlotTypeCounter(String plotType)
        {
            PlotType = plotType;
        }

    }
  
    //public enum PlotType
    //{
    //    GP,
    //    Ep,
    //    Mip,
    //    Vip
    //}

    public enum NewExperimentType
    {
        REX1,
        REX2,
        LUX,
        FOX,
        None
    }

}