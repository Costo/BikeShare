using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Services.Entities;
using System.Net;
using System.Xml.Linq;
using Enyim.Caching.Memcached;
using BikeShare.Services;
using System.Threading;
using BikeShare.Crawlers;

namespace BikeShare.Console.Crawlers
{
    public abstract class BixiSystemCrawler : ICrawler
    {
        readonly BikeShareWriteService svc;
        public BixiSystemCrawler(BikeShareWriteService svc)
        {
            this.svc = svc;
        }
        public abstract string XmlDataUrl { get; }
        private string systemId;
        public string SystemId
        {
            get
            {
                return systemId
                    ?? (systemId = GetType().Name.Replace("Crawler", string.Empty).ToLowerInvariant());
            }
        }
        public void Run()
        {
            while (true)
            {
                var fetch = CommonTasks.DownloadString(new Uri(XmlDataUrl));
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
            var xml = XDocument.Parse(t.Result);
            return (from s in xml.Root.Elements("station")
                   select Station.FromXml(s)).ToArray();
        }

        private void Update(Task<Station[]> t)
        {
            var stations = t.Result;
            System.Console.WriteLine(SystemId + ": storing " + stations.Length + " stations");

            svc.Store(this.SystemId, stations);
            
        }
    }
}
