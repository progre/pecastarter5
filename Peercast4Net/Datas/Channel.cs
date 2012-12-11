namespace Progressive.Peercast4Net.Datas
{
    public class Channel : IChannel
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Bitrate { get; set; }
        public string Type { get; set; }
        public int TotalListeners { get; set; }
        public int TotalRelays { get; set; }
        public int LocalListeners { get; set; }
        public int LocalRelays { get; set; }
        public string Status { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public string ContactUrl { get; set; }
        public string Comment { get; set; }
        public int Age { get; set; }
    }
}
