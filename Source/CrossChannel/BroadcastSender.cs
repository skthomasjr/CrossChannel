using System;
using System.ServiceModel;
using System.Threading;

#if (!NET30 && !NET35 && !NET40)
using System.Threading.Tasks;
#endif

namespace CrossChannel
{
    /// <summary>
    /// The broadcast sender.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BroadcastSender<T> : IBroadcastSender<T>
    {
        private IBroadcastReceiver<T> client;

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public IChannel Channel { get; private set; }

        /// <summary>
        /// Gets or sets the action to be executed when an exception is thrown.
        /// </summary>
        /// <value>The exception thrown.</value>
        public Action<T, Exception> ExceptionThrown { get; set; }

        private ChannelFactory<IBroadcastReceiver<T>> GetChannelFactory()
        {
            ChannelFactory<IBroadcastReceiver<T>> channelFactory = null;

            if (Channel?.Mode == ChannelMode.Local)
            {
                var localBinding = new NetNamedPipeBinding();
                localBinding.Security.Mode = NetNamedPipeSecurityMode.Transport;

                var localEndpointUri = new Uri($"net.pipe://localhost/{Channel.Name}");
                var localEndpoint = new EndpointAddress(localEndpointUri);

                channelFactory = new ChannelFactory<IBroadcastReceiver<T>>(localBinding);
                client = channelFactory.CreateChannel(localEndpoint);
            }

            if (Channel?.Mode == ChannelMode.Mesh)
            {
                var meshBinding = new NetPeerTcpBinding();
                meshBinding.Security.Mode = SecurityMode.None;

                var meshEndpointUri = new Uri($"net.p2p://{Channel.Name}");
                var meshEndpoint = new EndpointAddress(meshEndpointUri);

                channelFactory = new ChannelFactory<IBroadcastReceiver<T>>(meshBinding);
                client = channelFactory.CreateChannel(meshEndpoint);
            }

            return channelFactory;
        }

        /// <summary>
        /// Opens the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public void Open(IChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Send(T message)
        {
            try
            {
                using (GetChannelFactory())
                {
                    client?.ReceiveMessage(message);
                }
            }
            catch (Exception ex)
            {
                ExceptionThrown?.Invoke(message, ex);
            }
        }

#if (!NET30 && !NET35 && !NET40)
        /// <summary>
        /// Sends asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public async Task SendAsync(T message, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                using (GetChannelFactory())
                {
                    await Task.Run(() => client?.ReceiveMessage(message), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                ExceptionThrown?.Invoke(message, ex);
            }
        }
#endif
    }
}