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

        public Task BroadcastAsync()
        {
        //    // YPの更新確認
        //    if (yp.CanGetNoticeHash())
        //    {
        //        viewModel.Feedback = "規約の更新を確認中...";
        //        if (ypvm.AcceptedHash != await yp.GetNoticeHashAsync())
        //        {
        //            ypvm.IsAccepted = false;
        //            viewModel.Feedback = "中止";
        //            viewModel.NotifyAlert("イエローページの規約が更新されています。規約を再確認してください。");
        //            return;
        //        }
        //    }

        //    // 開始

        //    // ログ出力

        //    if (yp is PeercastYellowPages)
        //    {
        //        var result = await PeercastYpExecute(yp as PeercastYellowPages, ypvm, viewModel.ExternalSourceViewModel);
        //        if (result != Result.Success)
        //        {
        //            viewModel.Feedback = "中止";
        //            viewModel.NotifyAlert("チャンネルの作成時にエラーが発生しました。\n原因: " + GetErrorMessage(result));
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        var info = await WebApiYpExecute(yp as WebApiYellowPages, ypvm, viewModel.ExternalSourceViewModel);
        //        if (info.Result != Result.Success)
        //        {
        //            viewModel.Feedback = "中止";
        //            viewModel.NotifyAlert("チャンネルの作成時にエラーが発生しました。\n原因: " + GetErrorMessage(info));
        //            return;
        //        }
        //    }
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
            return null;
        }

        //private async Task<Result> PeercastYpExecute(PeercastYellowPages pyp, YellowPagesViewModel yp, ExternalSourceViewModel es)
        //{
        //    return (await peercast.Broadcast(pyp.Host, es.StreamUrl, es.Name.Value, yp.Prefix + es.Genre.Value, es.Description.Value, "WMV", es.ContactUrl.Value, es.Comment.Value, "", "", "", "", "")).Result;
        //}

        //private async Task<ResultInfo<string>> WebApiYpExecute(WebApiYellowPages wyp, YellowPagesViewModel ypvm, ExternalSourceViewModel esvm)
        //{
        //    var info = await peercast.Broadcast("", esvm.StreamUrl, esvm.Name.Value, esvm.Genre.Value, esvm.Description.Value, "WMV", esvm.ContactUrl.Value, esvm.Comment.Value, "", "", "", "", "");
        //    if (info.Result != Result.Success)
        //    {
        //        return ResultInfo.Create(info.Result, "");
        //    }
        //    var wypInfo = await wyp.Broadcast(GetWebApiParameters(wyp.BroadcastParameters, info.Value.Item1, ypvm, esvm, "http://localhost:7144/pls/" + info.Value.Item2));
        //    if (wypInfo.Result != Result.Success)
        //    {
        //        await peercast.Stop();
        //        return wypInfo;
        //    }
        //    return wypInfo;
        //}
    }
}
