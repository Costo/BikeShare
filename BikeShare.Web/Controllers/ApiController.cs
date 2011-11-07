using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Services;

namespace BikeShare.Web.Controllers
{
    public class ApiController : Controller
    {
        public ActionResult Index()
        {
            var systems = new[]
                {
                    new { Id="montrealbixi", Name="Montreal Bixi", data= Url.Action("System", new { id="montrealbixi"}) },
                    new { Id="torontbixi", Name="Toronto Bixi", data= Url.Action("System", new { id="torontbixi"}) },
                    new { Id="capitalbikeshare", Name="Cspital Bike Share", data= Url.Action("System", new { id="capitalbikeshare"}) },
                    new { Id="hubway", Name="Hubway", data= Url.Action("System", new { id="hubway"}) },
                };
            
            return Json(systems, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult System(string id)
        {
            var svc = new BikeShareReadService();
            var stations = svc.GetStations(id).ToArray();

            return Json(stations, JsonRequestBehavior.AllowGet);
        }

    }
}
