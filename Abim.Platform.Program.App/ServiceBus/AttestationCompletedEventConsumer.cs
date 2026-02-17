using Abim.Platform.Program.App.Services;
using MassTransit;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Text;
using Abim.Enterprise.Core.ServiceBus.Attestation.Interface;
using System;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// Consumes an IAttestationCompletedEvent
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{IAttestationAttestorApproved}" />
    public class AttestationEventConsumer : IConsumer<IAttestationAttestorApproved>
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
        protected IProgramRulesService ProgramRulesService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AttestationAttestorApprovedEventConsumer"/> class.
        /// </summary>
        public AttestationEventConsumer(IProgramRulesService programRulesService)
        {
            ProgramRulesService = programRulesService;
        }

        /// <summary>
        /// Consumes Attestation Attestor Approved
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IAttestationAttestorApproved> context)
        {
            Log.Debug("AttestationCompletedEventConsumer.IAttestationCompletedEvent called with the following parameters: {0}", context.Message.Dump());
            var @event = context.Message;

            if (@event.MemberGuid == Guid.Empty)
            {
                Log.Error("No MemberGuid is provided");
                return;
            }
            await ProgramRulesService.RunCreateNewCredentials(@event.MemberGuid, @event.AttesteeAbimId, @event.TimeStamp);
        }

    }
}
