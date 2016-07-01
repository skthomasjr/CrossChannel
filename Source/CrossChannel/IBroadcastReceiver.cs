using System;
using System.ServiceModel;

namespace CrossChannel
{
    /// <summary>
    ///     The broadcast receiver.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    [ServiceContract]
    public interface IBroadcastReceiver<T> : IDisposable
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
        ///     Gets or sets the message received action.
        /// </summary>
        /// <value>The message received.</value>
        Action<T> MessageReceived { get; set; }

        /// <summary>
        ///     Opens the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        void Open(IChannel channel);

        /// <summary>
        ///     Receives the message.
        /// </summary>
        /// <param name="message">The message.</param>
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(T message);
    }
}