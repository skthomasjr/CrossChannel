using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

namespace CrossChannel
{
    public class BroadcastSender<T> : IBroadcastSender<T>, IDisposable
    {
        private IChannelFactory<IBroadcastReceiver<T>> channelFactory;
        private IBroadcastReceiver<T> client;

        public void Close()
        {
            channelFactory?.Close();
        }

        public void Open(IChannel channel)
        {
            if (channel?.Mode == ChannelMode.Local)
            {
                var localBinding = new NetNamedPipeBinding();
                localBinding.Security.Mode = NetNamedPipeSecurityMode.Transport;

                var localEndpointUri = new Uri($"net.pipe://localhost/{channel.Name}.{typeof(T).Name}");
                var localEndpoint = new EndpointAddress(localEndpointUri);

                channelFactory = new ChannelFactory<IBroadcastReceiver<T>>(localBinding);
                client = channelFactory.CreateChannel(localEndpoint);
            }

            if (channel?.Mode == ChannelMode.Mesh)
            {
                var meshBinding = new NetPeerTcpBinding();
                meshBinding.Security.Mode = SecurityMode.None;

                var meshEndpointUri = new Uri($"net.p2p://{channel.Name}.{typeof(T).Name}");
                var meshEndpoint = new EndpointAddress(meshEndpointUri);

                channelFactory = new ChannelFactory<IBroadcastReceiver<T>>(meshBinding);
                client = channelFactory.CreateChannel(meshEndpoint);
            }
        }

        public async Task SendAsync(T message, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.Run(() => client?.MessageReceived(message), cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Close();
            channelFactory = null;
            client = null;
        }
    }
}