using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class TorontoBixiCrawler : BixiSystemCrawler
    {
        public TorontoBixiCrawler(BikeShareWriteService svc) : base(svc) { } 
        public override string XmlDataUrl
        {
            get { return "https://toronto.bixi.com/data/bikeStations.xml"; }
        }
    }
}
