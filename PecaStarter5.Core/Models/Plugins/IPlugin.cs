using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    interface IPlugin
    {
    }
}
