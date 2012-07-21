using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class BroadcastedParameter
    {
        public IEnumerable<KeyValuePair<string, string>> YellowPagesParameters { get; set; }
        public int Bitrate { get; set; }
        public string Id { get; set; }
        public BroadcastParameter BroadcastParameter { get; set; }
    }
}
