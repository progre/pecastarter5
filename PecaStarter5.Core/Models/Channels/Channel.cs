using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Progressive.PecaStarter5.Models.Channels
{
    public class Channel
    {
        string Id { get; set; }
        string StreamUrl { get; set; }
        string Name { get; set; }
        string Genre { get; set; }
        string Description { get; set; }
        string Type { get; set; }
        string ContactUrl { get; set; }
        string Comment { get; set; }
        string TrackArtist { get; set; }
        string TrackTitle { get; set; }
        string TrackAlbum { get; set; }
        string TrackGenre { get; set; }
        string TrackContact { get; set; }
        Dictionary<string, string> Extensions { get; set; }
    }
}
