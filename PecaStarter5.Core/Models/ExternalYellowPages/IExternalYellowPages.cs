﻿using Progressive.PecaStarter5.Plugin;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Models.ExternalYellowPages
{
    public interface IExternalYellowPages
    {
        /// <summary>
        /// 配信開始時に通知されるメソッド
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        Task OnBroadcastedAsync(BroadcastingParameter parameter);
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
        Task OnStopedAsync(StoppedParameter parameter);
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

        string Name { get; }
    }
}
