using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.Peercast4Net;

namespace Progressive.PecaStarter5.Models.Plugins
{
    public class BroadcastedParameter
    {
        public Dictionary<string, string> YellowPagesParameters { get; set; }
        public int Bitrate { get; set; }
        public string Id { get; set; }
        public BroadcastParameter BroadcastParameter { get; set; }
    }

    public class UpdatedParameter
    {
        public Dictionary<string, string> YellowPagesParameters { get; set; }
        public UpdateParameter UpdateParameter { get; set; }
    }

    public class StopedParameter
    {
        public Dictionary<string, string> YellowPagesParameters { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
