using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net
{
    public class PeercastStation : PeercastBase
    {
        #region IPeercast メンバー

        public override Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            throw new NotImplementedException();
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
