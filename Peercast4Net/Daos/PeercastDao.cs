using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Commons;
using Progressive.Peercast4Net.Commons.Utils;
using Progressive.Peercast4Net.Utils;

namespace Progressive.Peercast4Net.Daos
{
    class PeercastDao : PeercastDaoBase
    {
        protected const string SettingsHtmlUrl = "/html/ja/settings.html";
        protected const string ApplyUrl = "/admin?cmd=apply";
        protected const string FetchCommand = "/admin?cmd=fetch&url={0}&name={1}&genre={2}&desc={3}&contact={4}&type={5}";
        protected const string SetMetaCommand = "/admin?cmd=setmeta&name={0}&genre={1}&desc={2}&url={3}&comment={4}&t_artist={5}&t_title={6}&t_album={7}&t_genre={8}&t_contact={9}";
        protected const string StopCommand = "/admin?cmd=stop&id={0}";
        protected const string KeepCommand = "/admin?cmd=keep&id={0}";

        public PeercastDao(string address)
            :base(address)
        {
        }

        public virtual Task FetchAsync(string url, string name, string genre, string description, string contactUrl, string type)
        {
            return AccessAsync(string.Format(FetchCommand,
                PeercastUtils.PercentEncode(url), PeercastUtils.PercentEncode(name),
                PeercastUtils.PercentEncode(genre), PeercastUtils.PercentEncode(description),
                PeercastUtils.PercentEncode(contactUrl), PeercastUtils.PercentEncode(type)));
        }

        public virtual Task SetMetaAsync(
            string name, string genre, string description, string url, string comment,
            string trackArtist, string trackTitle, string trackAlbum, string trackGenre, string trackContact)
        {
            return AccessAsync(string.Format(SetMetaCommand,
                HttpUtils.ToRfc3986(name, Encoding.UTF8), PeercastUtils.PercentEncode(genre),
                PeercastUtils.PercentEncode(description), PeercastUtils.PercentEncode(url),
                PeercastUtils.PercentEncode(comment),
                PeercastUtils.PercentEncode(trackArtist), PeercastUtils.PercentEncode(trackTitle),
                PeercastUtils.PercentEncode(trackAlbum), PeercastUtils.PercentEncode(trackGenre),
                PeercastUtils.PercentEncode(trackContact)));
        }

        public virtual Task StopAsync(string id)
        {
            return AccessAsync(string.Format(StopCommand, id));
        }

        public virtual Task KeepAsync(string id)
        {
            return AccessAsync(string.Format(KeepCommand, id));
        }

        public virtual Task<string> GetSettingsHtmlAsync()
        {
            return DownloadAsync(SettingsHtmlUrl);
        }

        public virtual Task ApplyAsync(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var sb = new StringBuilder(ApplyUrl);
            foreach (var item in parameters)
            {
                sb.Append('&').Append(
                    PeercastUtils.PercentEncode(item.Key)).Append('=')
                    .Append(PeercastUtils.PercentEncode(item.Value));
            }
            return AccessAsync(sb.ToString());
        }

        private async Task AccessAsync(string url)
        {
            try
            {
                await client.AccessAsync("http://" + address + url);
            }
            catch (WebException)
            {
                throw new PeercastException("Peercastへの接続に失敗しました。" + Environment.NewLine
                    + "Peercastが起動しているか、またはポート番号が正しいか確認してください。");
            }
        }
    }
}
