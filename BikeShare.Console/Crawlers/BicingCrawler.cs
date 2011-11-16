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
using HtmlAgilityPack;
using System.Net;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

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
                var parse = fetch.ContinueWith<Tuple<Station, NameValueCollection>[]>(Parse, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
                var store = parse.ContinueWith(Store, TaskContinuationOptions.OnlyOnRanToCompletion);

                try
                {
                    Task.WaitAll(fetch, parse, store);
                }
                catch (AggregateException e)
                {
                    e.Handle(x =>
                    {
                        System.Console.WriteLine(x.Message);
                        return true;
                    });
                }
                CancellationTokenSource source = new CancellationTokenSource();

                Task.Factory.StartNew(() => StationLoop(parse.Result), source.Token);

                Thread.Sleep(TimeSpan.FromHours(12));
                source.Cancel();
            }
        }

        private Tuple<Station, NameValueCollection>[] Parse(Task<string> t)
        {
            System.Console.WriteLine("bicing: parsing " + t.Result.Length + " bytes");
            var result = new List<Tuple<Station, NameValueCollection>>();
            dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(t.Result);
            foreach (var m in json)
            {
                var qs = HttpUtility.ParseQueryString("?" + (string)m.data);
                var name = Encoding.GetEncoding("iso-8859-15").GetString(Convert.FromBase64String(qs["addressnew"]));
                result.Add(Tuple.Create(new Station(int.Parse(qs["idStation"]))
                {
                    Latitude = (double)m.lat,
                    Longitude = (double)m.@long,
                    Name = name,
                }, qs));
            }
            return result.ToArray();
        }

        private void Store(Task<Tuple<Station, NameValueCollection>[]> t)
        {
            var stations = t.Result.Select(x=>x.Item1).ToArray();
            System.Console.WriteLine("bicing: storing " + stations.Length + " stations");

            svc.Store("bicing", stations);

        }

        public void StationLoop(Tuple<Station, NameValueCollection>[] stations)
        {
            while (true)
            {
                try
                {
                    var forEach = Parallel.ForEach(stations, x => new StationCrawler(x.Item1, x.Item2, svc).Run());
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

        private class StationCrawler :ICrawler
        {
            readonly Station station;
            readonly NameValueCollection values;
            readonly BikeShareWriteService svc;
            static Uri Uri = new Uri("http://www.bicing.cat/CallWebService/StationBussinesStatus_Cache.php");
            public StationCrawler(Station station, NameValueCollection values, BikeShareWriteService svc)
            {
                this.station = station;
                this.values = values;
                this.svc = svc;
            }
            public void Run()
            {
                var fetch = Fetch();
                var parse = fetch.ContinueWith(Parse, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously);
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

            public Task<string> Fetch()
            {
                System.Console.WriteLine("post: " + Uri);
                var tcs = new TaskCompletionSource<string>();
                var webClient = new WebClient();
                webClient.UploadValuesCompleted += (s, e) =>
                {
                    try
                    {
                        tcs.SetResult(Encoding.GetEncoding("iso-8859-15").GetString(e.Result));
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }

                };
                webClient.Encoding = Encoding.UTF8;
                webClient.UploadValuesAsync(Uri, values);
                return tcs.Task;
            }

            private void Parse(Task<string> t)
            {
                const string pattern = "([0-9]+)"; 
                var doc = new HtmlDocument();
                doc.LoadHtml(t.Result );
                var div = doc.DocumentNode.SelectNodes("/div/div[last()]");
                var matches = Regex.Matches(div[0].InnerText, pattern);

                station.BikesAvailable = int.Parse(matches[0].Value);
                station.DocksAvailable = int.Parse(matches[0].Value);
            }
            private void Store(Task t)
            {
                System.Console.WriteLine("bicing: storing station" + station.Id);
                svc.Store("bicing", station);
            }

        }


    }
}
