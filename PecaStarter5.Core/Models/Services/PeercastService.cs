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

        public async Task BroadcastAsync(IYellowPages yellowPages, int acceptedHash, BroadcastParameter parameter,
            IProgress<string> progress)
        {
            if (yellowPages.IsCheckNoticeUrl)
            {
                // YPの更新確認
                progress.Report("規約の更新を確認中...");
                if (await IsUpdatedYellowPagesAsync(yellowPages, acceptedHash))
                {
                    throw new ApplicationException("イエローページの規約が更新されています。規約を再確認してください。");
                }
                // YP更新
                if (yellowPages.Host != await peercast.GetYellowPagesAsync())
                {
                    await peercast.SetYellowPagesAsync(yellowPages.Host);
                }
                // 開始
                await peercast.BroadcastAsync(parameter);
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
                progress.Report("チャンネルを作成しました");
            }

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
            //    viewModel.Feedback = "チャンネルを作成しました";
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
