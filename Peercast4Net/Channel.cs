using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net
{
    public class Channel
    {
        public string Name { get; private set; }
        public string Id { get; private set; }
        public int Bitrate { get; private set; }
        public string Type { get; private set; }
        public int TotalListeners { get; private set; }
        public int TotalRelays { get; private set; }
        public int LocalListeners { get; private set; }
        public int LocalRelays { get; private set; }
        public string Status { get; private set; }
        public bool IsKeep { get; private set; }
        public string Genre { get; private set; }
        public string Description { get; private set; }
        public string ContactUrl { get; private set; }
        public string Comment { get; private set; }
        public int Age { get; private set; }

        public Channel(string name, string id, int bitrate, string type, int totalListeners, int totalRelays,
            int localListeners, int localRelays, string status, bool isKeep, string genre, string description,
            string contactUrl, string comment, int age)
        {
            Name = name;
            Id = id;
            Bitrate = bitrate;
            Type = type;
            TotalListeners = totalListeners;
            TotalRelays = totalRelays;
            LocalListeners = localListeners;
            LocalRelays = localRelays;
            Status = status;
            IsKeep = isKeep;
            Genre = genre;
            Description = description;
            ContactUrl = contactUrl;
            Comment = comment;
            Age = age;
        }
    }
}
