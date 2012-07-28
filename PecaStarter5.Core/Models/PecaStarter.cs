using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Progressive.PecaStarter5.Models.Plugins;
using System.Threading.Tasks;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models
{
    // TODO: VMにあるロジック・エンティティを可能な限りここに移動
    public class PecaStarter
    {
        public Timer timer; // スレッドタイマが最も軽量

        public event UnhandledExceptionEventHandler ExceptionThrown;

        public PecaStarter()
        {
            Peercast = new Peercast();
            LoggerPlugin = new LoggerPlugin();
            Plugins = new IPlugin[] { LoggerPlugin };
        }

        public Peercast Peercast { get; private set; }
        public IEnumerable<IPlugin> Plugins { get; private set; }
        public LoggerPlugin LoggerPlugin { get; private set; }

        public void Broadcast(BroadcastParameter parameter)
        {
            BeginTimer(parameter.Name);
        }

        public void Interrupt(InterruptedParameter parameter)
        {
            foreach (var plugin in Plugins)
            {
                plugin.OnInterruptedAsync(parameter).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnExceptionThrown(t.Exception);
                });
            }

            BeginTimer(parameter.Name);
        }

        private void BeginTimer(string name)
        {
            const int period = 10 * 60 * 1000;
            timer = new Timer(s => OnTIckedAsync((string)s).Wait(), name, period, period);
        }

        public void EndTimer()
        {
            if (timer == null)
                return;
            timer.Dispose();
            timer = null;
        }

        private async Task OnTIckedAsync(string name)
        {
            try
            {
                var tuple = await Peercast.GetListenersAsync(name);
                Task.WaitAll(Plugins.Select(x => x.OnTickedAsync(name, tuple.Item1, tuple.Item2)).ToArray());
            }
            catch (Exception ex)
            {
                OnExceptionThrown(ex);
            }
        }

        private void OnExceptionThrown(Exception ex)
        {
            if (ExceptionThrown != null)
                ExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
