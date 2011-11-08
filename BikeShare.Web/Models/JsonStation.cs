using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Services.Entities;

namespace BikeShare.Web.Models
{
    public class JsonStation: Dictionary<string,object> 
    {
        public int id { get; set; }
        public string name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int dist { get; set; }


        public static JsonStation FromStation(Station station, int distance)
        {
            return new JsonStation
            { 
                { "id", station.Id },
                { "name", station.Name },
                { "lat", station.Latitude },
                { "lng", station.Longitude },
                { "dist", distance },
            };
        }
        public static JsonStation FromStation(Station station)
        {
            return new JsonStation
            { 
                { "id", station.Id },
                { "name", station.Name },
                { "lat", station.Latitude },
                { "lng", station.Longitude },
            };
        }

    }

}