using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Progressive.PecaStarter5.Models.Plugins;
using System.Threading.Tasks;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models.Daos;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Models.YellowPagesXml;

namespace Progressive.PecaStarter5.Models
{
    // TODO: VMにあるロジック・エンティティを可能な限りここに移動
    public class PecaStarterModel
    {
        private IExternalResource m_externalResource;
        public Timer timer; // スレッドタイマが最も軽量

        public event UnhandledExceptionEventHandler ExceptionThrown;

        public PecaStarterModel(string title, IExternalResource externalResource)
        {
            m_externalResource = externalResource;
            Title = title;
            Peercast = new Peercast();
            LoggerPlugin = new LoggerPlugin();
            Plugins = new IPlugin[] { LoggerPlugin };
            var dao = new ConfigurationDao(externalResource);

            Configuration = dao.Get();
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
