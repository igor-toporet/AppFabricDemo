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

            // Get cache client for cache "SampleNamedCache".
            var dataCache = cacheFactory.GetCache("SampleNamedCache");

            Application.Run(new Form2(dataCache));
        }
    }
}
