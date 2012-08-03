namespace Progressive.Peercast4Net.Datas
{
    public interface IChannel
    {
        string Name { get; }
        string Id { get; }
        int Bitrate { get; }
        string Type { get; }
        int TotalListeners { get; }
        int TotalRelays { get; }
        int LocalListeners { get; }
        int LocalRelays { get; }
        string Status { get; }
        string Genre { get; }
        string Description { get; }
        string ContactUrl { get; }
        string Comment { get; }
        int Age { get; }
    }
}
