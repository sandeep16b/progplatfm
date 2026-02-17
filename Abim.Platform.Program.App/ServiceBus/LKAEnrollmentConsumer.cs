using ServiceBus.Events;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// A consumer for IMemberEnrolled objects
    /// </summary>
    public class LKAEnrollmentConsumer : IConsumer<IMemberEnrolled>
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

        #endregion Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="credentialService">An instances of ICredentialService to use</param>
        public LKAEnrollmentConsumer(ICredentialService credentialService) 
        {
            CredentialService = credentialService ?? throw new ArgumentNullException(nameof(credentialService));
        }

        /// <summary>
        /// The method for consuming IMemberEnrolled objects
        /// </summary>
        /// <param name="context">A MassTransit ConsumeContext</param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IMemberEnrolled> context)
        {
            try
            {
                Log.Info($"LKAEnrollmentConsumer.Consume() called with the following parameters: {context.Message.Dump()}");
                var @event = context.Message;

                if (@event.CredentialId == Guid.Empty)
                {
                    Log.Error("No CredentialId is provided.");
                    return;
                }

                var updatePathwayCommand = GetUpdatePathwayCommand(@event.CredentialId);
                await CredentialService.Handle(updatePathwayCommand);
                Log.Info($"LKAEnrollmentConsumer.Consume() updated pathway to LNG for credential {updatePathwayCommand.CredentialId}.");

                var markForSelectOrDeselectCommand = GetMarkCertificatesForSelectOrDeselectCommand(@event.MemberId, @event.CredentialId);
                CredentialService.Handle(markForSelectOrDeselectCommand);
                Log.Info($"LKAEnrollmentConsumer.Consume() marked credential {@event.CredentialId} for selection.");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private UpdatePathwayCommand GetUpdatePathwayCommand(Guid credentialId)
        {
            var command = new UpdatePathwayCommand();
            command.CredentialId = credentialId;
            command.Pathway = Resources.PathwayType.LNG;
            command.UserInfo = new WebApi.Authentication.UserInfo { Username = "LKAEnrollmentConsumer" };
            return command;
        }

        private MarkCertificatesForSelectOrDeselectCommand GetMarkCertificatesForSelectOrDeselectCommand(Guid memberId, Guid credentialId)
        {
            var command = new MarkCertificatesForSelectOrDeselectCommand()
            {
                MemberId = memberId,
                CredentialIdsForSelect = new List<Guid>() { credentialId },
                UserInfo = new WebApi.Authentication.UserInfo()
                {
                    IsAdmin = false,
                    ProfileId = memberId,
                    Username = "System"
                }
            };
            return command;
        }
    }
}
