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

namespace Progressive.Peercast4Net
{
    class Peercast
    {
        private string address;
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

        public Peercast()
        {
        }

        public Task SetYellowPagesAsync(string yellowPagesAddress)
        {
            throw new NotImplementedException();// TODO:
        }

        public Task<Tuple<int, int>> GetListenersAsync()
        {
            throw new NotImplementedException();// TODO:
        }

        public Task<IEnumerable<Channel>> GetChannelsAsync()
        {
            throw new NotImplementedException();// TODO:
        }

        public async Task BroadcastAsync(
            string streamUrl, string name, string genre, string description, string type,
            string contactUrl, string comment,
            string trackArtist, string trackTitle, string trackAlbum, string trackGenre, string trackContact)
        {
            using (var dao = new PeercastDao(Address))
            {
                if (await ExistsAsync(dao, name))
                {
                    throw new PeercastException("Channel was duplicated.");
                }
                await dao.FetchAsync(streamUrl, name, genre, description, contactUrl, type);
                var id = await GetChannelIdAsync(dao, name);
                if (id == "")
                {
                    await dao.StopAsync(id);
                    throw new PeercastException("Creating channel failed.");
                }
                // TODO: setmeta
            }
        }

        public Task UpdateAsync(string genre, string description,
            string contactUrl, string comment,
            string trackArtist, string trackTitle, string trackAlbum, string trackGenre, string trackContact)
        {
            throw new NotImplementedException();// TODO:
        }

        private Task StopAsync(string id)
        {
            using (var dao = new PeercastDao(Address))
            {
                return dao.StopAsync(id);
            }
        }

        private Task<bool> ExistsAsync(PeercastDao dao, string name)
        {
            throw new NotImplementedException();// TODO:
        }

        private Task<string> GetChannelIdAsync(PeercastDao dao, string name)
        {
            throw new NotImplementedException();// TODO:
        }
    }
}
