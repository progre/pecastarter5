using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class LoggerPlugin : IPlugin
    {
        private Logger logger;

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
                logger = Logger.StartNew(BasePath, parameter.BroadcastParameter.Name);
                var param = parameter.BroadcastParameter;
                logger.Insert("", "", param.Genre, param.Description, param.Comment);
            });
        }

        public Task OnUpdatedAsync(UpdatedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                if (logger == null)
                    return;
                var param = parameter.UpdateParameter;
                logger.Insert("", "", param.Genre, param.Description, param.Comment);
            });
        }

        public Task OnStopedAsync(StopedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                if (logger == null)
                    return;
                logger.Insert("", "", "", "", "（配信終了）");
            });
        }

        public Task OnTickedAsync(string name, int relays, int listeners)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                if (logger == null)
                    return;
                logger.Insert(relays.ToString(), listeners.ToString(), "", "", "");
            });
        }

        public Task OnInterruptedAsync(InterruptedParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!IsEnabled)
                    return;
                logger = Logger.StartNew(BasePath, parameter.Name);
                logger.Insert("", "", parameter.Genre, parameter.Description, parameter.Comment);
            });
        }

        #endregion
    }
}
