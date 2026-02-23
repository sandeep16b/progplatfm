using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Util.General.Json;
using Hangfire;
using Hangfire.Server;
using MassTransit;
using Newtonsoft.Json;
using NLog;
using System;
using System.ComponentModel;
using System.Threading;

namespace Abim.Platform.Program.WebApi.Util.General.Extensions
{
    /// <summary>
    /// This static class greatly simplifies the code needed to just publish a given event at a given DateTime. The example is only 5 lines; otherwise the
    /// caller would need to replicate almost this entire file.
    /// </summary>
    /// <example>
    /// <code language="C#" title="Example Usage">
    /// <![CDATA[
    ///
    ///    var @event = new SomethingHappenedEvent()
    ///    {
    ///         AdministrationId = administration.Id
    ///    };
    ///    DelayedEvent.Publish(@event, someDateTime); 
    ///
    /// ]]>
    /// </code>
    /// </example>
    /// </summary>
    [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public static class DelayedEvent
    {
        /// <summary>
        /// Publishes an event at a point in time.
        /// </summary>
        /// <param name="event">The event.</param>
        /// <param name="dateTime">The date time.</param>
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public static void Publish<TEvent>(TEvent @event, DateTime dateTime)
            where TEvent : class, new()
        {
            string dateString = string.Format("{0}-{1}-{2}", dateTime.Month, dateTime.Date, dateTime.Year);
            string jobId = string.Format("Delayed_{0}_{1}_{2}", @event.GetType().Name, dateString, Guid.NewGuid().ToString().Substring(0, 8));
            RecurringJob.AddOrUpdate<DelayedEventJob>(jobId, x => x.Execute<TEvent>(null, null, JsonConvert.ToString(@event)), dateTime.ToCron(true));
        }
    }

    /// <summary>
    /// A basic job to publish a delayed event
    /// </summary>
    [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public class DelayedEventJob
    {
        #region Fields

        /// <summary>
        /// The bus control backing field
        /// </summary>
        private IBusControl _busControl;
        
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the bus control.
        /// </summary>
        /// <value>
        /// The bus control.
        /// </value>
        private IBusControl BusControl
        {
            get
            {
                if(_busControl == null)
                {
                    _busControl = DependencyResolver.Container.GetInstance<IBusControl>();
                }
                return _busControl;
            }
            set
            {
                _busControl = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedEventJob"/> class.
        /// </summary>
        /// <param name="busControl">The bus control.</param>
        public DelayedEventJob(IBusControl busControl)
        {
            BusControl = busControl;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SagaEventConsumer"/> class.
        /// </summary>
        private DelayedEventJob()
        {
        }

        #endregion
        
        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="json">The json for the event.</param>
        [DisableConcurrentExecution(120)]
        [DisplayName("Job to Publish an Event")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void Execute<TEvent>(PerformContext context, IJobCancellationToken token, string json)
            where TEvent : class, new()
        {
            try
            {
                var @event = JsonData.Parse<TEvent>(json, true);
                BusControl.Publish(@event, default(CancellationToken));
            }
            catch(Exception ex)
            {
                Log.Error("An exception occurred during job processing: " + ex.Stringify());
                throw;      //Hangfire may want to be informed of this exception
            }
        }
    }
}
