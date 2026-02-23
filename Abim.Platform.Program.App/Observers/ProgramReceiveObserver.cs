using MassTransit;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Consumers.Observers
{
    /// <summary>
    /// Observer class for Program. Created by Alan Hummel
    /// </summary>
    /// <seealso cref="MassTransit.IReceiveObserver" />
    public class ProgramReceiveObserver : IReceiveObserver
    {
        /// <summary>
        /// NLog logger
        /// </summary>
        public static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// PreReceive
        /// </summary>
        /// <remarks>
        /// called after the message has been received and processed
        /// </remarks>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PreReceive(ReceiveContext context)
        {
            try
            {
                LogEventInfo logMsg = new LogEventInfo();
                logMsg.Message = "PreReceive: " + GetMessageBody(context);
                logMsg.Level = LogLevel.Info;
                //can test consumers here, e.g. with TestConsumer<AdministrationTestCreatedEventConsumer, Abim.Enterprise.Core.ServiceBus.Program.IAdministrationTestCreatedEvent>(context);
                
                Log.Log(logMsg);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in ReceiveObserver.PreReceive()");
            }
            
            return Task.FromResult(0);
        }
        
        /// <summary>
        /// PostReceive
        /// </summary>
        /// <remarks>
        /// called after the message has been received and processed
        /// </remarks>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostReceive(ReceiveContext context)
        {
            try
            {
                LogEventInfo logMsg = new LogEventInfo();
                logMsg.Message = "PostReceive: " + GetMessageBody(context);
                logMsg.Level = LogLevel.Info;
                
                Log.Log(logMsg);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in ReceiveObserver.PostReceive()");
            }
            
            return Task.FromResult(0);
        }
        
        /// <summary>
        /// PostConsume
        /// </summary>
        /// <remarks>
        /// called when the message was consumed, once for each consumer
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <returns></returns>
        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            try
            {
                LogEventInfo logMsg = new LogEventInfo();
                logMsg.Message = $"PostConsume: Message ID {context.MessageId} - {GetMessageBody<T>(context.Message)}";
                logMsg.Level = LogLevel.Info;
                logMsg.Properties.Add("MessageId", context.MessageId);
                
                Log.Log(logMsg);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in ReceiveObserver.PostConsume()");
            }
            
            return Task.FromResult(0);
        }
        
        /// <summary>
        /// ConsumeFault
        /// </summary>
        /// <remarks>
        /// called when the message is consumed but the consumer throws an exception
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="elapsed"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception)
            where T : class
        {
            try
            {                
                LogEventInfo logMsg = new LogEventInfo();
                logMsg.Message = $"Consume Fault: Message ID {context.MessageId} - {GetMessageBody<T>(context.Message)}";
                logMsg.Level = LogLevel.Warn;
                logMsg.Properties.Add("MessageId", context.MessageId);
                
                Log.Log(logMsg);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in ReceiveObserver.ConsumeFault()");
            }
            
            return Task.FromResult(0);
        }
        
        /// <summary>
        /// ReceiveFault
        /// </summary>
        /// <remarks>
        /// called when an exception occurs early in the message processing, such as deserialization, etc.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            try
            {
                LogEventInfo logMsg = new LogEventInfo();
                var commentary = "";
                if(exception.Message.Contains("Anonymous types are not valid message types"))
                    commentary = " This may occur if a Consumer attempts to send back a primitive or string in context.Respond().";
                logMsg.Message = $"Receive Fault: {exception.Message}.{commentary} Message: {GetMessageBody(context)}";
                logMsg.Level = LogLevel.Error;
                
                Log.Log(logMsg);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Error in ReceiveObserver.ReceiveFault()");
            }
            
            return Task.FromResult(0);
        }
        
        
        #region Private Methods
        
        /// <summary>
        /// Gets the message body.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private string GetMessageBody(ReceiveContext context)
        {
            //This method uses expensive objects. We couldn't find a better way.
            var serializer = new JsonSerializer();
            using(var stream = context.GetBody())
            {
                using(var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    using(var jsonTextReader = new JsonTextReader(reader))
                    {
                        var json = serializer.Deserialize(jsonTextReader);
                        if(json != null)
                            return json.ToString();
                        else
                            return "(COULD NOT DESERIALIZE MESSAGE BODY)";
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the message body.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        private string GetMessageBody<T>(T message)
        {
            return (message != null ? JsonConvert.SerializeObject(message) : "(MESSAGE WAS NULL)");
        }
                        
        #endregion Private Methods

    }
}
