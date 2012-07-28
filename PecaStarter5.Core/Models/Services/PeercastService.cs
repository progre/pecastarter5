using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.ExternalYellowPages;
using System.Net;

namespace Progressive.PecaStarter5.Models.Services
{
    public class PeercastService
    {
        private Peercast peercast;
        private IEnumerable<IExternalYellowPages> externalYellowPagesList;
        private IEnumerable<IPlugin> plugins;

        public PeercastService(Peercast peercast, IEnumerable<IExternalYellowPages> externalYellowPagesList,
            IEnumerable<IPlugin> plugins)
        {
            this.peercast = peercast;
            this.externalYellowPagesList = externalYellowPagesList;
            this.plugins = plugins;
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
                    throw new ApplicationException("イエローページの規約が更新されています。規約を再確認してください。");
                }
            }

            // YP更新
            progress.Report("YPの設定を変更中...");
            await peercast.SetYellowPagesAsync(yellowPages.Host);

            // 開始
            progress.Report("チャンネルを作成中...");
            var param = (BroadcastParameter)parameter.Clone();
            param.Genre = yellowPages.GetPrefix(yellowPagesParameter) + param.Genre;
            var id = await peercast.BroadcastAsync(param);

            var broadcastedParameter = new BroadcastedParameter
            {
                Bitrate = -1, // TODO: ビットレート取得してない
                Id = id,
                YellowPagesParameters = yellowPagesParameter,
                BroadcastParameter = parameter
            };

            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                Exception exception = null;
                try
                {
                    await Find(externalYellowPagesList, yellowPages.Name).OnBroadcastedAsync(broadcastedParameter);
                }
                catch (Exception ex)
                {
                    exception = ex;
                    // catch内でawait出来ないので外出し
                }
                if (exception != null)
                {
                    await peercast.StopAsync(id);
                    throw exception;
                }
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(plugins.Select(x => x.OnBroadcastedAsync(broadcastedParameter)).ToArray());
            });

            //    // ログ出力
            //    if (viewModel.SettingsViewModel.Logging)
            //    {
            //        var esvm = viewModel.ExternalSourceViewModel;
            //        logger.Name = esvm.Name.Value;
            //        logger.StartAt = DateTime.Now;
            //        logger.insert("0", "0", esvm.Genre.Value, esvm.Description.Value, esvm.Comment.Value);
            //    }
            //    if (yp is WebApiYellowPages || viewModel.SettingsViewModel.Logging)
            //    {
            //        viewModel.BeginTimer();
            //    }

            progress.Report("チャンネルを作成しました");
            return id;
        }

        public async Task UpdateAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            UpdateParameter parameter, IProgress<string> progress)
        {
            // 更新
            progress.Report("通信中...");
            var param = (UpdateParameter)parameter.Clone();
            param.Genre = yellowPages.GetPrefix(yellowPagesParameter);
            await peercast.UpdateAsync(param);

            var updatedParameter = new UpdatedParameter
            {
                YellowPagesParameters = yellowPagesParameter,
                UpdateParameter = parameter
            };

            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(externalYellowPagesList, yellowPages.Name).OnUpdatedAsync(updatedParameter);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(plugins.Select(x => x.OnUpdatedAsync(updatedParameter)).ToArray());
            });

            progress.Report("チャンネル情報を更新しました");
        }

        public async Task StopAsync(IYellowPages yellowPages, Dictionary<string, string> yellowPagesParameter,
            string name, string id, IProgress<string> progress)
        {
            // 停止
            progress.Report("通信中...");
            await peercast.StopAsync(id);

            var stopedParameter = new StopedParameter
            {
                Name = name,
                Id = id,
                YellowPagesParameters = yellowPagesParameter
            };

            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(externalYellowPagesList, yellowPages.Name).OnStopedAsync(stopedParameter);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(plugins.Select(x => x.OnStopedAsync(stopedParameter)).ToArray());
            });
            progress.Report("チャンネルを切断しました");
        }

        public async Task TickAsync(IYellowPages yellowPages)
        {
            // 外部YPに通知
            if (yellowPages.IsExternal)
            {
                await Find(externalYellowPagesList, yellowPages.Name).OnTickedAsync("", -1, -1);
            }

            // プラグイン処理
            await Task.Factory.StartNew(() =>
            {
                Task.WaitAll(plugins.Select(x => x.OnTickedAsync("", -1, -1)).ToArray());
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
            return externalYellowPagesList.Single(x => x.Name == name);
        }
    }
}
