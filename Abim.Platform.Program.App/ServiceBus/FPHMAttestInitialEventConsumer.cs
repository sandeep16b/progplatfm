using Abim.Platform.Program.App.Services;
using MassTransit;
using System;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Text;
using Abim.Enterprise.Core.ServiceBus.Program;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// Consumes an IFPHMAttestInitialEvent
    /// </summary>
    /// <seealso cref="MassTransit.IConsumer{IFPHMAttestInitialEvent}" />
    public class FPHMAttestInitialEventConsumer : IConsumer<IFPHMAttestInitialEvent>
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
        /// Initializes a new instance of the <see cref="FPHMAttestInitialEventConsumer"/> class.
        /// </summary>
        public FPHMAttestInitialEventConsumer(IProgramRulesService programRulesService)
        {
            ProgramRulesService = programRulesService;
        }

        /// <summary>
        /// Consumes Registration Exam Result Event
        /// When an abim physician passes an initial certification exam, a new credential/issuance should be created
        /// PBI 94844:back end process: create credential/issuance on passing initial cert
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IFPHMAttestInitialEvent> context)
        {
            Log.Info($"FPHMAttestInitialEventConsumer.FPHMAttestInitialEvent called with the following parameters: {context.Message.Dump()}");
            var @event = context.Message;

            if (@event.MemberId == Guid.Empty)
            {
                Log.Error("No MemberId is provided");
                return;
            }

            //just to make sure they did not pass us 1/1/0001 12:00:00 AM as date in this field
            DateTime processingDate = @event.ProcessingDate.Equals(DateTime.MinValue) ? DateTime.Now : @event.ProcessingDate;

            await ProgramRulesService.RunCreateNewCredentials(@event.MemberId, @event.ProcessingDate);
        }

    }
}