namespace CrossChannel
{
    /// <summary>
    /// The communications channel.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        /// <value>The mode.</value>
        ChannelMode Mode { get; }
    }
}