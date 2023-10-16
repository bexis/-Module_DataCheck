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

        public int NumberOfAllPlots { get; set; }

        public int NumberOfDuplicates { get; set; }

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

        public bool Grassland { get; set; }
        public bool Forest { get; set; }
        public bool REX1  { get; set; }
        public bool REX2 { get; set; }
        public bool LUX { get; set; }
        public bool FOX { get; set; }
        public  bool JointExperimentForest { get; set; }
        public bool JointExperimentGrld { get; set; }


        public PlotProfiling()
        {
            PlotTypeCounters = new List<PlotTypeCounter>();
            JointExperimentForest = false;
            JointExperimentGrld = false;
            REX1 = false;
            REX2 = false;
            LUX = false;
            FOX = false;
            Grassland = false;
            Forest = false;
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