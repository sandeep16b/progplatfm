using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Events;
using MassTransit;
using NLog;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// Consumes an ILngParticipationResultEvent
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{ILngParticipationResultEvent}" />
    public class LngParticipationResultConsumer : IConsumer<ILngParticipationResultEvent>
    {
        #region Properties

        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the program rules service.
        /// </summary>
        /// <value>
        /// The  program rules service.
        /// </value>
        protected ICredentialService CredentialService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LngParticipationResultConsumer"/> class.
        /// </summary>
        public LngParticipationResultConsumer(ICredentialService credentialService)
        {
            CredentialService = credentialService;
        }

        /// <summary>
        /// Consumes LNG Participation Result Event
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<ILngParticipationResultEvent> context)
        {

            Log.Info($"LngParticipationResultConsumer called for CredentialGuid: '{context.Message.CredentialGuid}'");
            var @event = context.Message;

            if (@event.CredentialGuid == Guid.Empty)
            {
                Log.Error("No CredentialGuid is provided");
                return;
            }

            //just to make sure they did not pass us 1/1/0001 12:00:00 AM as date in this field
            DateTime processingDate = @event.ProcessingDate.Equals(DateTime.MinValue) ? DateTime.Now : @event.ProcessingDate;

            /* PBI 181022 : Program Rule 63 - Longitudinal Assessment Due Date */
            await CredentialService.Handle(new UpdateLngAssessmentDueDateCommand()
            {
                CredentialId = @event.CredentialGuid,
                Year = @event.Year,
                IsSummativeDecisionYear = @event.IsSummativeDecisionYear,
                MetParticipationStatus = @event.MetParticipationStatus,
                PassSummativeDecision = @event.PassSummativeDecision,
                UserName = "LngParticipationResultConsumerUser"
            });
        }
    }
}