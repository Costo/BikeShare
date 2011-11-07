using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class HubwayCrawler :  BixiSystemCrawler
    {
        public HubwayCrawler(BikeShareWriteService svc) : base(svc) { } 
        public override string XmlDataUrl
        {
            get { return "http://www.thehubway.com/data/stations/bikeStations.xml"; }
        }
    }
}
