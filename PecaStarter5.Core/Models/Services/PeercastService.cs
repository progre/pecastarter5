using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progressive.Peercast4Net;
using Progressive.PecaStarter5.Models.Plugins;
using Progressive.PecaStarter5.Models.ExternalYellowPages;

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
            BroadcastParameter parameter, IProgress<string> progress)
        {
            if (yellowPages.IsCheckNoticeUrl)
            {
                // YPの更新確認
                progress.Report("規約の更新を確認中...");
                if (yellowPages.IsCheckNoticeUrl
                    && acceptedHash.HasValue
                    && await IsUpdatedYellowPagesAsync(yellowPages, acceptedHash.Value))
                {
                    throw new ApplicationException("イエローページの規約が更新されています。規約を再確認してください。");
                }
            }
            // YP更新
            progress.Report("YPの設定を変更中...");
            await peercast.SetYellowPagesAsync(yellowPages.Host);
            // 開始
            progress.Report("チャンネルを作成中...");
            string id = await peercast.BroadcastAsync(parameter);
            var broadcastedParameter = new BroadcastedParameter
            {
                Bitrate = -1,
                Id = id,
                YellowPagesParameters = Enumerable.Empty<KeyValuePair<string, string>>(),
                BroadcastParameter = parameter
            };
            if (yellowPages.IsExternal)
            {
                await externalYellowPagesList.Single(x => x.Name == yellowPages.Name)
                    .OnBroadcastedAsync(broadcastedParameter);
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

        public async Task UpdateAsync(UpdateParameter parameter, IProgress<string> progress)
        {
            progress.Report("通信中...");
            await peercast.UpdateAsync(parameter);
            progress.Report("チャンネル情報を更新しました");
        }

        public async Task StopAsync(string id, IProgress<string> progress)
        {
            progress.Report("通信中...");
            await peercast.StopAsync(id);
            progress.Report("チャンネルを切断しました");
        }

        private async Task<bool> IsUpdatedYellowPagesAsync(IYellowPages yellowPages, int acceptedHash)
        {
            if (acceptedHash != await yellowPages.GetNoticeHashAsync())
            {
                return true;
            }
            return false;
        }
    }
}
