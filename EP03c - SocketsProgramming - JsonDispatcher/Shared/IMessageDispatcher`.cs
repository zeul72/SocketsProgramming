#nullable enable

using System.Threading.Tasks;

namespace Shared
{
    public interface IMessageDispatcher<TMessageType> where TMessageType : class
    {
        Task<TMessageType?> DispatchAsync( TMessageType message );
        void BindController<T>( );
        void BindChannel<TProtocol>( Channel<TProtocol, TMessageType> channel ) where TProtocol : Protocol<TMessageType>, new();
    }
}
