using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using BikeShare.Services.Entities;
using System.Threading;
using BikeShare.Console.Crawlers;
using BikeShare.Services;
using BikeShare.Crawlers;

namespace BikeShare.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var svc = new BikeShareWriteService();
            var crawlers = new ICrawler[] {
                new MontrealBixiCrawler(svc),
                new TorontoBixiCrawler(svc),
                new CapitalBikeShareCrawler(svc),
                new HubwayCrawler(svc),
                new NiceRideMNCrawler(svc),
                new VelovCrawler(svc)
            };
            
            
            Parallel.ForEach(crawlers, x => x.Run()); 

            System.Console.ReadLine();

        }
    }
}
