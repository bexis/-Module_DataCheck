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
        public  bool JointExperimentForest { get; set; }
        public bool JointExperimentGrld { get; set; }


        public PlotProfiling()
        {
            PlotTypeCounters = new List<PlotTypeCounter>();
            JointExperimentForest = false;
            JointExperimentGrld = false;
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
  
}