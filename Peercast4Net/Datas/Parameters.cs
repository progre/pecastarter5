using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net.Datas
{
    public class BroadcastParameter
    {
        public string StreamUrl { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string ContactUrl { get; set; }
        public string Comment { get; set; }
        public string TrackArtist { get; set; }
        public string TrackTitle { get; set; }
        public string TrackAlbum { get; set; }
        public string TrackGenre { get; set; }
        public string TrackContact { get; set; }

        #region ICloneable メンバー

        public BroadcastParameter Clone()
        {
            return (BroadcastParameter)MemberwiseClone();
        }

        #endregion
    }

    public class UpdateParameter : ICloneable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string ContactUrl { get; set; }
        public string Comment { get; set; }
        public string TrackArtist { get; set; }
        public string TrackTitle { get; set; }
        public string TrackAlbum { get; set; }
        public string TrackGenre { get; set; }
        public string TrackContact { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    public class Settings
    {
        public int Port { get; set; }
    }
}
