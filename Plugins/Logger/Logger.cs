using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Plugins.Logger.Views;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Plugins.Logger
{
    public class Logger : IPlugin
    {
        private Models.Logger m_logger;

        public Logger()
        {
            info = new PluginInfo(
                name: "Logger",
                displayName: "ログ出力",
                version: Assembly.GetExecutingAssembly().GetName().Version,
                hasSettingsDialog: true);
            configuration = new PluginConfiguration(10);
        }

        public string BasePath
        {
            get { return Repository["BasePath"] as string ?? ""; }
            set { Repository["BasePath"] = value; }
        }

        #region IPlugin メンバー

        private readonly PluginInfo info;
        public PluginInfo PluginInfo { get { return info; } }

        private readonly PluginConfiguration configuration;
        public PluginConfiguration PluginConfiguration { get { return configuration; } }

        public Dictionary<string, object> Repository { get; set; }

        public void Initialize()
        {
            if (!Repository.ContainsKey("BasePath"))
                Repository["BasePath"] = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + Path.DirectorySeparatorChar + "log";
        }

        public void ShowSettingsDialog()
        {
            var view = new Settings();
            view.DataContext = this;
            view.ShowDialog();
        }

        public Task OnBroadcastedAsync(BroadcastingParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                m_logger = Models.Logger.StartNew(BasePath, parameter.BroadcastParameter.Name);
                var param = parameter.BroadcastParameter;
                m_logger.Insert("", "", param.Genre, param.Description, param.Comment);
            });
        }

        public Task OnUpdatedAsync(UpdatedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (m_logger == null)
                    return;
                var param = parameter.UpdateParameter;
                m_logger.Insert("", "", param.Genre, param.Description, param.Comment);
            });
        }

        public Task OnStopedAsync(StoppedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (m_logger == null)
                    return;
                m_logger.Insert("", "", "", "", "（配信終了）");
            });
        }

        public Task OnTickedAsync(IChannel channel)
        {
            return Task.Factory.StartNew(() =>
            {
                if (m_logger == null)
                    return;
                m_logger.Insert(
                    channel.TotalRelays.ToString(),
                    channel.TotalListeners.ToString(),
                    "", "", "");
            });
        }

        public Task OnInterruptedAsync(InterruptedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                m_logger = Models.Logger.StartNew(BasePath, parameter.Name);
                m_logger.Insert("", "", parameter.Genre, parameter.Description, parameter.Comment);
            });
        }

        public void Terminate()
        {
            m_logger = null;
        }

        #endregion
    }
}
