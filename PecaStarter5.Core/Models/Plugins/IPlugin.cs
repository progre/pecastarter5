using System.Threading.Tasks;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.Plugins
{
    /// <summary>
    /// 配信開始時
    /// 配信情報更新時
    /// 配信開始後10分毎
    /// 配信終了時
    /// 配信オープン時？（ログ再開メッセージ）
    /// いずれもエラーが発生しても配信には影響しないものとする
    /// </summary>
    public interface IPlugin
    {
        Task OnBroadcastedAsync(BroadcastedParameter parameter);
        Task OnUpdatedAsync(UpdatedParameter parameter);
        Task OnStopedAsync(StopedParameter parameter);
        Task OnTickedAsync(string name, int relays, int listeners);
        Task OnInterruptedAsync(InterruptedParameter parameter);
    }
}
