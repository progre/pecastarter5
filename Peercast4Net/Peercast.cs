using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using Progressive.Peercast4Net.Commons;
using Progressive.Peercast4Net.Utils;
using Progressive.Peercast4Net.Commons.Dao;
using Progressive.Peercast4Net.Dao;
using Progressive.Peercast4Net.Settings;
using System.Configuration;
using System.Net.Configuration;

namespace Progressive.Peercast4Net
{
    public class Peercast
    {
        public const string NullId = "00000000000000000000000000000000";

        public Peercast()
        {
        }

        private string address = "localhost:7144";
        public string Address
        {
            get { return address; }
            set
            {
                if (!Regex.IsMatch(value, "^[^;/?:@&=+$,]+(:[0-9]{1,5})$"))
                {
                    throw new ArgumentException("Invalid address.");
                }
                address = value;
            }
        }

        public async Task<string> GetYellowPagesAsync()
        {
            IList<KeyValuePair<string, string>> nvc;
            using (var dao = new PeercastDao(Address))
            {
                nvc = await GetSettingsAsync(dao);
            }
            return nvc.Single(x => x.Key == "yp").Value;
        }

        public async Task SetYellowPagesAsync(string yellowPagesAddress)
        {
            using (var dao = new PeercastDao(Address))
            {
                var nvc = await GetSettingsAsync(dao);
                var ypParam = nvc.Single(x => x.Key == "yp");
                if (yellowPagesAddress == ypParam.Value)
                    return;

                nvc.Remove(ypParam);
                nvc.Add(new KeyValuePair<string, string>("yp", yellowPagesAddress));
                await dao.ApplyAsync(nvc);
            }
        }

        public async Task<Tuple<int, int>> GetListenersAsync(string name)
        {
            using (var dao = new PeercastDao(Address))
            {
                return (await GetXmlStatusAsync(dao)).GetHits(name);
            }
        }

        public async Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            using (var dao = new PeercastDao(Address))
            {
                return new XmlStatus(await dao.GetViewXmlAsync()).Channels;
            }
        }

        public async Task<string> BroadcastAsync(BroadcastParameter parameter)
        {
            using (var dao = new PeercastDao(Address))
            {
                if (await ExistsAsync(dao, parameter.Name))
                {
                    throw new PeercastException("Channel was duplicated.");
                }
                await dao.FetchAsync(parameter.StreamUrl, parameter.Name, parameter.Genre, parameter.Description,
                    parameter.ContactUrl, parameter.Type);
                var id = await GetChannelIdAsync(dao, parameter.Name);
                if (id == "")
                {
                    await dao.StopAsync(id);
                    throw new PeercastException("Creating channel failed.");
                }
                await dao.SetMetaAsync(parameter.Name, parameter.Genre, parameter.Description,
                    parameter.ContactUrl, parameter.Comment,
                    parameter.TrackArtist, parameter.TrackTitle, parameter.TrackAlbum,
                    parameter.TrackGenre, parameter.TrackContact);
                return id;
            }
        }

        public async Task UpdateAsync(UpdateParameter parameter)
        {
            using (var dao = new PeercastDao(Address))
            {
                await dao.SetMetaAsync(parameter.Name, parameter.Genre, parameter.Description,
                    parameter.ContactUrl, parameter.Comment,
                    parameter.TrackArtist, parameter.TrackTitle, parameter.TrackAlbum,
                    parameter.TrackGenre, parameter.TrackContact);
            }
        }

        public Task StopAsync(string id)
        {
            using (var dao = new PeercastDao(Address))
            {
                return dao.StopAsync(id);
            }
        }

        private async Task<IList<KeyValuePair<string, string>>> GetSettingsAsync(PeercastDao dao)
        {
            var html = await dao.GetSettingsHtmlAsync();
            return new SettingsHtmlParser().Parse(html);
        }

        private async Task<bool> ExistsAsync(PeercastDao dao, string name)
        {
            return (await GetXmlStatusAsync(dao)).Exists(name);
        }

        private async Task<string> GetChannelIdAsync(PeercastDao dao, string name)
        {
            return (await GetXmlStatusAsync(dao)).GetChannelId(name);
        }

        private async Task<XmlStatus> GetXmlStatusAsync(PeercastDao dao)
        {
            var xml = await dao.GetViewXmlAsync();
            return new XmlStatus(xml);
        }
    }
}
