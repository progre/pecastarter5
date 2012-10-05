using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.Plugins
{
    /// <summary>
    /// プラグイン
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// プラグイン情報
        /// </summary>
        PluginInfo PluginInfo { get; }
        /// <summary>
        /// 終了時にディスクに保存され、次回起動時に復元される
        /// </summary>
        Dictionary<string, object> Repository { get; set; }
        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();
        /// <summary>
        /// 設定ダイアログを開く
        /// </summary>
        void ShowSettingsDialog();
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
        /// <summary>
        /// 終了処理
        /// </summary>
        void Terminate();
    }

    /// <summary>
    /// プラグイン情報
    /// </summary>
    public struct PluginInfo
    {
        public PluginInfo(string name, string displayName, Version version, bool hasSettingsDialog)
        {
            this.name = name;
            this.displayName = displayName;
            this.version = version;
            this.hasSettingsDialog = hasSettingsDialog;
        }

        private string name;
        /// <summary>
        /// プラグイン名
        /// </summary>
        public string Name { get { return name; } }
        private string displayName;
        /// <summary>
        /// プラグインの表示名
        /// </summary>
        public string DisplayName { get { return displayName; } }
        private Version version;
        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public Version Version { get { return version; } }
        private bool hasSettingsDialog;
        /// <summary>
        /// 設定ダイアログがあるかどうか
        /// </summary>
        /// <returns></returns>
        public bool HasSettingsDialog { get { return hasSettingsDialog; } }
    }
}
