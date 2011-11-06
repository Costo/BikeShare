using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Entities;
using System.Net;
using System.Xml.Linq;

namespace BikeShare.Crawlers
{
    public abstract class BixiSystemCrawler : ICrawler
    {
        public abstract string XmlDataUrl { get; }
        public Task Run()
        {
            Console.WriteLine("starting task");

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
            Console.WriteLine("Result length: " + t.Result.Length);
            var xml = XDocument.Parse(t.Result);
            return from s in xml.Root.Elements("station")
                   select new Station { };
        }

        private void Update(Task<IEnumerable<Station>> t)
        {
            Console.WriteLine("Stations: " + t.Result.Count());

        }
    }
}
