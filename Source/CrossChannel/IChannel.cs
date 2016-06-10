namespace CrossChannel
{
    public interface IChannel
    {
        string Name { get; }

        ChannelMode Mode { get; }
    }
}