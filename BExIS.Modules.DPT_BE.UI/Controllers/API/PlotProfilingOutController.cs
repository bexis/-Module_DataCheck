using BExIS.App.Bootstrap.Attributes;
using BExIS.Modules.DPT_BE.UI.Helper;
using BExIS.Modules.DPT_BE.UI.Model;
using BExIS.Modules.DPT_BE.UI.Models;
using BExIS.Security.Services.Subjects;
using BExIS.Utils.Route;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace BExIS.Modules.DPT_BE.UI.Controllers
{
    public class PlotProfilingOutController : ApiController
    {
        // GET: api/DPT_BE/CountPlots
        /// <summary>
        /// Get plot counts based on plot list
        /// </summary>
        /// <returns>Summary of plot counts</returns>
        [BExISApiAuthorize]
        [PostRoute("api/DPT_BE/CountPlots")]
        public HttpResponseMessage Post([FromBody]PostApiPlotCount data)
    //    public JsonResult CountPlots(string[] plots)
        {
            PlotModel model = new PlotModel();
            model.NumberOfAllPlots = data.plots.Length;

            //count dupliactes 
            model.NumberOfDuplicates = data.plots.GroupBy(a => a).Count(a => a.Count() > 1);
            //remove dublicates
            var plots = data.plots.Distinct().ToArray();

            //list for non new exp plots
            List<string> plotList = plots.ToList();

            //get gp ref dataset 
            string gpRefDatasetId = Models.Settings.get("gpRefDataset").ToString();
            var datasetObjectGp = DataAccess.GetDatasetInfo(gpRefDatasetId, GetServerInformation());
            DataTable gpPlotRefTable = DataAccess.GetData(gpRefDatasetId, long.Parse(datasetObjectGp.DataStructureId), GetServerInformation());

            //get ep ref data set to find mips and vips
            string epRefDatasetId = Models.Settings.get("epRefDataset").ToString();
            var datasetObject = DataAccess.GetDatasetInfo(epRefDatasetId, GetServerInformation());
            DataTable epPlotRefTable = DataAccess.GetData(epRefDatasetId, long.Parse(datasetObject.DataStructureId), GetServerInformation());

            //get new experiment plots datasets
            string foxRefDatasetId = Models.Settings.get("foxRefDataset").ToString();
            var datasetObjectFox = DataAccess.GetDatasetInfo(foxRefDatasetId, GetServerInformation());
            DataTable foxPlotRefTable = DataAccess.GetData(foxRefDatasetId, long.Parse(datasetObjectFox.DataStructureId), GetServerInformation());

            string gNewExpRefDatasetId = Models.Settings.get("gNewExpDataset").ToString();
            var datasetObjectgNewExp = DataAccess.GetDatasetInfo(gNewExpRefDatasetId, GetServerInformation());
            DataTable gNewExpPlotRefTable = DataAccess.GetData(gNewExpRefDatasetId, long.Parse(datasetObjectgNewExp.DataStructureId), GetServerInformation());

            //

            //create lists with new experiment plot ids
            List<string> foxPlots = foxPlotRefTable.AsEnumerable().Select(a => a.Field<string>("Joint Experiment ID")).ToList();
            List<string> glnewExpPlots = gNewExpPlotRefTable.AsEnumerable().Select(a => a.Field<string>("Joint Experiment ID")).ToList();

            PlotTypeCounter gps = new PlotTypeCounter("GP");
            PlotTypeCounter eps = new PlotTypeCounter("EP");
            PlotTypeCounter vips = new PlotTypeCounter("VIP");
            PlotTypeCounter mips = new PlotTypeCounter("MIP");

            //check first if new exp plots are in the list and add info about it to the model
            foreach (var plot in plots)
            {
                if (foxPlots.Contains(plot))
                {
                    model.PlotProfiling.Forest = true;
                    DataRow row = foxPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Joint Experiment ID") == plot).FirstOrDefault();
                    if(row != null)
                    {
                        model.PlotProfiling.JointExperimentForest = true;
                        model.PlotProfiling.FOX = true;

                        //remove new exp plot from list
                        plotList.Remove(plot);

                        //if ep id is not in the plotlist add to count it later to Ep level
                        string ep = row.Field<string>("EP ID");
                        if(!plots.Contains(ep) && !plotList.Contains(ep))
                            plotList.Add(ep);
                    }

                }
                else if(glnewExpPlots.Contains(plot))
                {
                    model.PlotProfiling.Grassland = true;
                    DataRow row = gNewExpPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Joint Experiment ID") == plot).FirstOrDefault();

                    if (row != null)
                    {
                        model.PlotProfiling.JointExperimentGrld = true;

                        //remove new exp plot from list
                        plotList.Remove(plot);

                        string ep = row.Field<string>("EP ID");
                        if (!plots.Contains(ep) && !plotList.Contains(ep))
                            plotList.Add(ep);
                        if(row.Field<string>("REX I") == "yes")
                            model.PlotProfiling.REX1 = true;
                        if (row.Field<string>("REX II") == "yes")
                            model.PlotProfiling.REX2 = true;

                        if (row.Field<string>("LUX") == "yes")
                            model.PlotProfiling.LUX = true;


                    }
                }
            }

            //remove duplicates
            Dictionary<string, string> gpEpPlotsIDs = epPlotRefTable.AsEnumerable().ToDictionary<DataRow, string, string>(row => row.Field<string>(1),
                                row => row.Field<string>(0));
            List<string> plotWithOutDup = RemoveDuplicates(plotList, gpEpPlotsIDs, model);

            foreach (var plot in plotWithOutDup)
            {
                if (gpEpPlotsIDs.Keys.Contains(plot))
                {
                    DataRow row = gpPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Plot_ID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        if (row.Field<string>("Landuse") == "F")
                            model.PlotProfiling.Forest = true;

                        if (row.Field<string>("Landuse") == "G")
                            model.PlotProfiling.Grassland = true;

                        gps.Number++;

                        switch (row.Field<string>("Plotlevel"))
                        {
                            case "EP":
                                eps.Number++;
                                break;
                            case "MIP":
                                eps.Number++;
                                mips.Number++;
                                break;
                            case "VIP":
                                eps.Number++;
                                mips.Number++;
                                vips.Number++;
                                break;
                        }
                    }
                    else
                        model.NotVaildPlotIds.Add(plot);
                }
                else if (gpEpPlotsIDs.Values.Contains(plot))
                {
                    DataRow row = epPlotRefTable.AsEnumerable().Where(a => a.Field<string>("EP_PlotID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        gps.Number++;
                        eps.Number++;

                        if (row.Field<string>("VIP") == "yes")
                            vips.Number++;

                        if (row.Field<string>("MIP") == "yes")
                            mips.Number++;

                        if (row.Field<string>("Landuse") == "F")
                            model.PlotProfiling.Forest = true;

                        if (row.Field<string>("Landuse") == "G")
                            model.PlotProfiling.Grassland = true;

                    }
                    else
                        model.NotVaildPlotIds.Add(plot);
                }
               
                else
                    model.NotVaildPlotIds.Add(plot);

            }
            model.PlotProfiling.PlotTypeCounters.Add(gps);
            model.PlotProfiling.PlotTypeCounters.Add(eps);
            model.PlotProfiling.PlotTypeCounters.Add(mips);
            model.PlotProfiling.PlotTypeCounters.Add(vips);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            string resp = JsonConvert.SerializeObject(model);
            response.Content = new StringContent(resp, System.Text.Encoding.UTF8, "application/json");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
          //  return Json(model);
        }

        private List<string> RemoveDuplicates(List<string> plots, Dictionary<string, string> gpEpPlotsIDs, PlotModel model)
        {
            List<string> newPlotsList = new List<string>(plots);

            foreach (var plot in plots)
            {
                if (gpEpPlotsIDs.Keys.Contains(plot))
                {
                    string ep = gpEpPlotsIDs[plot];
                    if (plots.Contains(ep))
                    {
                        newPlotsList.Remove(ep);
                        model.NumberOfDuplicates++;
                    }
                }

                if(gpEpPlotsIDs.Values.Contains(plot))
                {
                    string gp = gpEpPlotsIDs.FirstOrDefault(x => x.Value == plot).Key;
                    if (plot.Contains(gp))
                    {
                        newPlotsList.Remove(gp);
                        model.NumberOfDuplicates++;
                    }
                }
            }

            return newPlotsList;
        }


        /// <summary>
        /// Get server information form json file in workspace
        /// </summary>
        /// <returns></returns>
        private ServerInformation GetServerInformation()
        {
            ServerInformation serverInformation = new ServerInformation();
            var uri = System.Web.HttpContext.Current.Request.Url;
            //serverInformation.ServerName = "http://be2020-dev.inf-bb.uni-jena.de:2010/";
            serverInformation.Token = "k4ywfsj6X32sXE62XybjtvJk5fs2JqNXyBmzkR7apBMgigwz9hiW3mFyR6uW7qy5";
            serverInformation.ServerName = uri.GetLeftPart(UriPartial.Authority) + "/";
            //serverInformation.Token = GetUserToken();

            return serverInformation;
        }

        private string GetUserToken()
        {
            var identityUserService = new IdentityUserService();
            var userManager = new UserManager();

            try
            {
                long userId = 0;
                long.TryParse(this.User.Identity.GetUserId(), out userId);

                var user = identityUserService.FindById(userId);

                user = identityUserService.FindById(userId);
                var token = userManager.GetTokenAsync(user).Result;
                return token;
            }
            finally
            {
                identityUserService.Dispose();
                userManager.Dispose();
            }
        }

    }
}