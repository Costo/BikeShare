using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BikeShare.Console.Crawlers;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace BikeShare.Crawlers
{
    public class BarclaysCycleHireCrawler : ICrawler
    {
        public System.Threading.Tasks.Task Run()
        {
            System.Console.WriteLine("barclayscyclehire" + ": starting");
            var tcs = new TaskCompletionSource<string>();
            Thread t = new Thread(() =>
                {

                    WebBrowser browser = new WebBrowser();
                    browser.Navigating += (s, e) =>
                    {
                        System.Console.WriteLine("Navigating");

                    };
                    browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser_DocumentCompleted);
                    browser.Navigated += (s, e) =>
                        {
                            System.Console.WriteLine("Navigated");
                            browser.Dispose();
                            tcs.SetResult("hello");
                        };
                    browser.Navigate("https://web.barclayscyclehire.tfl.gov.uk/maps");

                });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            return tcs.Task.ContinueWith(Continue);
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Console.WriteLine("Document completed");
        }



        void Continue(Task<string> t)
        {

        }
    }
}
