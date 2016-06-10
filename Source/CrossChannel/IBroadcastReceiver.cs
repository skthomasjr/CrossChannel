using System;
using System.ServiceModel;

namespace CrossChannel
{
    [ServiceContract]
    public interface IBroadcastReceiver<T>
    {
        Action<T> OnMessageReceived { get; set; }

        void Close();

        [OperationContract(IsOneWay = true)]
        void MessageReceived(T message);

        void Open(IChannel channel, Action<T> onMessageReceived = null);
    }
}