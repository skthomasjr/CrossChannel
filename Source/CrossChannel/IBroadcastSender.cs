using System.Threading;
using System.Threading.Tasks;

namespace CrossChannel
{
    public interface IBroadcastSender<in T>
    {
        void Close();

        Task SendAsync(T message, CancellationToken cancellationToken = default(CancellationToken));

        void Open(IChannel channel);
    }
}