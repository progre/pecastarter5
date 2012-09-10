using System.Threading.Tasks;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Plugin
{
    /// <summary>
    /// プラグイン
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 配信開始時に通知されるメソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnBroadcastedAsync(BroadcastedParameter parameter);
        /// <summary>
        /// 配信情報更新時に通知されるメソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnUpdatedAsync(UpdatedParameter parameter);
        /// <summary>
        /// 配信終了時に通知されるメソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnStopedAsync(StopedParameter parameter);
        /// <summary>
        /// 配信開始後10分毎に通知されるメソッド
        /// </summary>
        /// <param name="name"></param>
        /// <param name="relays"></param>
        /// <param name="listeners"></param>
        /// <returns></returns>
        Task OnTickedAsync(string name, int relays, int listeners);
        /// <summary>
        /// 配信中のチャンネル情報を取り込んだ時に通知されるメソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnInterruptedAsync(InterruptedParameter parameter);
    }
}
