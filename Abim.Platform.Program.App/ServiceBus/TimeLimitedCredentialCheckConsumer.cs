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
    internal class TimeLimitedCredentialCheckConsumer : IConsumer<IRunRuleChecksServiceBusCommand>
    {
        #region Fields

        /// <summary>
        /// Used for the ModifiedBy audit field of an Issuances
        /// </summary>
        private const string ConsumerUsername = "MBMforTL";

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
        /// Initializes a new instance of the <see cref="TimeLimitedCredentialCheckConsumer"/> class.
        /// </summary>
        public TimeLimitedCredentialCheckConsumer(ICredentialService credentialService, IProgramRulesService programRulesService)
        {
            CredentialService = credentialService;
            ProgramRulesService = programRulesService;
        }

        /// <summary>
        /// Consumes the event
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IRunRuleChecksServiceBusCommand> context)
        {
            var @event = context.Message;
            var memberId = @event.MemberId;
            DateTime startDate = new DateTime();

            //check if any value is pass to use, otherwise used current date
            DateTime processingDate = @event.ProcessingDate.HasValue ? @event.ProcessingDate.Value : DateTime.Now;

            // compare to bulk processing date. If it is after then we need to set it to the end of that window
            // It is requirement in pbi 73194
            if (DateTime.Compare(processingDate.Date, new DateTime(processingDate.Year, 9, 1).Date) > 0)
                startDate = new DateTime(processingDate.Year, 12, 31);
            else
                startDate = processingDate;

            // find all certificate that a person holds that are currently time limited (Duration = 'Timelimited') with active status (IssuanceStatus = 'Active' )
            // and Expiration is less of equal to startDate ( CredentialId, IssuanceId)
            IEnumerable<Tuple<Guid, int>> expiringCredentials = CredentialService.GetExpiredCredentials(startDate, memberId);

            foreach (var credential in expiringCredentials)
            {
                var command = new RunRulesForMustBeMaintainedCertificateCommand
                {
                    CredentialId = credential.Item1,
                    IssuanceId = credential.Item2,
                    EventDate = @event.EventDate,
                    CreatedBy = ConsumerUsername // per PBI
                };

                ProgramRulesService.HandleJob(command);
                //BackgroundJob.Enqueue<ExpireTLChildJob>(x => x.ExecuteChild(command.CredentialId, command, null, null));
            }
        }
    }
}