using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using BikeShare.Services.Entities;
using System.Threading.Tasks;
using System.Net;
using System.Xml.Linq;
using BikeShare.Services;

namespace BikeShare.Crawlers
{
    public class VelovStationCrawler :ICrawler
    {
        const string StationDataUrlFormat = "http://www.velov.grandlyon.com/velovmap/zhp/inc/DispoStationsParId.php?id={0}";
        readonly Station station;
        readonly Uri uri;
        readonly BikeShareWriteService svc;
        public VelovStationCrawler(Station station, BikeShareWriteService svc)
        {
            this.svc = svc;
            this.station = station;
            this.uri = new Uri(string.Format(StationDataUrlFormat, station.Id), UriKind.Absolute); 
        }
        public void Run()
        {
            var fetch = Fetch();
            var parse = fetch.ContinueWith(Parse,  TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
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

        }

        private Task<string> Fetch()
        {
            System.Console.WriteLine("velov: fetching " + uri);
            var downloadTask = CommonTasks.DownloadString(uri);
            return downloadTask;
        }

        private void Parse(Task<string> t)
        {
            var xml = XDocument.Parse(t.Result);
            station.BikesAvailable = (int)xml.Root.Element("available");
            station.DocksAvailable = (int)xml.Root.Element("free");
        }

        private void Store(Task t)
        {
            System.Console.WriteLine("velov: storing station" + station.Id);
            svc.Store("velov", station);
        }

    }
}
