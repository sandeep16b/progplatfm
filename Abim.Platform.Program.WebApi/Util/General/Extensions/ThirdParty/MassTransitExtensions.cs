using Abim.Platform.Program.Util;
using Abim.Platform.Program.Util.Extensions;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class MassTransitExtensions
    {
        /// <summary>
        /// Gets the base address.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <returns></returns>
        public static string BaseAddress(this IBusControl bus)
        {
            var uri = string.Join("/", bus.Address.AbsoluteUri.Split('/').WithoutLast().ToArray());
            return uri;
        }
        
        /// <summary>
        /// Sends to the specified queue.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static void Send(this IBusControl bus, string queueName, Object value)
        {
            var uri = BaseAddress(bus);
            var endpointTask = bus.GetSendEndpoint(new Uri(uri + "/" + queueName));
            endpointTask.Wait();
            var sendTask = endpointTask.Result.Send(value);
            sendTask.Wait();
        }

        /// <summary>
        /// Sends to the specified queue.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static async Task<bool> SendAsync(this IBusControl bus, string queueName, Object value)
        {
            var uri = BaseAddress(bus);
            var endpoint = await bus.GetSendEndpoint(new Uri(uri + "/" + queueName)).ConfigureAwait(false);
            await endpoint.Send(value).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Publishes and logs the published event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="bus">The bus.</param>
        /// <param name="event">The event.</param>
        /// <param name="log">The log.</param>
        public static async Task PublishAndLog<TEvent>(this IBusControl bus, TEvent @event, global::NLog.ILogger log)
            where TEvent : class
        {
            if(@event == null)
                throw new ArgumentException("PublishAndLog() was passed a null event");
            await bus.Publish(@event).ConfigureAwait(false);
            if(log == null)
                throw new ArgumentException("PublishAndLog() was passed a null ILogger");
            
            try
            {
                log.Debug("Published " + @event.GetType().Name + " on RabbitMQ platform " + bus.BaseAddress() + ": " + @event.JsonFormat());
            }
            catch(Exception ex)
            {
                if(log.GetType().Namespace.Contains("Moq")) return;
                try
                {
                    log.Error("Failed to log a Publish: " + ex.Stringify());
                }
                #pragma warning disable 0168
                catch(Exception ex2)
                {
                    //ignore
                }
            }
        }
    }
}
