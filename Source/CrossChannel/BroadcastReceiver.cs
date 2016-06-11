using System;
using System.ServiceModel;
using System.ServiceModel.PeerResolvers;

namespace CrossChannel
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class BroadcastReceiver<T> : IBroadcastReceiver<T>
    {
        private ServiceHost host;

        public IChannel Channel { get; private set; }

        public Action<T> MessageReceived { get; set; }

        public Action<T, Exception> ExceptionThrown { get; set; }

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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            host.Close();
            host = null;
        }
    }
}