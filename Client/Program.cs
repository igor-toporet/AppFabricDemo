using System;
using System.Windows.Forms;
using Microsoft.ApplicationServer.Caching;

namespace Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Use configuration from the application configuration file.
            var cacheFactory = new DataCacheFactory();

            // Get cache client for cache "NamedCache1".
            var dataCache = cacheFactory.GetCache("NamedCache1");

            // Add an object to the cache.
            dataCache.Add("helloKey", "hello world");

            Application.Run(new Form1());
        }
    }
}
