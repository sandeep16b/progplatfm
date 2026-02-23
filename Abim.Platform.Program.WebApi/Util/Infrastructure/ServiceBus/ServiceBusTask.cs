using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Util.Infrastructure.ServiceBus
{
    /// <summary>
    /// This class blocks a thread until an event is received by a consumer. Only for the rare cases in which we need this (don't abuse it)
    /// </summary>
    public static class ServiceBusTask
    {
        /// <summary>
        /// The lookup by messageId
        /// </summary>
        private static Dictionary<Guid, WaitTask> Lookup = new Dictionary<Guid, WaitTask>();

        /// <summary>
        /// The received responses
        /// </summary>
        private static Dictionary<Guid, Object> Receivals = new Dictionary<Guid, Object>();

        /// <summary>
        /// Blocks the current thread until a specific Service Bus event has been received
        /// </summary>
        /// <typeparam name="TReplyEvent">The type of the reply event.</typeparam>
        /// <param name="messageId">The message correlation identifier, to ensure that the reply we get is for this particular request</param>
        /// <param name="timeoutSeconds">The timeout seconds, if any</param>
        /// <returns></returns>
        public static TReplyEvent WaitForReply<TReplyEvent>(Guid messageId, int? timeoutSeconds = 30)
        {
            if(Receivals.ContainsKey(messageId))
            {
                var response = (TReplyEvent)(Receivals[messageId]);
                Receivals.Remove(messageId);
                return response;
            }
            var waitTask = new WaitTask();
            Lookup[messageId] = waitTask;
            if(timeoutSeconds != null)
                waitTask.SetTimeout<TReplyEvent>(timeoutSeconds.Value);
            waitTask.Wait();
            return waitTask.GetResult<TReplyEvent>();
        }

        /// <summary>
        /// Informs that an awaited reply has been received
        /// </summary>
        /// <typeparam name="TReplyEvent">The type of the reply event.</typeparam>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="event">The event.</param>
        /// <exception cref="System.Exception"></exception>
        public static void ReplyReceived<TReplyEvent>(Guid messageId, TReplyEvent @event)
        {
            WaitTask transaction = null;
            if(Lookup.ContainsKey(messageId))
                transaction = Lookup[messageId];
            if(transaction != null)
            {
                try
                {
                    transaction.SetResult(@event);
                }
                catch(Exception ex)
                {
                    //doesn't matter
                }
            }
            else Receivals[messageId] = @event;
        }
    }
    
    /// <summary>
    /// Holds a TaskCompletionSource and some metadata associated with the wait operation that's using it
    /// </summary>
    internal class WaitTask
    {        
        /// <summary>
        /// Gets or sets the task completion source.
        /// </summary>
        /// <value>
        /// The task completion source.
        /// </value>
        internal TaskCompletionSource<Object> TaskCompletionSource { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WaitTask"/> has completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if completed; otherwise, <c>false</c>.
        /// </value>
        internal bool Completed { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="WaitTask"/> has succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        internal bool Succeeded { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTask"/> class.
        /// </summary>
        internal WaitTask()
        {
            TaskCompletionSource = new TaskCompletionSource<Object>();
        }
                
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitTask"/> class.
        /// </summary>
        /// <param name="completionSource">The completion source.</param>
        internal WaitTask(TaskCompletionSource<Object> completionSource)
        {
            TaskCompletionSource = completionSource;
        }
        
        /// <summary>
        /// Waits for the task completion source to get a result. Blocks the thread
        /// </summary>
        internal void Wait()
        {
            TaskCompletionSource.Task.Wait();
        }
        
        /// <summary>
        /// Sets a timeout.
        /// </summary>
        /// <typeparam name="TResultEvent">The type of the result event.</typeparam>
        /// <returns></returns>
        internal async Task SetTimeout<TResultEvent>(int seconds)
        {
            SetTimeout(seconds, default(TResultEvent));
        }

        /// <summary>
        /// Sets a timeout, and delays until it's finished by putting the thread to sleep and then, on waking, setting the default value and 'false'
        /// unless during that time a real result came in.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        internal async Task SetTimeout(int seconds, Object defaultValue)
        {
            await Task.Delay(seconds * 1000);
            if(!Completed)
            {
                Completed = true;
                Succeeded = false;
                TaskCompletionSource.SetResult(defaultValue);
            }
        }

        /// <summary>
        /// Sets a result on the task completion source.
        /// </summary>
        /// <param name="value">The value.</param>
        internal void SetResult(Object value)
        {
            if(Completed)
                throw new Exception("The WaitTask has already been completed");
            Completed = true;
            Succeeded = true;
            TaskCompletionSource.SetResult(value);
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <typeparam name="TReplyEvent">The type of the reply event.</typeparam>
        /// <returns></returns>
        internal TReplyEvent GetResult<TReplyEvent>()
        {
            if(!Completed)
                throw new Exception("No result exists yet because Completed is false");
            return (TReplyEvent)(TaskCompletionSource.Task.Result);
        }
    }
}
