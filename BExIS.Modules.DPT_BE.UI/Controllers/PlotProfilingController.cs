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

        public ActionResult CountPlots(string[] plots)
        {
            //get gp ref dataset 
            string gpRefDatasetId = Models.Settings.get("gpRefDataset").ToString();
            var datasetObjectGp = DataAccess.GetDatasetInfo(gpRefDatasetId, GetServerInformation());
            DataTable gpPlotRefTable = DataAccess.GetData(gpRefDatasetId, long.Parse(datasetObjectGp.DataStructureId), GetServerInformation());

            //get ep ref data set to find mips and vips
            string epRefDatasetId = Models.Settings.get("epRefDataset").ToString();
            var datasetObject = DataAccess.GetDatasetInfo(epRefDatasetId, GetServerInformation());
            DataTable epPlotRefTable = DataAccess.GetData(epRefDatasetId, long.Parse(datasetObject.DataStructureId), GetServerInformation());

            PlotModel model = new PlotModel();

            PlotTypeCounter gps = new PlotTypeCounter(PlotType.GP);
            PlotTypeCounter eps = new PlotTypeCounter(PlotType.Ep);
            PlotTypeCounter vips = new PlotTypeCounter(PlotType.Vip);
            PlotTypeCounter mips = new PlotTypeCounter(PlotType.Mip);

            Regex gpRegex = new Regex(@"^[HhSsAa]\d{1,5}$");
            Regex epRegex = new Regex(@"^[HhSsAa][Ee][WwGg]\d{1,3}$");

            List<string> errors = new List<string>();

            foreach (var plot in plots)
            {
                if (gpRegex.IsMatch(plot))
                {
                    DataRow row = gpPlotRefTable.AsEnumerable().Where(a => a.Field<string>("Plot_ID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        gps.Number++;

                        if (row.Field<string>("Plotlevel") == "EP")
                        {
                            DataRow rowEp = epPlotRefTable.AsEnumerable().Where(a => a.Field<string>("PlotID") == plot).FirstOrDefault();
                            if (rowEp != null)
                            {
                                eps.Number++;

                                if (rowEp.Field<string>("VIP") == "yes")
                                    vips.Number++;

                                if (rowEp.Field<string>("MIP") == "yes")
                                    mips.Number++;
                            }
                        }
                    } 
                }
                else if (epRegex.IsMatch(plot))
                {
                    DataRow row = epPlotRefTable.AsEnumerable().Where(a => a.Field<string>("EP_PlotID") == plot).FirstOrDefault();
                    if (row != null)
                    {
                        eps.Number++;

                        if (row.Field<string>("VIP") == "yes")
                            vips.Number++;

                        if (row.Field<string>("MIP") == "yes")
                            mips.Number++;
                    }
                }
                else
                    errors.Add("Not a vaild plot: " + plot);

            }
            model.PlotProfiling.PlotTypeCounters.Add(gps);
            model.PlotProfiling.PlotTypeCounters.Add(eps);
            model.PlotProfiling.PlotTypeCounters.Add(mips);
            model.PlotProfiling.PlotTypeCounters.Add(vips);

            return View("", model);
        }


        /// <summary>
        /// Get server information form json file in workspace
        /// </summary>
        /// <returns></returns>
        public ServerInformation GetServerInformation()
        {
            ServerInformation serverInformation = new ServerInformation();
            var uri = System.Web.HttpContext.Current.Request.Url;
            serverInformation.ServerName = uri.GetLeftPart(UriPartial.Authority) + "/";
            serverInformation.Token = GetUserToken();

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