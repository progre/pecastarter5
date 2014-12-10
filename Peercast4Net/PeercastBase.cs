﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Progressive.Peercast4Net.Daos;
using Progressive.Peercast4Net.Datas;

namespace Progressive.Peercast4Net
{
    public abstract class PeerCastBase : IPeerCast
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

        public Task<IEnumerable<IChannel>> GetChannelsAsync()
        {
            return GetXmlStatusAsync().ContinueWith(t => t.Result.Channels);
        }

        public Task<IChannel> GetChannelAsync(string id)
        {
            return GetXmlStatusAsync().ContinueWith(t => t.Result.GetChannel(id));
        }

        [Obsolete]
        public Task<Tuple<int, int>> GetListenersAsync(string name)
        {
            return GetXmlStatusAsync().ContinueWith(t => t.Result.GetHits(name));
        }

        public abstract Task<Tuple<string, int>> BroadcastAsync(
            YellowPages yellowPages, BroadcastParameter parameter);
        public abstract Task StopAsync(string id);
        public abstract Task UpdateAsync(UpdateParameter parameter);

        #endregion

        private Task<XmlStatus> GetXmlStatusAsync()
        {
            var dao = new PeercastDao(Address);
            return dao.GetViewXmlAsync().ContinueWith(t =>
            {
                dao.Dispose();
                return new XmlStatus(t.Result);
            });
        }
    }
}
