using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.Daos;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models
{
    // TODO: VMにあるロジック・エンティティを可能な限りここに移動
    public class PecaStarterModel
    {
        private IExternalResource m_externalResource;
        private BroadcastTimer m_timer;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public PecaStarterModel(string title, IExternalResource externalResource)
        {
            m_externalResource = externalResource;
            m_timer = new BroadcastTimer();
            m_timer.Ticked += s => OnTickedAsync((string)s);
            Title = title;
            Peercast = new Peercast();
            LoggerPlugin = new LoggerPlugin();
            Plugins = new IPlugin[] { LoggerPlugin };
            Configuration = new ConfigurationDao(externalResource).Get();
        }

        public string Title { get; private set; }
        public Peercast Peercast { get; private set; }
        public IEnumerable<IPlugin> Plugins { get; private set; }
        public LoggerPlugin LoggerPlugin { get; private set; }
        public Configuration Configuration { get; private set; }

        public void Save()
        {
            new ConfigurationDao(m_externalResource).Put(Configuration);
        }

        public Tuple<List<IYellowPages>, List<IExternalYellowPages>> GetYellowPagesLists()
        {
            var yellowPagesList = new List<IYellowPages>();
            var externalYellowPagesList = new List<IExternalYellowPages>();
            foreach (var xml in m_externalResource.GetYellowPagesDefineInputStream())
            {
                var yp = YellowPagesParserFactory.GetInstance(xml).GetInstance();
                yellowPagesList.Add(yp);
                if (yp.IsExternal)
                {
                    externalYellowPagesList.Add((IExternalYellowPages)yp);
                }
            }
            return Tuple.Create(yellowPagesList, externalYellowPagesList);
        }

        public void Broadcast(BroadcastParameter parameter)
        {
            m_timer.BeginTimer(parameter.Name);
        }

        public void Interrupt(InterruptedParameter parameter)
        {
            m_timer.EndTimer();

            foreach (var plugin in Plugins)
            {
                plugin.OnInterruptedAsync(parameter).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnAsyncExceptionThrown(t.Exception);
                });
            }

            m_timer.BeginTimer(parameter.Name);
        }

        public void Stop()
        {
            m_timer.EndTimer();
        }

        private async Task OnTickedAsync(string name)
        {
            try
            {
                var tuple = await Peercast.GetListenersAsync(name);
                Task.WaitAll(Plugins.Select(x => x.OnTickedAsync(name, tuple.Item1, tuple.Item2)).ToArray());
            }
            catch (Exception ex)
            {
                OnAsyncExceptionThrown(ex);
            }
        }

        private void OnAsyncExceptionThrown(Exception ex)
        {
            if (AsyncExceptionThrown != null)
                AsyncExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
