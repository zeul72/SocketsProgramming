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

        readonly List<(RouteAttribute route, Func<TMessageType,Task<TMessageType?>> targetMethod)> _handlers = new List<(RouteAttribute route, Func<TMessageType, Task<TMessageType?>> targetMethod)>();


        public void Bind<TProtocol>( Channel<TProtocol, TMessageType> channel )
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


        public void Bind<TController>()
        {
            static bool returnTypeIsTask( MethodInfo mi )
             => mi.ReturnType.IsAssignableFrom( typeof( Task ) );

            static bool returnTypeIsTaskT( MethodInfo mi )
                => mi.ReturnType.BaseType?.IsAssignableFrom( typeof( Task ) ) ?? false;

            var methods = typeof(TController)
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
                                return Serialize( result as dynamic );
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

        private bool HasRouteAttribute( MethodInfo mi ) => GetRouteAttribute( mi ) != null;

        public async Task<TMessageType?> DispatchAsync( TMessageType message )
        {
            foreach ( var (route, target) in _handlers ) {
                if ( IsMatch( route, message ) ) {
                    return await target( message );
                }
            }
            //No handler?? what to do??
            return null;
        }

        protected void AddHandler( RouteAttribute route, Func<TMessageType, Task<TMessageType?>> targetMethod )
            => _handlers.Add( (route, targetMethod) );

        protected abstract bool IsMatch( RouteAttribute route, TMessageType message );

        public virtual void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            if ( !HasAttribute( target.Method ) )
                throw new Exception( "Missing Required Route Attribute" );

            var wrapper = new Func<TMessageType,Task<TMessageType?>>( async xml => {
                var @param = Deserialize<TParam>(xml);
                var result = await target(@param);

                if(result != null)
                    return Serialize<TResult>(result);
                else
                    return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }


        protected abstract TParam Deserialize<TParam>( TMessageType message );
        protected abstract object Deserialize( Type paramType, TMessageType message );

        protected abstract TMessageType? Serialize<T>( T instance );

        public virtual void Register<TParam>( Func<TParam, Task> target )
        {
            if ( !HasAttribute( target.Method ) )
                throw new Exception( "Missing Required Route Attribute" );

            var wrapper = new Func<TMessageType,Task<TMessageType?>>( async xml => {
                var @param = Deserialize<TParam>(xml);
                await target(@param);
                return null;
            });

#pragma warning disable CS8604 // Possible null reference argument.
            AddHandler( GetRouteAttribute( target.Method ), wrapper );
#pragma warning restore CS8604 // Possible null reference argument.
        }

        protected bool HasAttribute( MethodInfo mi ) => GetRouteAttribute( mi ) != null;
        protected abstract RouteAttribute? GetRouteAttribute( MethodInfo mi );


    }
}
