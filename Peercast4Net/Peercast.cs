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
    public class Peercast
    {
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

        public async Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            using (var dao = new PeercastDao(Address))
            {
                return new XmlStatus(await dao.GetViewXmlAsync()).Channels;
            }
        }

        public async Task BroadcastAsync(BroadcastParameter parameter)
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
                // TODO: setmeta
            }
        }

        public Task UpdateAsync(UpdateParameter parameter)
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
