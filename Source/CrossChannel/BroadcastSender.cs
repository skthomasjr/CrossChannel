using System;
using System.ServiceModel;
using System.Threading;

#if (!NET30 && !NET35 && !NET40)
using System.Threading.Tasks;
#endif

namespace CrossChannel
{
    public class BroadcastSender<T> : IBroadcastSender<T>
    {
        private IBroadcastReceiver<T> client;

        public IChannel Channel { get; private set; }

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

        public void Open(IChannel channel)
        {
            Channel = channel;
        }

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