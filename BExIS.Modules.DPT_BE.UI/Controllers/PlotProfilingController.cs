using BExIS.Modules.DPT_BE.UI.Helper;
using BExIS.Modules.DPT_BE.UI.Model;
using BExIS.Modules.DPT_BE.UI.Models;
using BExIS.Security.Services.Subjects;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
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
            string epRefDatasetId = Models.Settings.get("epRefDataset").ToString();
            var datasetObject = DataAccess.GetDatasetInfo(epRefDatasetId, GetServerInformation());
            DataTable epPlotRefTable = DataAccess.GetData(epRefDatasetId, long.Parse(datasetObject.DataStructureId), GetServerInformation());

            PlotModel model = new PlotModel();

            foreach(var plot in plots)
            {

            }

            return null;
        }


        /// <summary>
        /// Get server information form json file in workspace
        /// </summary>
        /// <returns></returns>
        public ServerInformation GetServerInformation()
        {
            //string filePath = Path.Combine(AppConfiguration.GetModuleWorkspacePath("LUI"), "Credentials.json");
            //string text = System.IO.File.ReadAllText(filePath);
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