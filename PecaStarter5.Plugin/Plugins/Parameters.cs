using System.Collections.Generic;
using Progressive.Peercast4Net.Datas;

namespace Progressive.PecaStarter5.Plugins
{
    public class BroadcastingParameter
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

    public class StoppedParameter
    {
        public Dictionary<string, string> YellowPagesParameters { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class InterruptedParameter
    {
        public string Name { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public string Comment { get; set; }
    }
}
