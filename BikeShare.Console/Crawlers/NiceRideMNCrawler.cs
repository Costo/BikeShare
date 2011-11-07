using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class NiceRideMNCrawler: BixiSystemCrawler
    {
        public NiceRideMNCrawler(BikeShareWriteService svc) : base(svc) { }
        public override string XmlDataUrl
        {
            get { return "http://secure.niceridemn.org/data2/bikeStations.xml"; }
        }
    }
}
