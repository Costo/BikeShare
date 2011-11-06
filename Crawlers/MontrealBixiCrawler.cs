using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeShare.Crawlers
{
    public class MontrealBixiCrawler: BixiSystemCrawler
    {
        public override string XmlDataUrl { get { return "https://profil.bixi.ca/data/bikeStations.xml"; } }
    }
}
