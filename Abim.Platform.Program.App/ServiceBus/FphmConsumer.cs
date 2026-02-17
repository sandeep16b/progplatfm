using Abim.Enterprise.Core.ServiceBus.Program.Interface;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using MassTransit;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Enterprise.Core.Resource.Program;
using NLog;
using Abim.Enterprise.Core.Relational.Validation;
using Abim.Platform.Program.App.Services.CommandResults;
using Hangfire;
using Abim.Platform.Program.App.HangFireJobs;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// Consumes an IRunCorrectiveActionCheckCommand
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{IRunRulesChecksServiceBusCommand}" />
    internal class FphmConsumer : IConsumer<IRunRuleChecksServiceBusCommand>
    {
        #region Fields

        /// <summary>
        /// Used for the ModifiedBy audit field of an Issuances
        /// </summary>
        private const string ConsumerUsername = "NewFP";

        #endregion

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
        /// Gets or sets the program Rules service.
        /// </summary>
        /// <value>
        /// The program rules service.
        /// </value>
        protected IProgramRulesService ProgramRulesService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FphmConsumer"/> class.
        /// </summary>
        public FphmConsumer(ICredentialService credentialService, IProgramRulesService programRulesService)
        {
            CredentialService = credentialService;
            ProgramRulesService = programRulesService;
        }

        /// <summary>
        ///  Consumes the event
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IRunRuleChecksServiceBusCommand> context)
        {
            var @event = context.Message;

            //check if any value is pass to use, otherwise used current date
            DateTime processingDate = @event.ProcessingDate.HasValue ? @event.ProcessingDate.Value : DateTime.Now;

            //to-do: check for this Requirement: The certificate status of the IM certificate is Active or Expired (eg. not revoked/surrendered/suspended)

            var command = new FPHMCredentialsCommand
            {
                MemberId = @event.MemberId,
                EventDate = @event.EventDate,
                CreatedBy = "NewFP"
            };

            ProgramRulesService.HandleJob(command);
        }
    }
}