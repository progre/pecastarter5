using System.Threading.Tasks;
using Progressive.PecaStarter5.Plugin;
using Logger.Views;
using System.Windows;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Progressive.PecaStarter5.Plugins.Logger
{
    public class Logger : IPlugin
    {
        private Models.Logger m_logger;

        public Logger()
        {
            BasePath = "";
        }

        public string Name { get { return "ログ出力"; } }
        public Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public bool IsEnabled { get; set; }
        public string BasePath { get; set; }

        #region IPlugin メンバー

        public bool HasSettingsDialog { get { return true; } }

        public void ShowSettingsDialog()
        {
            new Settings().ShowDialog();
        }

        public IDictionary<string, object> Repository
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public Task OnBroadcastedAsync(BroadcastingParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                m_logger = Models.Logger.StartNew(BasePath, parameter.BroadcastParameter.Name);
                var param = parameter.BroadcastParameter;
                m_logger.Insert("", "", param.Genre, param.Description, param.Comment);
            });
        }

        public Task OnUpdatedAsync(UpdatedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
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
                if (!IsEnabled)
                    return;
                if (m_logger == null)
                    return;
                m_logger.Insert("", "", "", "", "（配信終了）");
            });
        }

        public Task OnTickedAsync(string name, int relays, int listeners)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                if (m_logger == null)
                    return;
                m_logger.Insert(relays.ToString(), listeners.ToString(), "", "", "");
            });
        }

        public Task OnInterruptedAsync(InterruptedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                m_logger = Models.Logger.StartNew(BasePath, parameter.Name);
                m_logger.Insert("", "", parameter.Genre, parameter.Description, parameter.Comment);
            });
        }

        #endregion
    }
}
