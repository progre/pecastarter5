using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Datas;
using Progressive.Peercast4Net.Daos;

namespace Progressive.Peercast4Net
{
    public class PeercastStation : PeercastBase
    {
        #region IPeercast メンバー

        public override async Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            using (var dao = new PeercastStationDao(Address))
            {
                await dao.AddYellowPageAsync("pcp", yellowPages.Name, yellowPages.Url);
                throw new NotImplementedException();
            }
        }

        public override async Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            using (var dao = new PeercastStationDao(Address))
            {
                var xml = await dao.GetViewXmlAsync();
                return new XmlStatus(xml).Channels;
            }
        }

        public override Task<Tuple<int, int>> GetListenersAsync(string name)
        {
            throw new NotImplementedException();
        }

        public override Task StopAsync(string id)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(UpdateParameter parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
