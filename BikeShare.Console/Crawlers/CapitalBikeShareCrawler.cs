using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class CapitalBikeShareCrawler : BixiSystemCrawler
    {
        public CapitalBikeShareCrawler(BikeShareWriteService svc) : base(svc) { }
        public override string XmlDataUrl
        {
            get
            {
                return "http://capitalbikeshare.com/stations/bikeStations.xml";
            }
        }

    }
}
