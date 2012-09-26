using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    public class BroadcastModel
    {
        private readonly BroadcastTimer m_timer;
        private readonly IEnumerable<IPlugin> _plugins;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public BroadcastModel(IEnumerable<IPlugin> plugins)
        {
            _plugins = plugins;
            m_timer = new BroadcastTimer();
            m_timer.Ticked += s =>
            {
                var tuple1 = (Tuple<IYellowPages, string>)s;
                Service.OnTickedAsync(tuple1.Item1, tuple1.Item2).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnAsyncExceptionThrown(t.Exception);
                });
            };
        }

        private PeercastService Service { get; set; }

        public void Broadcast(IYellowPages yellowPages, Progressive.Peercast4Net.Datas.BroadcastParameter parameter)
        {
            m_timer.BeginTimer(yellowPages, parameter.Name);
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

        public void Stop()
        {
            m_timer.EndTimer();
        }

        private void OnAsyncExceptionThrown(Exception ex)
        {
            if (AsyncExceptionThrown != null)
                AsyncExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
