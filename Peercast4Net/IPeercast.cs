using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Progressive.Peercast4Net
{
    public interface IPeercast
    {
        string Address { get; set; }
        Task<Tuple<string, int>> BroadcastAsync(YellowPages yellowPages, BroadcastParameter parameter);
        Task<IEnumerable<IChannel>> GetChannelsAsync();
        Task<Tuple<int, int>> GetListenersAsync(string name);
        Task StopAsync(string id);
        Task UpdateAsync(UpdateParameter parameter);
    }
}
