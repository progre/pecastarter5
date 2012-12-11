using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Plugins;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Models.Dxos
{
    static class ChannelDxo
    {
        public static IChannel ToChannel(BroadcastingParameter parameter)
        {
            return new Channel
            {
                Name = parameter.BroadcastParameter.Name,
                Id = parameter.Id,
                Bitrate = parameter.Bitrate,
                Type = "",
                TotalListeners = 0,
                TotalRelays = 0,
                LocalListeners = 0,
                LocalRelays = 0,
                Status = "",
                Genre = parameter.BroadcastParameter.Genre,
                Description = parameter.BroadcastParameter.Description,
                ContactUrl = parameter.BroadcastParameter.ContactUrl,
                Comment = parameter.BroadcastParameter.Comment,
                Age = 0
            };
        }

        public static IChannel ToChannel(UpdatedParameter parameter)
        {
            return new Channel
            {
                Name = parameter.UpdateParameter.Name,
                Id = parameter.UpdateParameter.Id,
                Bitrate = 0,
                Type = "",
                TotalListeners = 0,
                TotalRelays = 0,
                LocalListeners = 0,
                LocalRelays = 0,
                Status = "",
                Genre = parameter.UpdateParameter.Genre,
                Description = parameter.UpdateParameter.Description,
                ContactUrl = parameter.UpdateParameter.ContactUrl,
                Comment = parameter.UpdateParameter.Comment,
                Age = 0
            };
        }
    }
}
