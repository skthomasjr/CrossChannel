using System;
using System.ServiceModel;
using System.ServiceModel.PeerResolvers;

namespace CrossChannel
{
    /// <summary>
    ///     The broadcast receiver.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class BroadcastReceiver<T> : IBroadcastReceiver<T>
    {
        private ServiceHost host;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            MessageReceived = null;
            ExceptionThrown = null;

            host.Close();
            host = null;
        }

        /// <summary>
        ///     Gets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public IChannel Channel { get; private set; }

        /// <summary>
        ///     Gets or sets the message received action.
        /// </summary>
        /// <value>The message received.</value>
        public Action<T> MessageReceived { get; set; }

        /// <summary>
        ///     Gets or sets the action to be executed when an exception is thrown.
        /// </summary>
        /// <value>The exception thrown.</value>
        public Action<T, Exception> ExceptionThrown { get; set; }

        /// <summary>
        ///     Receives the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public virtual void ReceiveMessage(T message)
        {
            try
            {
                MessageReceived?.Invoke(message);
            }
            catch (Exception ex)
            {
                ExceptionThrown?.Invoke(message, ex);
            }
        }

        /// <summary>
        ///     Opens the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public void Open(IChannel channel)
        {
            Channel = channel;
            host = new ServiceHost(this);

            if (channel?.Mode == ChannelMode.Local)
            {
                var localBinding = new NetNamedPipeBinding();
                localBinding.Security.Mode = NetNamedPipeSecurityMode.Transport;

                var localEndpoint = new Uri($"net.pipe://localhost/{channel.Name}");
                host.AddServiceEndpoint(typeof(IBroadcastReceiver<T>), localBinding, localEndpoint);
            }

            if (channel?.Mode == ChannelMode.Mesh)
            {
                var meshBinding = new NetPeerTcpBinding();
                meshBinding.Resolver.Mode = PeerResolverMode.Pnrp;
                meshBinding.Security.Mode = SecurityMode.None;

                var meshEndpoint = new Uri($"net.p2p://{channel.Name}");
                host.AddServiceEndpoint(typeof(IBroadcastReceiver<T>), meshBinding, meshEndpoint);
            }

            host.Open();
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}