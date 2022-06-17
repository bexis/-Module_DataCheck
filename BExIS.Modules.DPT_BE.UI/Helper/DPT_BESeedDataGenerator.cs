using BExIS.Security.Entities.Objects;
using BExIS.Security.Services.Objects;
using System;
using System.Linq;

namespace BExIS.Modules.DPT_BE.UI.Helpers
{
    public class DPT_BESeedDataGenerator : IDisposable
    {
        public void GenerateSeedData()
        {
            FeatureManager featureManager = new FeatureManager();
            OperationManager operationManager = new OperationManager();

            try
            {
                Feature rootDataToolsFeature = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Data Tools"));
                if (rootDataToolsFeature == null) rootDataToolsFeature = featureManager.Create("Data Tools", "Data Tools");

                Feature plotProfilingFeature = featureManager.FeatureRepository.Get().FirstOrDefault(f => f.Name.Equals("Plot Profiling"));
                if (plotProfilingFeature == null) plotProfilingFeature = featureManager.Create("Plot Profiling", "Plot Profiling", rootDataToolsFeature);

                operationManager.Create("DPT_BE", "PlotProfiling", "*", plotProfilingFeature);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                featureManager.Dispose();
                operationManager.Dispose();
            }

        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
