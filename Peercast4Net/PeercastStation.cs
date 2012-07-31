using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net
{
    public class PeercastStation : IPeercast
    {
        #region IPeercast メンバー

        public string Address
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Threading.Tasks.Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<Tuple<int, int>> GetListenersAsync(string name)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task StopAsync(string id)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateAsync(UpdateParameter parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
