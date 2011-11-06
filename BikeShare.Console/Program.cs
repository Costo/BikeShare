using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Crawlers;
using System.Threading.Tasks;
using Enyim.Caching;
using BikeShare.Entities;
using System.Threading;

namespace BikeShare
{
    class Program
    {
        public static MemcachedClient BikeShareCache;
        static void Main(string[] args)
        {
            BikeShareCache = new MemcachedClient();
            var montreal = new MontrealBixiCrawler();

            montreal.Run().ContinueWith(Continue);

            Console.ReadLine();

        }

        static void Continue(Task t)
        {
            Thread.Sleep(60 * 1000);
            Main(null);
        }
    }
}
