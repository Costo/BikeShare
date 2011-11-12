using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using System.Threading.Tasks;
using System.Net;
using BikeShare.Services.Entities;
using BikeShare.Services;
using System.Xml.Linq;
using System.Threading;

namespace BikeShare.Crawlers
{
    public class VelovCrawler : ICrawler
    {
        readonly BikeShareWriteService svc;
        static readonly int[] Boroughs = new[] { 69381, 69382, 69383, 69384, 69385, 69386, 69387, 69388, 69389, 69266, 69034, 69256 };
        const string StationListUrlFormat = "http://www.velov.grandlyon.com/velovmap/zhp/inc/StationsParArrondissement.php?arrondissement={0}";
        const string StationDataUrlFormat = "http://www.velov.grandlyon.com/velovmap/zhp/inc/DispoStationsParId.php?id={0}";


        public VelovCrawler(BikeShareWriteService svc)
        {
            this.svc = svc;
        }
        public void Run()
        {
            while (true)
            {
                var fetchTasks = Fetch();
                var parseTask = Task.Factory.ContinueWhenAll<string, Station[]>(fetchTasks, Parse);
                var storeTask = parseTask.ContinueWith<Station[]>(Store);

                var allTasks = new List<Task>(fetchTasks);
                allTasks.Add(parseTask);
                allTasks.Add(storeTask);

                try
                {
                    Task.WaitAll(allTasks.ToArray());
                }
                catch(AggregateException e)
                {
                    e.Handle(x =>
                    {
                        System.Console.WriteLine(x.Message);
                        return true;
                    });
                }

                CancellationTokenSource source = new CancellationTokenSource();

                Task.Factory.StartNew(() => StationLoop(parseTask.Result), source.Token);

                Thread.Sleep(TimeSpan.FromHours(12));
                source.Cancel();
            }
        }

        public void StationLoop(Station[] stations)
        {
            while(true)
            {
                try
                {
                    var forEach = Parallel.ForEach(stations, x => new VelovStationCrawler(x, svc).Run());
                }
                catch (AggregateException e)
                {
                    e.Handle(x =>
                    {
                        System.Console.WriteLine(x.Message);
                        return true;
                    });
                }
                Thread.Sleep(TimeSpan.FromMinutes(4));
            }
        }

        private Task<string>[] Fetch()
        {
            
            var tasks = from b in Boroughs
                        select FetchBorough(b);

            return tasks.ToArray();
        }

        private Task<string> FetchBorough(int id)
        {
            var url = new Uri(string.Format(StationListUrlFormat, id), UriKind.Absolute);
            System.Console.WriteLine("velov: fetching " + url);
            return CommonTasks.DownloadString(url);
        }

        private Station[] Parse(Task<string>[] tasks)
        {
            var stations = new List<Station>();
            foreach (var t in tasks)
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(t.Result);
                    foreach (var m in json.markers)
                    {
                        var station = new Station((int)m.numStation)
                        {
                            Latitude = m.x,
                            Longitude = m.y,
                            Name = m.nomStation
                        };
                        stations.Add(station);
                    }

                }
            }
            return stations.ToArray();
        }


        private Station[] Store(Task<Station[]> t)
        {
            var stations = t.Result;
            System.Console.WriteLine("velov: storing " + stations.Length + " stations");

            svc.Store("velov", stations);

            return stations;
        }

    }
}
