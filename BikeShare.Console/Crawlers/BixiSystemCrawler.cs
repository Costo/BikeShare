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
        public Task Run()
        {
            System.Console.WriteLine(SystemId + ": starting");

            return Fetch()
                .ContinueWith<IEnumerable<Station>>(Parse)
                .ContinueWith(Update);

        }

        private Task<string> Fetch()
        {
            System.Console.WriteLine(SystemId + ": fetching " + XmlDataUrl);
            var tcs = new TaskCompletionSource<string>();
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (s, e) =>
            {
                tcs.SetResult(e.Result);
            };
            webClient.Encoding = Encoding.UTF8;
            webClient.DownloadStringAsync(new Uri(XmlDataUrl));
            return tcs.Task;
        }

        private IEnumerable<Station> Parse(Task<string> t)
        {
            System.Console.WriteLine(SystemId + ": parsing " + t.Result.Length + " bytes");
            var xml = XDocument.Parse(t.Result);
            return from s in xml.Root.Elements("station")
                   select Station.FromXml(s);
        }

        private void Update(Task<IEnumerable<Station>> t)
        {
            var stations = t.Result.ToArray();
            System.Console.WriteLine(SystemId + ": storing " + stations.Length + " stations");

            svc.Store(this.SystemId, stations);
            
        }
    }
}
