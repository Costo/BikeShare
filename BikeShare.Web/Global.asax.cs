using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Enyim.Caching.Configuration;
using System.Configuration;
using BikeShare.Services;

namespace BikeShare.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "Near",
                "Api/{id}/near/{latitude},{longitude}/radius/{radius}",
                new { controller = "Api", action = "Near" }
            );


        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeMemcachedClient();
        }

        private void InitializeMemcachedClient()
        {
            var config = new MemcachedClientConfiguration();
            var address = ConfigurationManager.AppSettings.Get("MemcachedAddress");
            var port = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MemcachedPort"));
            config.AddServer(address, port);
            Cache.Initialize(config);
        }
    }
}