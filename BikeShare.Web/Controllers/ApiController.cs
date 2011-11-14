using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BikeShare.Services;
using System.Dynamic;
using BikeShare.Web.Models;

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
                    new { Id="capitalbikeshare", Name="Capital Bike Share", data= Url.Action("System", new { id="capitalbikeshare"}) },
                    new { Id="hubway", Name="Hubway", data= Url.Action("System", new { id="hubway"}) },
                    new { Id="niceridemn", Name="Nice Ride MN", data= Url.Action("System", new { id="niceridemn"}) },
                    new { Id="velov", Name="Velov", data= Url.Action("System", new { id="velov"}) },
                };
            
            return Json(systems, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult System(string id)
        {
            var svc = new BikeShareReadService();
            var stations = from s in svc.GetStations(id)
                           select JsonStation.FromStation(s);

            return Json(stations.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Near(string id, double latitude, double longitude, int radius)
        {

            var svc = new BikeShareReadService();
            var stations = from s in svc.GetStations(id)
                           let dist = (int)CalculateDistance(latitude, longitude, s.Latitude, s.Longitude)
                           where dist < radius
                           select JsonStation.FromStation(s, dist);

            return Json(stations.ToArray(), JsonRequestBehavior.AllowGet);

        }

        private double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double DegreesToRadians = Math.PI / 180;
            const double EarthRadiusInMeters = 6376 * 1000;

            var lat1InRads = lat1 * DegreesToRadians;
            var lat2InRads = lat2 * DegreesToRadians;
            var lng1InRads = lng1 * DegreesToRadians;
            var lng2InRads = lng2 * DegreesToRadians;


            //2*asin(sqrt((sin((lat1-lat2)/2))^2 + cos(lat1)*cos(lat2)*(sin((lon1-lon2)/2))^2))

            var distanceInRadians = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((lat1InRads - lat2InRads) / 2), 2)
                + Math.Cos(lat1InRads) * Math.Cos(lat2InRads) * Math.Pow(Math.Sin((lng1InRads - lng2InRads) / 2), 2)));

            return distanceInRadians * EarthRadiusInMeters;
        }

    }
}
