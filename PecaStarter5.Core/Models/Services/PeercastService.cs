using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.Services
{
    public class PeercastService
    {
        private Peercast peercast;

        public PeercastService(Peercast peercast)
        {
            this.peercast = peercast;
        }

        public async Task<string> BroadcastAsync(IYellowPages yellowPages, int? acceptedHash, BroadcastParameter parameter,
            IProgress<string> progress)
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
            if (yellowPages.Host != await peercast.GetYellowPagesAsync())
            {
                await peercast.SetYellowPagesAsync(yellowPages.Host);
            }
            // 開始
            string id = await peercast.BroadcastAsync(parameter);
            try
            {
                yellowPages.OnBroadcastAsync();
            }
            catch (Exception ex)
            {
                //peercast.StopAsync();
                throw ex;
            }
            // ログ出力など
            // logger.OnBroadcast();

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
