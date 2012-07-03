using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Utils;

namespace Progressive.Peercast4Net.Dao
{
    class TestingPeercastDao : PeercastDao
    {
        public TestingPeercastDao(string address)
            : base(address)
        {
        }

        public override Task FetchAsync(string url, string name, string genre, string description, string contactUrl, string type)
        {
            return Task.Factory.StartNew(() =>
                Console.WriteLine(
                "http://" + address + string.Format(FetchCommand,
                PeercastUtils.PercentEncode(url), PeercastUtils.PercentEncode(name),
                PeercastUtils.PercentEncode(genre), PeercastUtils.PercentEncode(description),
                PeercastUtils.PercentEncode(contactUrl), PeercastUtils.PercentEncode(type))));
        }

        public override Task SetMetaAsync(
            string name, string genre, string description, string url, string comment,
            string trackArtist, string trackTitle, string trackAlbum, string trackGenre, string trackContact)
        {
            return Task.Factory.StartNew(() =>
                Console.WriteLine(
                "http://" + address + string.Format(SetMetaCommand,
                PeercastUtils.PercentEncode(name), PeercastUtils.PercentEncode(genre),
                PeercastUtils.PercentEncode(description), PeercastUtils.PercentEncode(url),
                PeercastUtils.PercentEncode(comment),
                PeercastUtils.PercentEncode(trackArtist), PeercastUtils.PercentEncode(trackTitle),
                PeercastUtils.PercentEncode(trackAlbum), PeercastUtils.PercentEncode(trackGenre),
                PeercastUtils.PercentEncode(trackContact))));
        }

        public override Task StopAsync(string id)
        {
            return Task.Factory.StartNew(() =>
                Console.WriteLine(
                "http://" + address + string.Format(StopCommand, id)));
        }

        public override Task KeepAsync(string id)
        {
            return Task.Factory.StartNew(() =>
                Console.WriteLine(
                "http://" + address + string.Format(KeepCommand, id)));
        }

        public override Task<string> GetViewXmlAsync()
        {
            return Task.Factory.StartNew(() => "");
        }

        public override Task<string> GetSettingsHtmlAsync()
        {
            return Task.Factory.StartNew(() => "");
        }

        public override Task ApplyAsync(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var sb = new StringBuilder(ApplyUrl);
            foreach (var item in parameters)
            {
                sb.Append('&').Append(
                    PeercastUtils.PercentEncode(item.Key)).Append('=')
                    .Append(PeercastUtils.PercentEncode(item.Value));
            }
            return Task.Factory.StartNew(() =>
                Console.WriteLine(sb.ToString()));
        }
    }
}
