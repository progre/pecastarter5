using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.Peercast4Net;
using Progressive.Peercast4Net.Datas;
using Progressive.PecaStarter5.Plugin;

namespace Progressive.PecaStarter5.Models.Broadcasts
{
    public class PeercastService
    {
        private readonly Peercast m_peercast = new Peercast();
        private readonly PeercastStation m_peercastStation = new PeercastStation();
        private readonly IEnumerable<IExternalYellowPages> m_externalYellowPagesList;
        private readonly IEnumerable<IPlugin> m_plugins;
        private readonly Configuration m_configuration;

        public PeercastService(IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IEnumerable<Progressive.PecaStarter5.Plugin.IPlugin> plugins, Configuration configuration)
        {
            m_externalYellowPagesList = externalYellowPagesList;
            m_plugins = plugins;
            m_configuration = configuration;
        }

        private IPeercast Peercast
        {
            get
            {
                IPeercast peca;
                if (m_configuration.PeercastType == PeercastType.Peercast)
                    peca = m_peercast;
                else
                    peca = m_peercastStation;
                peca.Address = "localhost:" + m_configuration.Port;
                return peca;
            }
        }

        public Task<string> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // YPの更新確認
                if (yellowPages.IsCheckNoticeUrl && acceptedHash.HasValue)
                {
                    progress.Report("規約の更新を確認中...");
                    if (IsUpdatedYellowPagesAsync(yellowPages, acceptedHash.Value).Result)
                    {
                        throw new YellowPagesException("イエローページの規約が更新されています。規約を再確認してください。");
                    }
                }

                // 開始
                progress.Report("チャンネルを作成中...");
                var param = (BroadcastParameter)parameter.Clone();
                param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
                var tuple = Peercast.BroadcastAsync(
                    new Peercast4Net.Datas.YellowPages() { Name = yellowPages.Name, Url = yellowPages.Host },
                    param).Result;

                var broadcastedParameter = new Progressive.PecaStarter5.Plugin.BroadcastedParameter
                {
                    Bitrate = tuple.Item2,
                    Id = tuple.Item1,
                    YellowPagesParameters = yellowPagesParameter,
                    BroadcastParameter = parameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Exception exception = null;
                    try
                    {
                        Find(m_externalYellowPagesList, yellowPages.Name)
                            .OnBroadcastedAsync(broadcastedParameter).Wait();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                        // catch内でawait出来ないので外出し
                    }
                    if (exception != null)
                    {
                        Peercast.StopAsync(tuple.Item1).Wait();
                        throw exception;
                    }
                }

                // プラグイン処理
                Task.WaitAll(m_plugins.Select(x => x.OnBroadcastedAsync(broadcastedParameter)).ToArray());

                progress.Report("チャンネルを作成しました");
                return tuple.Item1;
            });
        }

        public Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // 更新
                progress.Report("通信中...");
                var param = (UpdateParameter)parameter.Clone();
                param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
                Peercast.UpdateAsync(param).Wait();

                var updatedParameter = new UpdatedParameter
                {
                    YellowPagesParameters = yellowPagesParameter,
                    UpdateParameter = parameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Find(m_externalYellowPagesList, yellowPages.Name).OnUpdatedAsync(updatedParameter).Wait();
                }

                // プラグイン処理
                Task.WaitAll(m_plugins.Select(x => x.OnUpdatedAsync(updatedParameter)).ToArray());

                progress.Report("チャンネル情報を更新しました");
            });
        }

        public Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            return Task.Factory.StartNew(() =>
            {
                // 停止
                progress.Report("通信中...");
                Peercast.StopAsync(id).Wait();

                var stopedParameter = new StopedParameter
                {
                    Name = name,
                    Id = id,
                    YellowPagesParameters = yellowPagesParameter
                };

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    Find(m_externalYellowPagesList, yellowPages.Name).OnStopedAsync(stopedParameter).Wait();
                }

                // プラグイン処理
                Task.WaitAll(m_plugins.Select(x => x.OnStopedAsync(stopedParameter)).ToArray());
                progress.Report("チャンネルを切断しました");
            });
        }

        public Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            return Peercast.GetChannelsAsync();
        }

        public Task OnTickedAsync(IYellowPages yellowPages, string name)
        {
            return Task.Factory.StartNew(() =>
            {
                var tuple = Peercast.GetListenersAsync(name).Result;

                // 外部YPに通知
                if (yellowPages.IsExternal)
                {
                    ((IExternalYellowPages)yellowPages).OnTickedAsync(name, tuple.Item1, tuple.Item2).Wait();
                }

                Task.WaitAll(m_plugins.Select(x => x.OnTickedAsync(name, tuple.Item1, tuple.Item2)).ToArray());
            });
        }

        private async Task<bool> IsUpdatedYellowPagesAsync(IYellowPages yellowPages, int acceptedHash)
        {
            if (acceptedHash != await yellowPages.GetNoticeHashAsync())
            {
                return true;
            }
            return false;
        }

        private IExternalYellowPages Find(IEnumerable<IExternalYellowPages> list, string name)
        {
            return m_externalYellowPagesList.Single(x => x.Name == name);
        }
    }
}
