using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Enyim.Caching.Memcached;

namespace BikeShare.Entities
{
    [Serializable]
    public class Station
    {
        public Station(int id)
        {
            this.id = id;
        }
        readonly int id;
        public int Id { get { return id; } }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BikesAvailable { get; set; }
        public int DocksAvailable { get; set; }

        public static Station FromXml(XElement xml)
        {
            var station = new Station((int)xml.Element("id"))
            {
                Name = (string)xml.Element("name"),
                Latitude = (double)xml.Element("lat"),
                Longitude = (double)xml.Element("long"),
                BikesAvailable = (int)xml.Element("nbBikes"),
                DocksAvailable = (int)xml.Element("nbEmptyDocks"),
            };
            return station;
        }

        public void Store()
        {
        }

    }
}
