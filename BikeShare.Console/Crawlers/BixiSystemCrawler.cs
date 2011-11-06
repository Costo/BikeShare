using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Services.Entities;
using System.Net;
using System.Xml.Linq;
using Enyim.Caching.Memcached;

namespace BikeShare.Console.Crawlers
{
    public abstract class BixiSystemCrawler : ICrawler
    {
        public abstract string XmlDataUrl { get; }
        public Task Run()
        {
            System.Console.WriteLine("starting task");

            return Fetch()
                .ContinueWith<IEnumerable<Station>>(Parse)
                .ContinueWith(Update);

        }

        private Task<string> Fetch()
        {
            var tcs = new TaskCompletionSource<string>();
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (s, e) =>
            {
                tcs.SetResult(e.Result);
            };
            webClient.DownloadStringAsync(new Uri(XmlDataUrl));
            return tcs.Task;
        }

        private IEnumerable<Station> Parse(Task<string> t)
        {
            System.Console.WriteLine("Result length: " + t.Result.Length);
            var xml = XDocument.Parse(t.Result);
            return from s in xml.Root.Elements("station")
                   select Station.FromXml(s);
        }

        private void Update(Task<IEnumerable<Station>> t)
        {
            var stations = t.Result.ToArray();
            Program.BikeShareCache.Store(StoreMode.Set, "Montreal", stations.Select(x=>x.Id).ToArray());
            foreach (var s in stations)
            {
                Program.BikeShareCache.Store(StoreMode.Set, "station_" + s.Id, s);
            }
        }
    }
}
