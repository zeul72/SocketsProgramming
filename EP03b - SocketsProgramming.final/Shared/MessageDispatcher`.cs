#nullable enable

using System;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class MessageDispatcher<TMessageType> where TMessageType : class, new()
    {
        public abstract void Register<TParam, TResult>( Func<TParam, Task<TResult>> target );
        public abstract void Register<TParam>( Func<TParam, Task> target );
        public abstract Task<TMessageType?> DispatchAsync( TMessageType message );

    }
}
