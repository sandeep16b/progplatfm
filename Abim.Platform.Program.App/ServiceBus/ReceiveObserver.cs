using CacheManager.Core.Logging;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// To observe received messages immediately after they are delivered by the transport,
    ///     create a class that implements the IReceiveObserver interface
    /// </summary>
    public class ReceiveObserver : IReceiveObserver
    {
        #region Properties

        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly NLog.ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Events

        /// <summary>
        /// called when an exception occurs early in the message processing, such as deserialization, etc.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            //Log error since Mass Transit only create one info record like this 'MOVE rabbitmq://rabbitmqdev.abim.org:5671/test/program_correctiveAction_queue N/A rabbitmq://rabbitmqdev.abim.org:5671/test/program_correctiveAction_queue_error?bind=true&queue=program_correctiveAction_queue_error Fault: Could not cast or convert from System.String to System.Guid. 
            Log.Error(exception);

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// called when the message was consumed, once for each consumer
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PostReceive(ReceiveContext context)
        {
            // this condition would be met when Interface messageType is wrong and cannot be delivered and cannot be determine it is faulted status
            if (!context.IsDelivered && !context.IsFaulted)
            {
                Log.Error($"Specified Interface messageType cannot be cast, InputAddress:'{context.InputAddress}' ");

                return Task.FromResult<object>(null);
            }
            return Task.FromResult<object>(null);
        }

        #endregion

        #region Not Used / Implemented Events

        /// <summary>
        /// called immediately after the message was delivery by the transport
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task PreReceive(ReceiveContext context)
        {
            // called immediately after the message was delivery by the transport
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// called when the message was consumed, once for each consumer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <returns></returns>
        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType)
            where T : class
        {
            // called when the message was consumed, once for each consumer
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// called when the message is consumed but the consumer throws an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="elapsed"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan elapsed, string consumerType, Exception exception) where T : class
        {
            // Mass Transit would create 2 log records (Error first then Info message)
            return Task.FromResult<object>(null);
        }

        #endregion
    }
}
