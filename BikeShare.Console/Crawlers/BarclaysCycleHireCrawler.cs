using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using BikeShare.Services.Entities;
using System.Diagnostics;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class BarclaysCycleHireCrawler : ICrawler
    {
        readonly BikeShareWriteService svc;
        public BarclaysCycleHireCrawler(BikeShareWriteService svc)
        {
            this.svc = svc;
        }
        public void Run()
        {
            while (true)
            {
                var fetch = CommonTasks.ExecuteScript("Crawlers\\Scripts\\BarclaysCycleHire.js");
                var parse = fetch.ContinueWith<Station[]>(Parse, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
                var update = parse.ContinueWith(Update, TaskContinuationOptions.OnlyOnRanToCompletion);

                try
                {
                    Task.WaitAll(fetch, parse, update);
                }
                catch (AggregateException e)
                {
                    e.Handle(x =>
                    {
                        System.Console.WriteLine(x.Message);
                        return true;
                    });
                }
                Thread.Sleep(TimeSpan.FromMinutes(1d));
            }
        }

        private Station[] Parse(Task<string> t)
        {
            System.Console.WriteLine("barclayscyclehire: parsing " + t.Result.Length + " bytes");
            var stations = new List<Station>();
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(t.Result);
            foreach (var m in json)
            {
                var station = m.station;
                stations.Add(new Station((int)station.id)
                {
                    Latitude = (double)station.lat,
                    Longitude = (double)station.@long,
                    Name = station.name,
                    BikesAvailable = station.nbBikes,
                    DocksAvailable = station.nbEmptyDocks

                });
            }
            return stations.ToArray();
        }

        private void Update(Task<Station[]> t)
        {
            var stations = t.Result;
            System.Console.WriteLine("barclayscyclehire: storing " + stations.Length + " stations");

            svc.Store("barclayscyclehire", stations);

        }

    }
}
