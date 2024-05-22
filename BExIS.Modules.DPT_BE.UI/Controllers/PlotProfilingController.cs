using BExIS.UI.Helpers;
using System.Web.Mvc;
using Vaiona.Web.Extensions;
using Vaiona.Web.Mvc.Models;

namespace BExIS.Modules.DPT_BE.UI.Controllers
{
    public class PlotProfilingController : Controller
    {
        // GET: PlotProfiling
        public ActionResult Index()
        {
            ViewBag.Title = PresentationModel.GetViewTitleForTenant("Plot Profiling", this.Session.GetTenant());
            string module = "DPT_BE";

            ViewData["app"] = SvelteHelper.GetApp(module);
            ViewData["start"] = SvelteHelper.GetStart(module);

            return View();
        }
    }
}