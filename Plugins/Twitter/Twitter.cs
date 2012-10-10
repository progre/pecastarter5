using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Progressive.Peercast4Net.Datas;
using System.Reflection;

namespace Progressive.PecaStarter5.Plugins.Twitter
{
    public class Twitter : IPlugin
    {
        public Twitter()
        {
            pluginInfo = new PluginInfo("Twitter",
                "Twitter",
                Assembly.GetExecutingAssembly().GetName().Version,
                false);
        }

        #region IPlugin メンバー

        private readonly PluginInfo pluginInfo;
        public PluginInfo PluginInfo
        {
            get { return pluginInfo; }
        }

        public Dictionary<string, object> Repository { get { return null; } set { } }

        public void Initialize()
        {
        }

        public void ShowSettingsDialog()
        {
        }

        public Task OnBroadcastedAsync(BroadcastingParameter parameter)
        {
            return Task.Factory.StartNew(() => Process.Start(
                "https://twitter.com/intent/tweet?text="
                   + GetMessage(parameter.BroadcastParameter)));
        }

        private string GetMessage(BroadcastParameter parameter)
        {
            var message = "Peercastで配信中！" + parameter.Name + " [" + parameter.Genre + " - " + parameter.Description + "]「" + parameter.Comment + "」";
            if (message.Length > 140)
            {
                message.Remove(138);
                message += "…";
            }
            return message;
        }

        public Task OnUpdatedAsync(UpdatedParameter parameter)
        {
            return null;
        }

        public Task OnStopedAsync(StoppedParameter parameter)
        {
            return null;
        }

        public Task OnTickedAsync(string name, int relays, int listeners)
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
