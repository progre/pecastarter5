using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.Daos;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models.YellowPagesXml;
using Progressive.Peercast4Net;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models
{
    // TODO: VMにあるロジック・エンティティを可能な限りここに移動
    public class PecaStarterModel
    {
        private readonly IExternalResource m_externalResource;
        private readonly BroadcastTimer m_timer;
        private readonly List<IExternalYellowPages> m_externalYellowPagesList;
        private readonly IEnumerable<IPlugin> m_plugins;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public PecaStarterModel(string title, IExternalResource externalResource)
        {
            m_externalResource = externalResource;
            m_timer = new BroadcastTimer();
            m_timer.Ticked += s =>
            {
                var tuple1 = (Tuple<IYellowPages, string>)s;
                OnTickedAsync(tuple1.Item1, tuple1.Item2);
            };
            LoggerPlugin = new LoggerPlugin();
            m_plugins = new IPlugin[] { LoggerPlugin };

            Title = title;
            Peercast = new Peercast();
            Configuration = new ConfigurationDao(externalResource).Get();
            Configuration.DefaultLogPath = externalResource.DefaultLogPath;
            Configuration.PropertyChanged += (sender, e) =>
            {
                var config = (Configuration)sender;
                if (e.PropertyName == "Port")
                {
                    Peercast.Address = "localhost:" + config.Port;
                }
            };
            // Configを反映
            Peercast.Address = "localhost:" + Configuration.Port;

            var tuple = GetYellowPagesLists();
            YellowPagesList = tuple.Item1;
            m_externalYellowPagesList = tuple.Item2;
            Service = new PeercastService(Peercast, m_externalYellowPagesList, m_plugins);
        }

        public string Title { get; private set; }
        public PeercastService Service { get; private set; }
        public Peercast Peercast { get; private set; }
        public LoggerPlugin LoggerPlugin { get; private set; }
        public Configuration Configuration { get; private set; }
        public List<IYellowPages> YellowPagesList { get; private set; }

        public void Save()
        {
            new ConfigurationDao(m_externalResource).Put(Configuration);
        }

        public void Broadcast(IYellowPages yellowPages, BroadcastParameter parameter)
        {
            m_timer.BeginTimer(yellowPages, parameter.Name);
        }

        public void Interrupt(IYellowPages yellowPages, InterruptedParameter parameter)
        {
            m_timer.EndTimer();

            foreach (var plugin in m_plugins)
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

        private Tuple<List<IYellowPages>, List<IExternalYellowPages>> GetYellowPagesLists()
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

        private async Task OnTickedAsync(IYellowPages yellowPages, string name)
        {
            try
            {
                var tuple = await Peercast.GetListenersAsync(name);

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    await ((IExternalYellowPages)yellowPages).OnTickedAsync(name, tuple.Item1, tuple.Item2);
                }

                Task.WaitAll(m_plugins.Select(x => x.OnTickedAsync(name, tuple.Item1, tuple.Item2)).ToArray());
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
