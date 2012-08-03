using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net
{
    public abstract class PeercastBase : IPeercast
    {
        #region IPeercast メンバー

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

        public abstract Task<Tuple<string, int>> BroadcastAsync(
            YellowPages yellowPages, BroadcastParameter parameter);
        public abstract Task<IEnumerable<IChannel>> GetChannelsAsync();
        public abstract Task<Tuple<int, int>> GetListenersAsync(string name);
        public abstract Task StopAsync(string id);
        public abstract Task UpdateAsync(UpdateParameter parameter);

        #endregion
    }
}
