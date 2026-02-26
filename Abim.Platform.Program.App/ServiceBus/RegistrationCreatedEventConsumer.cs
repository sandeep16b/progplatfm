using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Core.Identity;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// RegistrationCreatedEventConsumer
    /// </summary>
    public class RegistrationCreatedEventConsumer : IConsumer<IRegistrationCreatedEvent>
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
        /// AccessTokenService
        /// </summary>
        protected IAccessTokenService AccessTokenService { get; set; }

        /// <summary>
        /// Gets or sets the Registration Interservice.
        /// </summary>
        /// <value>
        /// The Registration Interservice
        /// </value>
        protected IRegistrationInterservice RegistrationInterservice { get; set; }


        /// <summary>
        /// AccessToke for interservices comuncation
        /// </summary>
        private string AccessToken
        {
            get
            {
                return AccessTokenService.GetAccessToken();
            }
        }

        #endregion

        /// <summary>
        /// RegistrationCreatedEventConsumer
        /// </summary>
        /// <param name="credentialService"></param>
        /// <param name="registrationInterservice"></param>
        /// <param name="accessTokenSingleton"></param>
        public RegistrationCreatedEventConsumer(ICredentialService credentialService, IRegistrationInterservice registrationInterservice, IAccessTokenService accessTokenSingleton )
        {
            CredentialService = credentialService;
            AccessTokenService = accessTokenSingleton;
            RegistrationInterservice = registrationInterservice;
        }
        /// <summary>
        /// Consume
        /// </summary>
        /// <param name="context"></param> 
        public async Task Consume(ConsumeContext<IRegistrationCreatedEvent> context)
        {
            try
            {
                Log.Info($"RegistrationCreatedEventConsumer.Consume() called with the following parameters: {context.Message.Dump()}");
                var @event = context.Message;

                var certificationId = GetCertificationIdFromRegistration(@event.RegistrationId).Result;
                if (certificationId == Guid.Empty)
                {
                    Log.Error($"Could not get CertificationId using RegistrationId {@event.RegistrationId} from event.");
                    return;
                }

                var credentialId = GetCredentialIdForCertification(@event.MemberId, certificationId);
                if (credentialId == Guid.Empty)
                {
                    Log.Info($"No credential found for MemberId {context.Message.MemberId} with CertificationId {certificationId}. No Certificate to be marked for selection.");
                    return;
                }

                var command = GetMarkCertificatesForSelectOrDeselectCommand(@event.MemberId, credentialId);
                CredentialService.Handle(command);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        /// <summary>
        /// GetCertificationIdFromRegistration
        /// </summary>
        /// <param name="registrationId"></param> 
        private async Task<Guid> GetCertificationIdFromRegistration(Guid registrationId)
        {

            var registration = await RetryHelper.RetryTask(() => RegistrationInterservice.GetRegistrationById(AccessToken, registrationId), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

            if (registration != null)
            {
                return registration.CertificationId;
            }
            return Guid.Empty;
        }
        /// <summary>
        /// GetCredentialIdForCertification
        /// </summary>
        /// <param name="memberId"></param> 
        /// <param name="certificationId"></param> 
        private Guid GetCredentialIdForCertification(Guid memberId, Guid certificationId)
        {
            var issuances = CredentialService.GetIssuancesForMemberId(memberId).ToList();

            if (issuances.Any())
            {
                var issuance = issuances
                                    .OrderByDescending(x => x.IssuanceDate)
                                    .FirstOrDefault(x => x.Credential.Certification.ExternalId == certificationId);

                if (issuance != null)
                {
                    return issuance.Credential.ExternalId;
                }
            }
            return Guid.Empty;
        }
        /// <summary>
        /// GetMarkCertificatesForSelectOrDeselectCommand
        /// </summary>
        /// <param name="memberId"></param> 
        /// <param name="credentialId"></param> 
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
