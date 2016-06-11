using System;
using System.ServiceModel;

namespace CrossChannel
{
    [ServiceContract]
    public interface IBroadcastReceiver<T> : IDisposable
    {
        IChannel Channel { get; }

        Action<T, Exception> ExceptionThrown { get; set; }

        Action<T> MessageReceived { get; set; }

        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(T message);

        void Open(IChannel channel);
    }
}