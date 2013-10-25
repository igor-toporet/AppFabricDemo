using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.ApplicationServer.Caching;

namespace Client
{
    public partial class Form2 : Form
    {
        private readonly DataCache _dataCache;
        private readonly IDictionary<string, DataCacheItemVersion> _prevVersions = new Dictionary<string, DataCacheItemVersion>();
        private readonly IList<string> _rememberedKeys = new List<string>();

        private const DataCacheOperations ItemFilter = DataCacheOperations.AddItem
                                                       | DataCacheOperations.RemoveItem
                                                       | DataCacheOperations.ReplaceItem;

        private const DataCacheOperations RegionFilter = DataCacheOperations.ClearRegion
                                                         | DataCacheOperations.CreateRegion
                                                         | DataCacheOperations.RemoveRegion;


        public Form2(DataCache dataCache)
        {
            _dataCache = dataCache;
            InitializeComponent();


            //var cacheLevelCallback = _dataCache.AddCacheLevelCallback(ItemFilter | RegionFilter, CacheLevelCallback);
            var cacheLevelBulkCallback = _dataCache.AddCacheLevelBulkCallback(CacheLevelBulkCallback);
            //var failureNotificationCallback = _dataCache.AddFailureNotificationCallback(FailureNotificationCallback);
            //var itemLevelCallback = _dataCache.AddItemLevelCallback("MagicKey", ItemFilter, ItemLevelCallback);
        }

        private void ItemLevelCallback(string cachename,
                                       string regionname,
                                       string key,
                                       DataCacheItemVersion version,
                                       DataCacheOperations operations,
                                       DataCacheNotificationDescriptor nd)
        {
            Log("Item '{0}' operations {1}", key, operations);
        }

        private void FailureNotificationCallback(string cachename,
                                                 DataCacheNotificationDescriptor nd)
        {
            Log("Received Failure Notification");
        }

        private void CacheLevelBulkCallback(string cachename,
                                            IEnumerable<DataCacheOperationDescriptor> operations,
                                            DataCacheNotificationDescriptor nd)
        {
            foreach (var operation in operations)
            {
                Log("{0}", operation);
            }
        }

        private void CacheLevelCallback(string cachename,
                                        string regionname,
                                        string key,
                                        DataCacheItemVersion version,
                                        DataCacheOperations cacheOperations,
                                        DataCacheNotificationDescriptor nd)
        {
            Log("{0}: '{1}'", cacheOperations, key);
        }

        private void Log(string format, params object[] args)
        {
            var currentTime = DateTime.Now.ToString("HH:mm:ss.FFFFFF");
            string message = string.Format(format, args);
            string logEntry = currentTime + '\t' + message + Environment.NewLine;

            txtLog.BeginInvoke(new Action<string>(AppendLogEntryToTextBox), logEntry);
        }

        private void AppendLogEntryToTextBox(string logEntry)
        {
            txtLog.AppendText(logEntry);
        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            txtKey.Focus();
        }

        private void btnPut_Click(object sender, EventArgs e)
        {
            var value = string.IsNullOrWhiteSpace(Value) ? Key : Value;
            var version = _dataCache.Put(Key, value /*,TimeSpan.FromSeconds(7) */);

            Log("+++ '{0}' : '{1}'", Key, Value);

            txtKey.Select(0, Key.Length);
            txtKey.Focus();


            //var prevVersion = _prevVersions.ContainsKey(Key) ? _prevVersions[Key] : null;

            //Log("Item with the key '{0}' has been put into cache (new version is greater than previous: {1})",
            //    Key, version > prevVersion);

            //_prevVersions[Key] = version;
        }

        private string Value
        {
            get { return txtValue.Text; }
        }

        private string Key
        {
            get { return txtKey.Text; }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var success = _dataCache.Remove(Key);

            var successMsg = success ? "Success" : "Fail";
            Log("--- '{0}' ({1})", Key, successMsg);
        }

        private void btnPutMany_Click(object sender, EventArgs e)
        {
            var random = new Random();
            var count = random.Next(100, 1000);
            var sleepMilliseconds = random.Next(5, 50);

            Log("Putting {0} items to the cache (sleep after each {1} ms)...", count, sleepMilliseconds);

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < count; i++)
            {
                var ticks = DateTime.Now.Ticks.ToString();

                _dataCache.Put(ticks, ticks);
                //_dataCache.AddItemLevelCallback(ticks, ItemFilter, ItemLevelCallback);
                _rememberedKeys.Add(ticks);

                Thread.Sleep(sleepMilliseconds);
            }

            stopwatch.Stop();
            Log("Done putting {0} items to the cache (took {1} s)", count, stopwatch.Elapsed.TotalSeconds);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            foreach (var key in _rememberedKeys)
            {
                bool success = _dataCache.Remove(key);
            }

            _rememberedKeys.Clear();
        }

        private void txtKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnPut.PerformClick();
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }
    }
}