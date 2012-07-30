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

        public async Task<string> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash,
            Dictionary<string, string> yellowPagesParameter,
            BroadcastParameter parameter, IProgress<string> progress)
        {
            // YPの更新確認
            if (yellowPages.IsCheckNoticeUrl && acceptedHash.HasValue)
            {
                progress.Report("規約の更新を確認中...");
                if (await IsUpdatedYellowPagesAsync(yellowPages, acceptedHash.Value))
                {
                    throw new Exception("イエローページの規約が更新されています。規約を再確認してください。");
                }
            }

            // YP更新
            progress.Report("YPの設定を変更中...");
            await m_peercast.SetYellowPagesAsync(yellowPages.Host);

            // 開始
            progress.Report("チャンネルを作成中...");
            var param = (BroadcastParameter)parameter.Clone();
            param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
            var tuple = await m_peercast.BroadcastAsync(param);

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
                    await Find(m_externalYellowPagesList, yellowPages.Name).OnBroadcastedAsync(broadcastedParameter);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    // catch内でawait出来ないので外出し
                }
                if (exception != null)
                {
                    await m_peercast.StopAsync(tuple.Item1);
                    throw exception;
                }
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(m_plugins.Select(x => x.OnBroadcastedAsync(broadcastedParameter)).ToArray());
            });

            // TODO: タイマー起動する

            progress.Report("チャンネルを作成しました");
            return tuple.Item1;
        }

        public async Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            // 更新
            progress.Report("通信中...");
            var param = (UpdateParameter)parameter.Clone();
            param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
            await m_peercast.UpdateAsync(param);

            var updatedParameter = new UpdatedParameter
            {
                YellowPagesParameters = yellowPagesParameter,
                UpdateParameter = parameter
            };

            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(m_externalYellowPagesList, yellowPages.Name).OnUpdatedAsync(updatedParameter);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(m_plugins.Select(x => x.OnUpdatedAsync(updatedParameter)).ToArray());
            });

            progress.Report("チャンネル情報を更新しました");
        }

        public async Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            // 停止
            progress.Report("通信中...");
            await m_peercast.StopAsync(id);

            var stopedParameter = new StopedParameter
            {
                Name = name,
                Id = id,
                YellowPagesParameters = yellowPagesParameter
            };

            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(m_externalYellowPagesList, yellowPages.Name).OnStopedAsync(stopedParameter);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(m_plugins.Select(x => x.OnStopedAsync(stopedParameter)).ToArray());
            });
            progress.Report("チャンネルを切断しました");
        }

        public async Task TickAsync(IYellowPages yellowPages)
        {
            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(m_externalYellowPagesList, yellowPages.Name).OnTickedAsync("", -1, -1);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(m_plugins.Select(x => x.OnTickedAsync("", -1, -1)).ToArray());
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
