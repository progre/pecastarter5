using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.Services
{
    public class PeercastService
    {
        private Peercast m_peercast;
        private IEnumerable<IExternalYellowPages> m_externalYellowPagesList;
        private IEnumerable<IPlugin> m_plugins;

        public PeercastService(Peercast peercast, IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IEnumerable<IPlugin> plugins)
        {
            this.m_peercast = peercast;
            this.m_externalYellowPagesList = externalYellowPagesList;
            this.m_plugins = plugins;
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
                        throw new Exception("イエローページの規約が更新されています。規約を再確認してください。");
                    }
                }

                // YP更新
                progress.Report("YPの設定を調整中...");
                m_peercast.SetYellowPagesAsync(yellowPages.Host).Wait();

                // 開始
                progress.Report("チャンネルを作成中...");
                var param = (BroadcastParameter)parameter.Clone();
                param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
                var tuple = m_peercast.BroadcastAsync(param).Result;

                var broadcastedParameter = new BroadcastedParameter
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
                        m_peercast.StopAsync(tuple.Item1).Wait();
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
                m_peercast.UpdateAsync(param).Wait();

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
                m_peercast.StopAsync(id).Wait();

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
