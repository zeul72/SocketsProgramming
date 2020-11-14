#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Shared
{
    public abstract class MessageDispatcher<TMessageType> where TMessageType : class, new()
    {
        List<(RouteAttribute routeAttribute, Func<TMessageType, Task<TMessageType?>> targetMethod)>
            Handlers { get; } = new List<(RouteAttribute routeAttribute, Func<TMessageType, Task<TMessageType?>> targetMethod)>( );

        public virtual async Task<TMessageType?> DispatchAsync( TMessageType message )
        {
            foreach ( var (route, target) in Handlers ) {
                if ( IsMatch( route, message ) ) {
                    return await target( message );
                }
            }
            //No handler?? what to do??
            return null;
        }

        public virtual void BindController<T>( )
        {
            static bool returnTypeIsTask( MethodInfo mi )
                => mi.ReturnType.IsAssignableFrom( typeof( Task ) );

            static bool returnTypeIsTaskT( MethodInfo mi )
                => mi.ReturnType.BaseType?.IsAssignableFrom( typeof( Task ) ) ?? false;

            var methods = typeof(T)
                            .GetMethods(BindingFlags.Public|BindingFlags.Static)
                            //must have a route
                            .Where( HasRouteAttribute )
                            //only support a single parameter
                            .Where( x => x.GetParameters().Count() == 1 )               
                            //only support methods that return a Task or Task<T>
                            .Where( x => returnTypeIsTask(x) || returnTypeIsTaskT(x));

            foreach ( var mi in methods ) {

                var wrapper = new Func<TMessageType, Task<TMessageType?>>( async msg => {
                    var @param = Deserialize(mi.GetParameters()[0].ParameterType,msg);
                    try {
                        if(returnTypeIsTask(mi))
                        {
                            var t = (mi.Invoke(null,new object[] { @param } ) as Task);
                            if ( t != null )
                                await t;
                            return null;
                        } else {
                            var result = (await (mi.Invoke(null,new object[] { @param }) as dynamic) as dynamic);
                            if ( result != null ) {
                                return Serialize( result.GetType(), result );
                            } else
                                return null;
                        }
                    } catch(Exception _e) {
                        //logging would go here & exception decisions happen here
                        Console.WriteLine(_e);
                        return null;
                    }
                } );

#pragma warning disable CS8604 // Possible null reference argument.
                //routeAttribute is not null here - hence the suppression
                AddHandler( GetRouteAttribute( mi ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }


        public virtual void BindChannel<TProtocol>( Channel<TProtocol, TMessageType> channel )
         where TProtocol : Protocol<TMessageType>, new()
            => channel.OnMessage( async m => {
                var response = await DispatchAsync(m).ConfigureAwait(false);
                if ( response != null ) {
                    try {
                        await channel.SendAsync( response ).ConfigureAwait( false );
                    } catch ( Exception _e ) {
                        Console.WriteLine( $"Oh NO!!! {_e}" );
                    }
                }
            } );

        public abstract void Register<TParam, TResult>( Func<TParam, Task<TResult>> target );
        public abstract void Register<TParam>( Func<TParam, Task> target );

        protected virtual void AddHandler( RouteAttribute routeAttribute, Func<TMessageType, Task<TMessageType?>> handler )
            => Handlers.Add( (routeAttribute, handler) );
        protected abstract object Deserialize( Type target, TMessageType data );
        protected abstract TMessageType Serialize( Type type, object @obj );
        protected abstract bool IsMatch( RouteAttribute route, TMessageType message );
        protected virtual bool HasRouteAttribute( MethodInfo mi ) => GetRouteAttribute( mi ) != null;
        protected abstract RouteAttribute? GetRouteAttribute( MemberInfo mi );
    }
}
