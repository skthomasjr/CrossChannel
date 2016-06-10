namespace CrossChannel
{
    public class Channel : IChannel
    {
        public Channel(string name, ChannelMode mode = ChannelMode.Local) : this()
        {
            Name = name;
            Mode = mode;
        }

        public Channel() { }

        public string Name { get; }

        public ChannelMode Mode { get; }
    }
}