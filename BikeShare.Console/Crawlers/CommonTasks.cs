﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;

namespace BikeShare.Crawlers
{
    public static class CommonTasks
    {
        public static Task<string> DownloadString(Uri uri)
        {
            System.Console.WriteLine("get: " + uri);
            var tcs = new TaskCompletionSource<string>();
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += (s, e) =>
            {
                try
                {
                    tcs.SetResult(e.Result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }

            };
            webClient.Encoding = Encoding.UTF8;
            webClient.DownloadStringAsync(uri);
            return tcs.Task;
        }

        

        public static Task<string> ExecuteScript(string path)
        {
            var tcs = new TaskCompletionSource<string>();
            var result = new StringBuilder();
            var process = new Process();
            process.StartInfo.Arguments = path;
            process.StartInfo.FileName = ConfigurationManager.AppSettings["nodePath"];
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += (s,e)=>
                {
                    result.Append(e.Data );
                };
            process.Exited += (s,e)=>
                {
                    tcs.SetResult(result.ToString());
                };
            process.Start();
            process.BeginOutputReadLine();

            return tcs.Task;
        }

    }
}
