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
        private readonly Peercast m_peercast;
        private readonly PeercastStation m_peercastStation;
        private readonly IExternalResource m_externalResource;
        private readonly BroadcastTimer m_timer;
        private readonly List<IExternalYellowPages> m_externalYellowPagesList;
        private readonly IEnumerable<IPlugin> m_plugins;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public PecaStarterModel(string title, IExternalResource externalResource)
        {
            m_peercast = new Peercast();
            m_peercastStation = new PeercastStation();
            m_externalResource = externalResource;
            m_plugins = new IPlugin[] { LoggerPlugin };
            Title = title;
            LoggerPlugin = new LoggerPlugin();
            var tuple = GetYellowPagesLists();
            m_externalYellowPagesList = tuple.Item2;
            YellowPagesList = tuple.Item1;

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

            Configuration = new ConfigurationDao(externalResource).Get();
            Configuration.DefaultLogPath = externalResource.DefaultLogPath;
            Service = new PeercastService(m_externalYellowPagesList, m_plugins, Configuration);
        }

        public string Title { get; private set; }
        public PeercastService Service { get; private set; }
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

        private void OnAsyncExceptionThrown(Exception ex)
        {
            if (AsyncExceptionThrown != null)
                AsyncExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
