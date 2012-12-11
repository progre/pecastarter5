using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.Configurations;
using Progressive.PecaStarter5.Models.Dxos;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.YellowPages;
using Progressive.Peercast4Net;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    public class BroadcastModel
    {
        private readonly PeercastService service = new PeercastService();
        private readonly Peercast peercast = new Peercast();
        private readonly PeercastStation peercastStation = new PeercastStation();
        private readonly BroadcastTimer timer = new BroadcastTimer();
        private readonly Configuration configuration;
        private readonly IEnumerable<IExternalYellowPages> externalYellowPagesList;
        private readonly IEnumerable<ExternalPlugin> plugins;

        public IChannel Channel { get; private set; }

        /// <summary>非同期にエラーが発生した時に通知されるイベント</summary>
        public event UnhandledExceptionEventHandler AsyncExceptionThrown;

        /// <summary>チャンネル情報が更新された時に通知されるイベント</summary>
        public event EventHandler<ChannelStatusChangedEventArgs> ChannelStatusChanged = (s, e) => { };

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

        public Task<Progressive.PecaStarter5.Plugins.BroadcastingParameter> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            return service.BroadcastAsync(Peercast, externalYellowPagesList,
                yellowPages, acceptedHash, yellowPagesParameter, parameter, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                var broadcasting = t.Result;
                Channel = ChannelDxo.ToChannel(broadcasting);
                ChannelStatusChanged(this,
                    new ChannelStatusChangedEventArgs(Channel));
                timer.BeginTimer(yellowPages, broadcasting.Id);
                // プラグイン処理
                foreach (var plugin in plugins
                    .Where(x => x.IsEnabled).Select(x => x.Instance))
                {
                    plugin.OnBroadcastedAsync(broadcasting).ContinueWith(t1 =>
                        OnAsyncExceptionThrown(t1.Exception),
                        TaskContinuationOptions.OnlyOnFaulted);
                }
                return broadcasting;
            });
        }

        public Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            return service.UpdateAsync(Peercast, externalYellowPagesList,
                yellowPages, yellowPagesParameter, parameter, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                Channel = ChannelDxo.ToChannel(t.Result);
                ChannelStatusChanged(this,
                    new ChannelStatusChangedEventArgs(Channel));
                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnUpdatedAsync(t.Result);
            });
        }

        public Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            return service.StopAsync(Peercast, externalYellowPagesList,
                yellowPages, yellowPagesParameter, name, id, progress)
                .ContinueWith(t =>
            {
                if (t.IsFaulted)
                    throw t.Exception;
                Channel = null;
                ChannelStatusChanged(this,
                    new ChannelStatusChangedEventArgs(Channel));
                // プラグイン処理
                foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
                    plugin.OnStopedAsync(t.Result);
                timer.EndTimer();
            });
        }

        public Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            return service.GetChannelsAsync(Peercast);
        }

        public void Interrupt(IYellowPages yellowPages, Progressive.PecaStarter5.Plugins.InterruptedParameter parameter)
        {
            timer.EndTimer();

            foreach (var plugin in plugins.Where(x => x.IsEnabled).Select(x => x.Instance))
            {
                try
                {
                    var task = plugin.OnInterruptedAsync(parameter);
                    if (task == null)
                        continue;
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            OnAsyncExceptionThrown(t.Exception);
                    });
                }
                catch (Exception ex)
                {
                    Task.Factory.StartNew(() => OnAsyncExceptionThrown(ex));
                }
            }

            timer.BeginTimer(yellowPages, parameter.Id);
        }

        private void timer_Ticked(long count, IYellowPages yellowPages, string id)
        {
            try
            {
                IChannel channel = null;
                // 10分おき
                if (count % 10 == 0)
                {
                    channel = service.OnTickedAsync(Peercast, yellowPages, id).Result;
                }

                // プラグイン処理
                foreach (var plugin in plugins
                    .Where(x => x.IsEnabled).Select(x => x.Instance)
                    .Where(x =>
                    {
                        var tickInterval = x.PluginConfiguration.TickInterval;
                        return tickInterval > 0 && count % tickInterval == 0;
                    }))
                {
                    if (channel == null)
                    {
                        channel = peercast.GetChannelAsync(id).Result;
                    }
                    var task = plugin.OnTickedAsync(channel);
                    if (task == null)
                        continue;
                    task.Wait();
                }
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

    public class ChannelStatusChangedEventArgs : EventArgs
    {
        private readonly IChannel channel;
        public ChannelStatusChangedEventArgs(IChannel channel)
        {
            this.channel = channel;
        }
        public IChannel Channel { get { return channel; } }
    }
}
