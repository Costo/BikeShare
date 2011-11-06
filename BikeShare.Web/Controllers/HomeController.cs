using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Services;

namespace BikeShare.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var svc = new BikeShareReadService();
            var stations = svc.GetStations();

            var bikeCount = stations.Sum(x => x.BikesAvailable);

            return View(bikeCount);
            
        }

    }
}
