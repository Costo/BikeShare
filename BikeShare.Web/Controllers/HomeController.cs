using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Services;
using System.Dynamic;

namespace BikeShare.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult System(string id)
        {
            var svc = new BikeShareReadService();
            var stations = svc.GetStations(id).ToArray();

            dynamic model = new ExpandoObject();
            model.Stations = stations;

            return View(model);
        }

    }
}
