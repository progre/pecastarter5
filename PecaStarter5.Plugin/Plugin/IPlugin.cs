using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Progressive.PecaStarter5.Plugin
{
    /// <summary>
    /// プラグイン
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// プラグインの表示名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// 設定ダイアログがあるかどうか
        /// </summary>
        /// <returns></returns>
        bool HasSettingsDialog { get; }
        /// <summary>
        /// 設定ダイアログを開く
        /// </summary>
        void ShowSettingsDialog();
        /// <summary>
        /// 終了時にディスクに保存され、次回起動時に復元される
        /// </summary>
        IDictionary<string, object> Repository { get; set; }
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

    public interface IPluginCommands
    {
    }
}
