using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BikeShare.Crawlers
{
    public static class CommonTasks
    {
        public static Task<string> DownloadString(Uri uri)
        {

            var tcs = new TaskCompletionSource<string>();
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (s, e) =>
            {
                tcs.SetResult(e.Result);
            };
            webClient.Encoding = Encoding.UTF8;
            webClient.DownloadStringAsync(uri);
            return tcs.Task;
        }
    }
}
