using System;
using System.Threading;

#if (!NET30 && !NET35 && !NET40)
using System.Threading.Tasks;
#endif

namespace CrossChannel
{
    public interface IBroadcastSender<T>
    {
        IChannel Channel { get; }

        Action<T, Exception> ExceptionThrown { get; set; }

        void Open(IChannel channel);

        void Send(T message);

#if (!NET30 && !NET35 && !NET40)
        Task SendAsync(T message, CancellationToken cancellationToken = default(CancellationToken));
#endif
    }
}