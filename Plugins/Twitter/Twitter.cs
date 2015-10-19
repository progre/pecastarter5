using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Progressive.Peercast4Net.Datas;
using System.Reflection;
using Progressive.PecaStarter5.Plugins.Twitter.Views;
using System.Collections.ObjectModel;
using System.Net;

namespace Progressive.PecaStarter5.Plugins.Twitter
{
    public class Twitter : IPlugin
    {
        public Twitter()
        {
            info = new PluginInfo("Twitter",
                "Twitter",
                Assembly.GetExecutingAssembly().GetName().Version,
                true);
            configuration = new PluginConfiguration(0);
        }

        public bool HasUrl
        {
            get { return (bool)Repository["HasUrl"]; }
            set { Repository["HasUrl"] = value; }
        }
        public string UrlBase
        {
            get { return (string)Repository["UrlBase"]; }
            set { Repository["UrlBase"] = value; }
        }
        public bool HasPeercastHashtag
        {
            get { return (bool)Repository["HasPeercastHashtag"]; }
            set { Repository["HasPeercastHashtag"] = value; }
        }
        public bool HasPecaStarterHashtag
        {
            get { return (bool)Repository["HasPecaStarterHashtag"]; }
            set { Repository["HasPecaStarterHashtag"] = value; }
        }

        #region IPlugin メンバー

        private readonly PluginInfo info;
        public PluginInfo PluginInfo { get { return info; } }

        private readonly PluginConfiguration configuration;
        public PluginConfiguration PluginConfiguration { get { return configuration; } }

        public Dictionary<string, object> Repository { get; set; }

        public void Initialize()
        {
            if (!Repository.ContainsKey("HasUrl"))
                Repository["HasUrl"] = true;
            if (!Repository.ContainsKey("UrlBase"))
                Repository["UrlBase"] = "http://www.prgrssv.net/izayoi/pecaredirect/";
            if (!Repository.ContainsKey("HasPeercastHashtag"))
                Repository["HasPeercastHashtag"] = false;
            if (!Repository.ContainsKey("HasPecaStarterHashtag"))
                Repository["HasPecaStarterHashtag"] = false;
        }

        public void ShowSettingsDialog()
        {
            var view = new Views.Settings();
            view.DataContext = this;
            view.ShowDialog();
        }

        public Task OnBroadcastedAsync(BroadcastingParameter parameter)
        {
            var list = new List<string>();
            if ((bool)Repository["HasPeercastHashtag"])
                list.Add("peercast");
            if ((bool)Repository["HasPecaStarterHashtag"])
                list.Add("pecastarter");
            return Task.Factory.StartNew(() =>
            {
                var message = GetMessage(parameter.BroadcastParameter);
                if (HasUrl)
                {
                    list.Add("peercast_yp");
                    var client = new WebClient();
                    var ipApi = "http://ipv4bot.whatismyipaddress.com/";
                    var ip = Encoding.ASCII.GetString(client.DownloadData(ipApi));
                    var url = CreateUrl(parameter.Id, ip, parameter.Settings.Port);
                    var shortUrlApi = "http://is.gd/create.php?format=simple&url=" + Uri.EscapeDataString(url);
                    var shortUrl = Encoding.ASCII.GetString(client.DownloadData(shortUrlApi));
                    message += " " + shortUrl;
                }
                new TwitterModel().Tweet(message, list);
            });
        }

        private string GetMessage(BroadcastParameter parameter)
        {
            return "PeerCastで配信中！" + parameter.Name
                + " [" + parameter.Genre + " - " + parameter.Description + "]「"
                + parameter.Comment + "」";
        }

        private string CreateUrl(string id, string ip, int port)
        {
            return UrlBase + "?id=" + id + "&tip=" + ip + ":" + port;
        }

        public Task OnUpdatedAsync(UpdatedParameter parameter)
        {
            return null;
        }

        public Task OnStopedAsync(StoppedParameter parameter)
        {
            return null;
        }

        public Task OnTickedAsync(IChannel channel)
        {
            return null;
        }

        public Task OnInterruptedAsync(InterruptedParameter parameter)
        {
            return null;
        }

        public void Terminate()
        {
        }

        #endregion
    }
}
