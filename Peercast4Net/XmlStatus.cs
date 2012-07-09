using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Progressive.Peercast4Net
{
    public class XmlStatus
    {
        private XDocument xml;

        public IEnumerable<IChannel> Channels
        {
            get
            {
                return ChannelElements.Select(x => CreateChannel(x));
            }
        }

        private Channel CreateChannel(XElement channel)
        {
            var founds = FoundChannelElements;
            var relay = channel.Element("relay");
            var hits = founds.Where(ch => ch.Attribute("id").Value == channel.Attribute("id").Value).Single().Element("hits");
            return new Channel()
            {
                Name = channel.Attribute("name").Value,
                Id = channel.Attribute("id").Value,
                Bitrate = int.Parse(channel.Attribute("bitrate").Value),
                Type = channel.Attribute("type").Value,
                Genre = channel.Attribute("genre").Value,
                Description = channel.Attribute("desc").Value,
                ContactUrl = channel.Attribute("url").Value,
                Age = int.Parse(channel.Attribute("age").Value),
                Comment = channel.Attribute("comment").Value,
                LocalListeners = int.Parse(relay.Attribute("listeners").Value),
                LocalRelays = int.Parse(relay.Attribute("relays").Value),
                Status = relay.Attribute("status").Value,
                TotalListeners = int.Parse(hits.Attribute("listeners").Value),
                TotalRelays = int.Parse(hits.Attribute("relays").Value),
            };
        }

        private IEnumerable<XElement> ChannelElements
        {
            get
            {
                return xml.Element("peercast").Elements("channels_relayed").Elements("channel");
            }
        }
        private IEnumerable<XElement> FoundChannelElements
        {
            get
            {
                return xml.Element("peercast").Elements("channels_found").Elements("channel");
            }
        }

        public XmlStatus(string source)
        {
            xml = XDocument.Parse(source);
        }

        public bool Exists(string name)
        {
            foreach (var nameAttribute in ChannelElements.Attributes("name"))
            {
                if (nameAttribute.Value == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool EqualsChannel(string name, string genre, string description, string url, string comment,
            string trackTitle, string trackArtist, string trackAlbum, string trackGenre, string trackContact)
        {
            foreach (var channel in ChannelElements)
            {
                if (channel.Attribute("name").Value != name)
                {
                    continue;
                }
                if (channel.Attribute("genre").Value != genre ||
                    channel.Attribute("desc").Value != description ||
                    channel.Attribute("url").Value != url ||
                    channel.Attribute("comment").Value != comment)
                {
                    return false;
                }
                var track = channel.Element("track");
                if (track.Attribute("title").Value != trackTitle ||
                    track.Attribute("artist").Value != trackArtist ||
                    track.Attribute("album").Value != trackAlbum ||
                    track.Attribute("genre").Value != trackGenre ||
                    track.Attribute("contact").Value != trackContact)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public int GetBitrate(string name)
        {
            foreach (var channel in ChannelElements)
            {
                if (channel.Attribute("name").Value == name)
                {
                    return int.Parse(channel.Attribute("bitrate").Value);
                }
            }
            return -1;
        }

        public string GetChannelId(string name)
        {
            foreach (var channel in ChannelElements)
            {
                if (channel.Attribute("name").Value == name)
                {
                    return channel.Attribute("id").Value;
                }
            }
            return PeercastServerDefine.NullId;
        }

        public bool IsExistOnRelaysById(string id)
        {
            foreach (XAttribute idAttribute in ChannelElements.Attributes("id"))
            {
                if (idAttribute.Value == id)
                {
                    return true;
                }
            }
            return false;
        }
        public Tuple<int, int> GetHits(string name)
        {
            foreach (XElement channel in xml.Element("peercast").Elements("channels_found").Elements("channel"))
            {
                if (channel.Attribute("name").Value != name)
                {
                    continue;
                }
                XElement hit = channel.Element("hits");
                return Tuple.Create(int.Parse(hit.Attribute("listeners").Value),
                        int.Parse(hit.Attribute("hosts").Value));
            }
            throw new ApplicationException();
        }
    }
}
