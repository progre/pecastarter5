using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Commons;
using Progressive.Peercast4Net.Utils;
using Progressive.Peercast4Net.Commons.Utils;

namespace Progressive.Peercast4Net.Dao
{
    class PeercastDao : IDisposable
    {
        protected const string SettingsHtmlUrl = "/html/ja/settings.html";
        protected const string ApplyUrl = "/admin?cmd=apply";
        protected const string ViewXmlUrl = "/admin?cmd=viewxml";
        protected const string FetchCommand = "/admin?cmd=fetch&url={0}&name={1}&genre={2}&desc={3}&contact={4}&type={5}";
        protected const string SetMetaCommand = "/admin?cmd=setmeta&name={0}&genre={1}&desc={2}&url={3}&comment={4}&t_artist={5}&t_title={6}&t_album={7}&t_genre={8}&t_contact={9}";
        protected const string StopCommand = "/admin?cmd=stop&id={0}";
        protected const string KeepCommand = "/admin?cmd=keep&id={0}";

        protected string address;
        private WebClient client;

        public PeercastDao(string address)
        {
            client = new WebClient();
            this.address = address;
        }

        public virtual Task FetchAsync(string url, string name, string genre, string description, string contactUrl, string type)
        {
            return client.AccessAsync(
                "http://" + address + string.Format(FetchCommand,
                PeercastUtils.PercentEncode(url), PeercastUtils.PercentEncode(name),
                PeercastUtils.PercentEncode(genre), PeercastUtils.PercentEncode(description),
                PeercastUtils.PercentEncode(contactUrl), PeercastUtils.PercentEncode(type)));
        }

        public virtual Task SetMetaAsync(
            string name, string genre, string description, string url, string comment,
            string trackArtist, string trackTitle, string trackAlbum, string trackGenre, string trackContact)
        {
            return client.AccessAsync(
                "http://" + address + string.Format(SetMetaCommand,
                HttpUtils.ToRfc3986(name, Encoding.UTF8), PeercastUtils.PercentEncode(genre),
                PeercastUtils.PercentEncode(description), PeercastUtils.PercentEncode(url),
                PeercastUtils.PercentEncode(comment),
                PeercastUtils.PercentEncode(trackArtist), PeercastUtils.PercentEncode(trackTitle),
                PeercastUtils.PercentEncode(trackAlbum), PeercastUtils.PercentEncode(trackGenre),
                PeercastUtils.PercentEncode(trackContact)));
        }

        public virtual Task StopAsync(string id)
        {
            return client.AccessAsync(
                "http://" + address + string.Format(StopCommand, id));
        }

        public virtual Task KeepAsync(string id)
        {
            return client.AccessAsync(
                "http://" + address + string.Format(KeepCommand, id));
        }

        public virtual Task<string> GetViewXmlAsync()
        {
            return client.DownloadAsync(
                "http://" + address + ViewXmlUrl);
        }

        public virtual Task<string> GetSettingsHtmlAsync()
        {
            return client.DownloadAsync(
                "http://" + address + SettingsHtmlUrl);
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
            return client.AccessAsync("http://" + address + sb.ToString());
        }

        #region IDisposable メンバー

        public virtual void Dispose()
        {
            client.Dispose();
        }

        #endregion
    }
}
