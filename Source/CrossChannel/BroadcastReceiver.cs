using System;
using System.ServiceModel;
using System.ServiceModel.PeerResolvers;

namespace CrossChannel
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class BroadcastReceiver<T> : IBroadcastReceiver<T>, IDisposable
    {
        private ServiceHost host;

        public void Close()
        {
            host.Close();
        }

        public virtual void MessageReceived(T message)
        {
            OnMessageReceived?.Invoke(message);
        }

        public void Open(IChannel channel, Action<T> onMessageReceived = null)
        {
            OnMessageReceived = onMessageReceived;

            host = new ServiceHost(this);

            if (channel?.Mode == ChannelMode.Local)
            {
                var localBinding = new NetNamedPipeBinding();
                localBinding.Security.Mode = NetNamedPipeSecurityMode.Transport;

                var localEndpoint = new Uri($"net.pipe://localhost/{channel.Name}.{typeof(T).Name}");
                host.AddServiceEndpoint(typeof(IBroadcastReceiver<T>), localBinding, localEndpoint);
            }

            if (channel?.Mode == ChannelMode.Mesh)
            {
                var meshBinding = new NetPeerTcpBinding();
                meshBinding.Resolver.Mode = PeerResolverMode.Pnrp;
                meshBinding.Security.Mode = SecurityMode.None;

                var meshEndpoint = new Uri($"net.p2p://{channel.Name}.{typeof(T).Name}");
                host.AddServiceEndpoint(typeof(IBroadcastReceiver<T>), meshBinding, meshEndpoint);
            }

            host.Open();
        }

        public Action<T> OnMessageReceived { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Close();
            host = null;
        }
    }
}