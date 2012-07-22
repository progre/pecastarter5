using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.Peercast4Net
{
    [Obsolete]
    public interface IChannelParameter : IBroadcastParameter, IUpdateParameter
    {
        string Id { get; }
        string StreamUrl { get; }
        string Name { get; }
        string Genre { get; }
        string Description { get; }
        string Type { get; }
        string ContactUrl { get; }
        string Comment { get; }
        string TrackArtist { get; }
        string TrackTitle { get; }
        string TrackAlbum { get; }
        string TrackGenre { get; }
        string TrackContact { get; }
    }

    [Obsolete]
    public interface IBroadcastParameter
    {
        string StreamUrl { get; }
        string Name { get; }
        string Genre { get; }
        string Description { get; }
        string Type { get; }
        string ContactUrl { get; }
        string Comment { get; }
        string TrackArtist { get; }
        string TrackTitle { get; }
        string TrackAlbum { get; }
        string TrackGenre { get; }
        string TrackContact { get; }
    }

    [Obsolete]
    public interface IUpdateParameter
    {
        string Id { get; }
        string Name { get; }
        string Genre { get; }
        string Description { get; }
        string ContactUrl { get; }
        string Comment { get; }
        string TrackArtist { get; }
        string TrackTitle { get; }
        string TrackAlbum { get; }
        string TrackGenre { get; }
        string TrackContact { get; }
    }

    public class ChannelParameter : IBroadcastParameter, IUpdateParameter
    {
        public string Id { get; set; }
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
    }

    public class BroadcastParameter : IBroadcastParameter, ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    public class UpdateParameter : IUpdateParameter, ICloneable
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
}
