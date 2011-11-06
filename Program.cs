using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Crawlers;
using System.Threading.Tasks;

namespace BikeShare
{
    class Program
    {
        static void Main(string[] args)
        {
            var montreal = new MontrealBixiCrawler();
            
            Task.WaitAll(montreal.Run());

        }
    }
}
