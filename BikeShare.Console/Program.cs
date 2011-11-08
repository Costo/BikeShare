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
            var montreal = new MontrealBixiCrawler(svc);
            var toronto = new TorontoBixiCrawler(svc);
            var washington = new CapitalBikeShareCrawler(svc);
            var boston = new HubwayCrawler(svc);
            var minneapolis = new NiceRideMNCrawler(svc);
            
            //var london = new BarclaysCycleHireCrawler();

            while (true)
            {
                var t = Task.Factory.ContinueWhenAll( new[] { minneapolis.Run(), montreal.Run(),toronto.Run(), washington.Run(), boston.Run() }, Continue  );

             
                Task.WaitAll(t);
            }



            System.Console.ReadLine();

        }

        static void Continue(Task[] tasks)
        {
            foreach (var t in tasks)
            {
                if (t.IsFaulted)
                {
                    System.Console.WriteLine(t.Exception.Message);
                }
            }
            Thread.Sleep(60 * 1000);
        }
    }
}
