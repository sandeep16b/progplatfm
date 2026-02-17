using Abim.Enterprise.Core.ServiceBus.Product;
using Abim.Enterprise.Core.ServiceBus.Program;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Util;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// Consumes an IActivityCompletedEvent, IExamPassedEvent, IIssuanceCreatedEvent
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{IRunRulesChecksServiceBusCommand}" />
    public class CorrectiveActionConsumer  : IConsumer<IIssuanceCreatedEvent> , IConsumer<IActivityCompletedEvent>, IConsumer<IActivityCreatedEvent>
    {
        #region Properties
        
        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the credential service.
        /// </summary>
        /// <value>
        /// The credential service.
        /// </value>
        protected ICredentialService CredentialService { get; set; }

        /// <summary>
        /// Gets or sets the program rules service.
        /// </summary>
        /// <value>
        /// The  program rules service.
        /// </value>
        protected IProgramRulesService ProgramRulesService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectiveActionConsumer"/> class.
        /// </summary>
        public CorrectiveActionConsumer(ICredentialService credentialService, 
                                    IProgramRulesService programRulesService)
        {
            CredentialService = credentialService;
            ProgramRulesService = programRulesService;
        }

        /// <summary>
        /// Consumes Activity Completed Event ('Complete Reciprocity Attestation', 'Earn MOC Point's, 'Complete ICard or FPHM attestation')
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IActivityCompletedEvent> context)
        {
            Log.Info($"CorrectiveActionConsumer.IActivityCompletedEvent called with the following parameters: {context.Message.Dump()}");
            var @event = context.Message;

            // find processing Date
            DateTime processingDate = @event.ProcessingDate.HasValue && !@event.ProcessingDate.Equals(DateTime.MinValue) ? @event.ProcessingDate.Value : DateTime.Now;

            if (@event.MemberId != Guid.Empty && @event.EventDate != default(DateTime))
                await ProgramRulesService.RunCorrectiveActionForMember(@event.MemberId, @event.EventDate, processingDate, TriggeringEvent.ActivityCompleted);
            else if (@event.ActivityId!=Guid.Empty)
                await ProgramRulesService.RunCorrectiveActionForActivity(@event.ActivityId, processingDate, TriggeringEvent.ActivityCompleted);  
            else
                throw new ArgumentNullException();

        }

        /// <summary>
        /// Consumes Exam pass or fail
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IActivityCreatedEvent> context)
        {
            Log.Info($"CorrectiveActionConsumer.IActivityCreatedEvent called with the following parameters: {context.Message.Dump()}");
            var @event = context.Message;

            DateTime processingDate = @event.ProcessingDate.HasValue && !@event.ProcessingDate.Equals(DateTime.MinValue) ? @event.ProcessingDate.Value : DateTime.Now;

            if (@event.MemberId != Guid.Empty && @event.EventDate != default(DateTime))
                await ProgramRulesService.RunCorrectiveActionForMember(@event.MemberId, @event.EventDate, processingDate, TriggeringEvent.ActivityCreated);
            else if (@event.ActivityId != Guid.Empty)
                await ProgramRulesService.RunCorrectiveActionForActivity(@event.ActivityId, processingDate, TriggeringEvent.ActivityCreated);

        }

        /// <summary>
        /// Consumes new Issuane Create Event ('Earn new credential from ABIM')
        /// Most of the time deplomate earns credential/issuance by passing exam (by ExamResultEvent event)
        /// in case they earned by sitting on some boards they would not take any exam.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IIssuanceCreatedEvent> context)
        {
            Log.Info($"CorrectiveActionConsumer.IIssuanceCreatedEvent called with the following parameters: {context.Message.Dump()}");
            var @event = context.Message;

            DateTime processingDate = @event.ProcessingDate.HasValue ? @event.ProcessingDate.Value : DateTime.Now;

            await ProgramRulesService.RunCorrectiveActionForNewIssuance(@event.CredentialId, @event.MemberId, @event.IssuanceDate, processingDate, TriggeringEvent.IssuanceCreated);
        }

    }
}