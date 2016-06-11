namespace CrossChannel
{
    public class Channel : IChannel
    {
        public Channel(string name, ChannelMode mode = ChannelMode.Local)
        {
            Name = name;
            Mode = mode;
        }

        public string Name { get; }

        public ChannelMode Mode { get; }
    }
}