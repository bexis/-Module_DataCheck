using BExIS.Modules.DPT_BE.UI.Helper;
using BExIS.Modules.DPT_BE.UI.Model;
using BExIS.Modules.DPT_BE.UI.Models;
using BExIS.Security.Services.Subjects;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace BExIS.Modules.DPT_BE.UI.Controllers
{
    public class PlotProfilingController : Controller
    {
        // GET: PlotProfiling
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CountPlots(string[] plots, bool header = false)
        {
            if(header)
                plots = plots.Skip(1).ToArray();

            plots = plots.Distinct().ToArray();

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

            PlotModel model = new PlotModel();

            PlotTypeCounter gps = new PlotTypeCounter("GP");
            PlotTypeCounter eps = new PlotTypeCounter("EP");
            PlotTypeCounter vips = new PlotTypeCounter("VIP");
            PlotTypeCounter mips = new PlotTypeCounter("MIP");

            Regex gpRegex = new Regex(@"^[HhSsAa]\d{1,5}$");
            Regex epRegex = new Regex(@"^[HhSsAa][Ee][WwGg]\d{1,3}$");

            //check first if new exp plots are there and ad info to the model
            foreach (var plot in plots)
            {
                if (foxPlots.Contains(plot))
                {
                    DataRow row = foxPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Joint Experiment ID") == plot).FirstOrDefault();
                    if(row != null)
                    {
                        model.PlotProfiling.JointExperimentForest = true;

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
                    DataRow row = gNewExpPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Joint Experiment ID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        model.PlotProfiling.JointExperimentGrld = true;

                        //remove new exp plot from list
                        plotList.Remove(plot);

                        string ep = row.Field<string>("EP ID");
                        if (!plots.Contains(ep) && !plotList.Contains(ep))
                            plotList.Add(ep);
                    }
                }
            }

            foreach (var plot in plotList)
            {
                if (gpRegex.IsMatch(plot))
                {
                    DataRow row = gpPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Plot_ID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        gps.Number++;

                        switch(row.Field<string>("Plotlevel"))
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
                else if (epRegex.IsMatch(plot))
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

            return Json(model);
        }


        /// <summary>
        /// Get server information form json file in workspace
        /// </summary>
        /// <returns></returns>
        public ServerInformation GetServerInformation()
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