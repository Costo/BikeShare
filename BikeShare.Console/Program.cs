using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using BikeShare.Services.Entities;
using System.Threading;
using BikeShare.Console.Crawlers;

namespace BikeShare.Console
{
    class Program
    {
        public static MemcachedClient BikeShareCache;
        static void Main(string[] args)
        {
            BikeShareCache = new MemcachedClient();
            var montreal = new MontrealBixiCrawler();

            montreal.Run().ContinueWith(Continue);

            System.Console.ReadLine();

        }

        static void Continue(Task t)
        {
            Thread.Sleep(60 * 1000);
            Main(null);
        }
    }
}
