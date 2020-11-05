#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace ScratchPad
{

    /*
     *  <Message type='Request' action='HeartBeat' id='0001'>
     *    <POS id='POS_001'/>
     *  </Message>
     * 
     * <Message type='Response' action='HeartBeat' id='0001'>
     *   <Result status='Success'/>
     * </Message>
     * 
     */

    public enum MessageType
    {
        Request,
        Response
    }

    public enum Status
    {
        Success,
        Failure
    }

    [XmlRoot("Message")]
    public abstract class Message
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }

        [XmlAttribute("type")]
        public MessageType Type { get; set; }

        [XmlAttribute("action")]
        public string? Action { get; set; }
    }

    public class POSData
    {
        [XmlAttribute("id")]
        public string? Id { get; set; }
    }

    public class Result
    {
        [XmlAttribute("status")]
        public Status Status { get; set; }
    }

    public class ResponseMessage : Message
    {
        public ResponseMessage() => Type = MessageType.Response;

        [XmlElement("Result")]
        public Result? Result { get; set; }
    }


    [XmlRoot("Message")]
    public class HBRequestMessage : Message
    {

        [XmlElement("POS")]
        public POSData? POSData { get; set; }

        public HBRequestMessage()
        {
            Type = MessageType.Request;
            Action = "HB";
        }
    }


    [XmlRoot("Message")]
    public class HBResponseMessage : ResponseMessage
    {

        public HBResponseMessage() => Action = "HB";
    }



    public abstract class MessageDispatcher<TMessageType> where TMessageType : class
    {
        public abstract void RegisterHandler<TInputParam>(Func<TInputParam, Task> target);
        public abstract void RegisterHandler<TInputParam, TResult>(Func<TInputParam, Task<TResult>> target);
        public abstract Task<TMessageType?> Dispatch(TMessageType message);

    }


    public class XmlMessageDispatcher : MessageDispatcher<XDocument>
    {
        readonly List<(string matchExpression, Func<XDocument, Task<XDocument?>>)> _handlers = new List<(string matchExpression, Func<XDocument, Task<XDocument?>>)>();


        public override void RegisterHandler<TInputParam, TResult>(Func<TInputParam, Task<TResult>> target)
        {
            var routeAttribute = target.Method.GetCustomAttribute<XmlRouteAttribute>();
            if (routeAttribute == null)
                throw new Exception("Missing XmlRouteAttribute");

            RegisterHandler($"boolean({routeAttribute.XPath})", target);
        }

        public void RegisterHandler<TInputParam, TResult>(string matchExpression, Func<TInputParam, Task<TResult>> target)
        {

#pragma warning disable CS8603 // Possible null reference return.
            async Task<XDocument?> wrapper(XDocument param)
            {

                var result = await target(XmlFunctions.Deserialize<TInputParam>(param));

                if (result != null)
                    return XmlFunctions.Serialize(result);
                else
                    return null;
            }
#pragma warning restore CS8603 // Possible null reference return.

            _handlers.Add((matchExpression, wrapper));
        }

        public override void RegisterHandler<TInputParam>(Func<TInputParam, Task> target)
        {
            var routeAttribute = target.Method.GetCustomAttribute<XmlRouteAttribute>();
            if (routeAttribute == null)
                throw new Exception("Missing XmlRouteAttribute");

            RegisterHandler($"boolean({routeAttribute.XPath})", target);
        }

        public void RegisterHandler<TInputParam>(string matchExpression, Func<TInputParam, Task> target)
        {

#pragma warning disable CS8603 // Possible null reference return.
            async Task<XDocument?> wrapper(XDocument param)
            {
                await target(XmlFunctions.Deserialize<TInputParam>(param));
                return null;
            }
#pragma warning restore CS8603 // Possible null reference return.

            _handlers.Add((matchExpression, wrapper));
        }


        public override async Task<XDocument?> Dispatch(XDocument message)
        {
            foreach (var (m, t) in _handlers)
            {
                var result = message.XPathEvaluate(m);
                if (result is true)
                {
                    return await t(message);
                }
            }
            return null;
        }

    }

    public static class XmlFunctions
    {
        public static XDocument Serialize<T>(T instance) => Serialize(typeof(T), instance);
        public static XDocument Serialize(Type type, object? instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            using var ms = new MemoryStream();
            var xs = new XmlSerializer(type));
            xs.Serialize(ms, instance);
            ms.Flush();
            ms.Position = 0L;
            return XDocument.Load(ms);
        }

        public static T Deserialize<T>(XDocument xml) => (T)Deserialize(typeof(T), xml);
        public static object Deserialize(Type targetType, XDocument xml) => new XmlSerializer(targetType).Deserialize(new StringReader(xml.ToString()));
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class XmlRouteAttribute : Attribute
    {
        public string? XPath { get; }

        public XmlRouteAttribute(string xpath) => XPath = xpath;
    }


    class Program
    {
        static async Task Main(string[] args)
        {

            var hbMsg = new HBRequestMessage
            {
                Id = "123456789",
                POSData = new POSData { Id = "POS_99" }
            };

            var dispatcher = new XmlMessageDispatcher();

            //dispatcher.RegisterHandler<HBRequestMessage, HBResponseMessage>("boolean(/Message[@type='Request' and @action='HeartBeat'])", HandleMessage);
            dispatcher.RegisterHandler<HBRequestMessage, HBResponseMessage>(HandleMessage);
            dispatcher.RegisterHandler<HBResponseMessage>("boolean(/Message[@type='Response' and @action='HeartBeat'])", HandleMessage);

            var response = await dispatcher.Dispatch(Serialize(hbMsg));

            if (response != null)
                await dispatcher.Dispatch(response);

        }


        [XmlRoute("/Message[@type = 'Request' and @action = 'HeartBeat']")]
        static Task<HBResponseMessage> HandleMessage(HBRequestMessage message)
        {
            var response = new HBResponseMessage
            {
                Id = message.Id,
                Result = new Result { Status = Status.Success }
            };

            return Task.FromResult(response);

        }
        static Task HandleMessage(HBResponseMessage? message)
        {
            Console.WriteLine($"Received Response: {message?.Result?.Status}");
            return Task.CompletedTask;
        }



        static XDocument Serialize<T>(T msg)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var xs = new XmlSerializer(typeof(T));
            xs.Serialize(sw, msg);
            sw.Flush();
            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()), new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore });
            return XDocument.Load(xmlReader);
        }

    }
}
