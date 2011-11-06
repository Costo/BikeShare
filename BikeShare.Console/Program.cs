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

namespace BikeShare.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var montreal = new MontrealBixiCrawler( new BikeShareWriteService()  );

            montreal.Run().ContinueWith(Continue);

            System.Console.ReadLine();

        }

        static void Continue(Task t)
        {
            Thread.Sleep(30 * 1000);
            Main(null);
        }
    }
}
