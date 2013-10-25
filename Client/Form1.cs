using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.ApplicationServer.Caching;

namespace Client
{
    public partial class Form1 : Form
    {
        private readonly DataCache _dataCache;

        public Form1(DataCache dataCache)
        {
            _dataCache = dataCache;
            InitializeComponent();

        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            var tags = txtTags.Text.Split(" ,;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => new DataCacheTag(t));

            var objects = _dataCache.GetObjectsByAnyTag(tags, string.Empty);

            var cacheItems = objects.Select(o=>_dataCache.GetCacheItem(o.Key));

            cacheItemsBindingSource.SuspendBinding();
            cacheItemsBindingSource.DataSource = cacheItems;
            cacheItemsBindingSource.ResetBindings(false);
            cacheItemsBindingSource.ResumeBinding();
        }

        private void btnStore_Click(object sender, EventArgs e)
        {
            var key = txtKey.Text;
            var value = txtValue.Text;
            _dataCache.Put(key, value);

            //Log("[PUT]\t'{0}' : '{1}'", key, value);
        }

        private void Log(string format, params object[] args)
        {
            var timeStamp = DateTime.Now.ToString("u");
            var message = string.Format(format, args);
            txtLog.AppendText(timeStamp + message + Environment.NewLine);
        }

       
    }
}