using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading;

namespace CefSharpScreenshotExample
{
    class Program
    {
        static void Main(string[] args)
        {
            foo();
        }

        private static void foo()
        {
            var settings = new CefSettings();
            //settings.RemoteDebuggingPort = 8888;

            //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache");
            Console.WriteLine(path);
            settings.CachePath = path;

            //settings.MultiThreadedMessageLoop = true;
            //settings.WindowlessRenderingEnabled = true;
            //settings.ExternalMessagePump = false;
            Cef.EnableHighDPISupport();
            if (!Cef.Initialize(settings, true, null))
            {
                throw new InvalidOperationException();
            }

            var context = new RequestContext();
            var browser = new ChromiumWebBrowser("https://developer.android.com/index.html");

            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            browser.Size = new System.Drawing.Size(1366 * 2, 768 * 2);

            Console.ReadLine();
        }

        private static void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            var browser = sender as ChromiumWebBrowser;
            if (browser.IsBrowserInitialized)
            {
                //browser.SetZoomLevel(3.0);
            }

            if (!e.IsLoading)
            {
                browser.LoadingStateChanged -= Browser_LoadingStateChanged;
                //browser.SetZoomLevel(4.0);

                Thread.Sleep(5000);
                var task = browser.ScreenshotAsync();
                task.ContinueWith(x =>
                {
                    // Make a file to save screenshot to
                    var screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshot.png");
                    x.Result.Save(screenshotPath);
                });
            }
        }
    }

}
