namespace Progressive.PecaStarter5.Models.Channels
{
    public class BroadcastingChannel
    {
        public BroadcastingChannel(string name,string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; private set; }
        public string Id { get; private set; }
    }
}
