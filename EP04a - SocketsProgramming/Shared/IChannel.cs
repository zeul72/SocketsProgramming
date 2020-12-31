using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Shared
{
    public interface IChannel
    {
        Guid Id { get; }

        void Attach( Socket socket );
        void Close( );
        void Dispose( );
        Task SendAsync<T>( T message );
    }
}