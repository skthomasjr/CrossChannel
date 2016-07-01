namespace CrossChannel
{
    /// <summary>
    ///     The communications channel.
    /// </summary>
    public class Channel : IChannel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Channel" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="mode">The mode.</param>
        public Channel(string name, ChannelMode mode = ChannelMode.Local)
        {
            Name = name;
            Mode = mode;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        ///     Gets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public ChannelMode Mode { get; }
    }
}