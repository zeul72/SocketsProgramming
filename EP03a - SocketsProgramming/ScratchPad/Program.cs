#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ScratchPad
{

    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false )]
    public class RouteAttribute : Attribute
    {
        public string Path { get; }

        public RouteAttribute( string path ) => Path = path;
    }


    public abstract class MessageDispatcher<TMessageType> where TMessageType : class, new()
    {
        public abstract void Register<TParam, TResult>( Func<TParam, Task<TResult>> target );
        public abstract void Register<TParam>( Func<TParam, Task> target );
        public abstract Task<TMessageType?> DispatchAsync( TMessageType message );

    }

    public class XDocumentMessageDispatcher : MessageDispatcher<XDocument>
    {
        readonly List<(string,Func<XDocument,Task<XDocument?>>)> _handlers = new List<(string, Func<XDocument, Task<XDocument?>>)>();

        public override async Task<XDocument?> DispatchAsync( XDocument message )
        {
            foreach(var (route,target) in _handlers)
            {
                if( (message.XPathEvaluate(route) as bool?) == true)
                {
                    return await target( message );
                }
            }

            //No Handler registered
            return null;
        }

        public override void Register<TParam, TResult>( Func<TParam, Task<TResult>> target )
        {
            async Task<XDocument?> wrapper( XDocument xml )
            {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                var result = await target(@param);
                if ( result != null )
                    return XmlSerialization.Serialize( result );
                else
                    return null;
            }

            _handlers.Add( (GetRouteExpression( target.Method ), wrapper) );
        }

        public override void Register<TParam>( Func<TParam, Task> target )
        {
            async Task<XDocument?> wrapper( XDocument xml )
            {
                var @param = XmlSerialization.Deserialize<TParam>(xml);
                await target( @param );
                return null;
            }

            _handlers.Add( (GetRouteExpression( target.Method ), wrapper) );
        }

        string GetRouteExpression( MethodInfo mi )
        {
            var route =   mi.GetCustomAttribute<RouteAttribute>( );
            if ( route == null )
                throw new ArgumentException( $"{mi.Name} missing RouteAttribute" );

            return $"boolean({route.Path})";
        }
    }

    class Program
    {
        static async Task Main( string[ ] args )
        {
            var messageHandler = new XDocumentMessageDispatcher( ); 

            //register handlers
            messageHandler.Register<HeartBeatRequestMessage, HeartBeatResponseMessage>( HeartBeatRequestHandler );
            messageHandler.Register<HeartBeatResponseMessage>( HeartBeatResponseHandler );

            //create hb message and serialize to XDocument
            var hbRequest = new HeartBeatRequestMessage {
                Id = "HB_0001",
                POSData = new POSData { Id = "POS_001"}
            };
            var hbRequestXDoc = XmlSerialization.Serialize(hbRequest);


            //dispatch the message
            var responseXDoc = await messageHandler.DispatchAsync(hbRequestXDoc);
            if ( responseXDoc != null )
                await messageHandler.DispatchAsync( responseXDoc );

        }


        //Handler on the 'Server' side of the system
        [Route( "/Message[@type='Request' and @action='HeartBeat']" )]
        public static Task<HeartBeatResponseMessage> HeartBeatRequestHandler( HeartBeatRequestMessage request )
        {
            var response = new HeartBeatResponseMessage {
                Id = request.Id,
                POSData = request.POSData,
                Result = new Result { Status = Status.Success}
            };

            return Task.FromResult( response );
        }

        //Handler on the 'Client' side of the system
        [Route( "/Message[@type='Response' and @action='HeartBeat']" )]
        public static Task HeartBeatResponseHandler( HeartBeatResponseMessage request )
        {
            Console.WriteLine( $"Received Response: {request?.Result?.Status}" );
            return Task.CompletedTask;
        }
    }
}
