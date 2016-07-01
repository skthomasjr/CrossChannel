using System;
using System.Threading;
#if (!NET30 && !NET35 && !NET40)
using System.Threading.Tasks;
#endif

namespace CrossChannel
{
    /// <summary>
    ///     The broadcast sender.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    public interface IBroadcastSender<T>
    {
        /// <summary>
        ///     Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        IChannel Channel { get; }

        /// <summary>
        ///     Gets or sets the action to be executed when an exception is thrown.
        /// </summary>
        /// <value>The exception thrown.</value>
        Action<T, Exception> ExceptionThrown { get; set; }

        /// <summary>
        ///     Opens the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        void Open(IChannel channel);

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Send(T message);

#if (!NET30 && !NET35 && !NET40)
        /// <summary>
        ///     Sends asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        Task SendAsync(T message, CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}