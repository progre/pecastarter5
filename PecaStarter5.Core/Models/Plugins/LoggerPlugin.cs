using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class LoggerPlugin : IPlugin
    {
        private Logger m_logger;

        public LoggerPlugin()
        {
            BasePath = "";
        }

        public bool IsEnabled { get; set; }
        public string BasePath { get; set; }

        #region IPlugin メンバー

        public Task OnBroadcastedAsync(BroadcastedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                m_logger = Logger.StartNew(BasePath, parameter.BroadcastParameter.Name);
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

        public Task OnStopedAsync(StopedParameter parameter)
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
                m_logger = Logger.StartNew(BasePath, parameter.Name);
                m_logger.Insert("", "", parameter.Genre, parameter.Description, parameter.Comment);
            });
        }

        #endregion
    }
}
