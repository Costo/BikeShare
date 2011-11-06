using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Services;

namespace BikeShare.Console.Crawlers
{
    public class MontrealBixiCrawler: BixiSystemCrawler
    {
        public MontrealBixiCrawler(BikeShareWriteService svc) : base(svc) { }   
        public override string XmlDataUrl { get { return "https://profil.bixi.ca/data/bikeStations.xml?" + DateTime.Now.Ticks; } }
    }
}
