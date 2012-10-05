using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.Configurations;
using Progressive.PecaStarter5.Models.YellowPages;
using Progressive.PecaStarter5.Plugins;
using Progressive.Peercast4Net;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    public class BroadcastModel
    {
        private readonly PeercastService _service = new PeercastService();
        private readonly Peercast peercast = new Peercast();
        private readonly PeercastStation peercastStation = new PeercastStation();
        private readonly BroadcastTimer timer = new BroadcastTimer();
        private readonly Configuration configuration;
        private readonly IEnumerable<IExternalYellowPages> externalYellowPagesList;
        private readonly IEnumerable<ExternalPlugin> plugins;

        /// <summary>非同期にエラーが発生した場合に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        public BroadcastModel(Configuration configuration,
            IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IEnumerable<ExternalPlugin> plugins)
        {
            this.configuration = configuration;
            this.externalYellowPagesList = externalYellowPagesList;
            this.plugins = plugins;
            timer.Ticked += timer_Ticked;
        }

        private IPeercast Peercast
        {
            get
            {
                IPeercast peca;
                if (configuration.PeercastType == PeercastType.Peercast)
                    peca = peercast;
                else
                    peca = peercastStation;
                peca.Address = "localhost:" + configuration.Port;
                return peca;
            }
        }

        public Task<BroadcastingParameter> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            return _service.BroadcastAsync(Peercast, externalYellowPagesList,
                yellowPages, acceptedHash, yellowPagesParameter, parameter, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                timer.BeginTimer(yellowPages, parameter.Name);
                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnBroadcastedAsync(t.Result);
                return t.Result;
            });
        }

        public Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            return _service.UpdateAsync(Peercast, externalYellowPagesList,
                yellowPages, yellowPagesParameter, parameter, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnUpdatedAsync(t.Result);
            });
        }

        public Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            return _service.StopAsync(Peercast, externalYellowPagesList,
                yellowPages, yellowPagesParameter, name, id, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnStopedAsync(t.Result);
                timer.EndTimer();
            });
        }

        public Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            return _service.GetChannelsAsync(Peercast);
        }

        public void Interrupt(IYellowPages yellowPages, InterruptedParameter parameter)
        {
            timer.EndTimer();

            foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
            {
                plugin.OnInterruptedAsync(parameter).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        OnAsyncExceptionThrown(t.Exception);
                });
            }

            timer.BeginTimer(yellowPages, parameter.Name);
        }

        private void timer_Ticked(object state)
        {
            var tuple1 = (Tuple<IYellowPages, string>)state;
            var yellowPages = tuple1.Item1;
            var name = tuple1.Item2;
            _service.OnTickedAsync(Peercast, yellowPages, name).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    OnAsyncExceptionThrown(t.Exception);

                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnTickedAsync(name, t.Result.Item1, t.Result.Item2)
                        .ContinueWith(
                            t1 => OnAsyncExceptionThrown(t1.Exception),
                            TaskContinuationOptions.OnlyOnFaulted);
            });
        }

        private void OnAsyncExceptionThrown(Exception ex)
        {
            if (AsyncExceptionThrown != null)
                AsyncExceptionThrown(this, new UnhandledExceptionEventArgs(ex, false));
        }
    }
}
