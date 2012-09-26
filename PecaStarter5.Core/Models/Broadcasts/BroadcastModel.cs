using System;
using System.Collections.Generic;
using Progressive.PecaStarter5.Plugin;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    public class BroadcastModel
    {
        private readonly PeercastService _service;
        private readonly BroadcastTimer m_timer;
        private readonly IEnumerable<IPlugin> _plugins;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        internal BroadcastModel(IEnumerable<IPlugin> plugins, PeercastService service)
        {
            _plugins = plugins;
            m_timer = new BroadcastTimer();
            m_timer.Ticked += s =>
            {
                var tuple1 = (Tuple<IYellowPages, string>)s;
                _service.OnTickedAsync(tuple1.Item1, tuple1.Item2).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnAsyncExceptionThrown(t.Exception);
                });
            };

            _service = service;
        }

        public Task<string> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            return _service.BroadcastAsync(yellowPages, acceptedHash, yellowPagesParameter, parameter, progress)
                .ContinueWith(t => 
            {
                if (t.IsFaulted)
                    throw t.Exception;
                m_timer.BeginTimer(yellowPages, parameter.Name);
                return t.Result;
            });
        }

        public Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            return _service.UpdateAsync(yellowPages, yellowPagesParameter, parameter, progress);
        }

        public Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            return _service.StopAsync(yellowPages, yellowPagesParameter, name, id, progress)
                .ContinueWith(t=>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                m_timer.EndTimer();
            });
        }

        public Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            return _service.GetChannelsAsync();
        }

        public void Interrupt(IYellowPages yellowPages, InterruptedParameter parameter)
        {
            m_timer.EndTimer();

            foreach (var plugin in _plugins)
            {
                plugin.OnInterruptedAsync(parameter).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnAsyncExceptionThrown(t.Exception);
                });
            }

            m_timer.BeginTimer(yellowPages, parameter.Name);
        }

        private void OnAsyncExceptionThrown(Exception ex)
        {
            if (AsyncExceptionThrown != null)
                AsyncExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
