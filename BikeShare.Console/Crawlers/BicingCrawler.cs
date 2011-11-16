using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services.Entities;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class BicingCrawler : ICrawler
    {
        readonly BikeShareWriteService svc;
        public BicingCrawler(BikeShareWriteService svc)
        {
            this.svc = svc;
        }
        public void Run()
        {
            while (true)
            {
                var fetch = CommonTasks.ExecuteScript("Crawlers\\Scripts\\Bicing.js");
                var parse = fetch.ContinueWith<Station[]>(Parse, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
                var update = parse.ContinueWith(Store, TaskContinuationOptions.OnlyOnRanToCompletion);

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
                Thread.Sleep(TimeSpan.FromMinutes(4d));
            }
        }

        private Station[] Parse(Task<string> t)
        {
            System.Console.WriteLine("bicing: parsing " + t.Result.Length + " bytes");
            var stations = new List<Station>();
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(t.Result);
            foreach (var m in json)
            {
                var qs = HttpUtility.ParseQueryString("?" + (string)m.data);
                var name = Encoding.GetEncoding("iso-8859-15").GetString(Convert.FromBase64String(qs["addressnew"]));
                stations.Add(new Station(int.Parse(qs["idStation"]))
                {
                    Latitude = (double)m.lat,
                    Longitude = (double)m.@long,
                    Name = name,
                });
            }
            return stations.ToArray();
        }

        private void Store(Task<Station[]> t)
        {
            var stations = t.Result;
            System.Console.WriteLine("bicing: storing " + stations.Length + " stations");

            svc.Store("bicing", stations);

        }


    }
}
