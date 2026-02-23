extern alias SharedOldServiceBus;

using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Platform.Program.App.Classes;
using Abim.Platform.Program.App.DTOs;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Core.Identity;
using Hangfire;
using Hangfire.Server;
using MassTransit;
using NLog;
using SharedOldServiceBus::Abim.Enterprise.Core.ServiceBus.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using static Abim.Platform.Program.App.Util.Constants;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// A Hangfire child job for processing certificate deselection
    /// </summary>
    public class DeselectCertificateChildJob : IDeselectCertificateChildJob
    {
        private ICredentialService _credentialService;
        private IBusControl _bus;
        private IProfileInterservice _profileInterservice;
        private IAccessTokenService _accessTokenService;
        private string _profileHostUrl;
        private static ILogger Log = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Creates a new instance of DeSelectCertificateChildJob
        /// </summary>
        /// <param name="credentialService">An instance of ICredentialService required by the job</param>
        /// <param name="bus">An instance of IBusControl with which to publish messages</param>
        /// <param name="profileInterservice">An instance of IProfileInterservice to be used to retrieve profile information</param>
        /// <param name="profileHostUrl">The URL of the profile host</param>
        /// <param name="accessTokenSingletonWraper">An instance of IAccessTokenService with which to retrieve an access token for interservice calls</param>
        public DeselectCertificateChildJob(
            ICredentialService credentialService, 
            IBusControl bus, 
            IProfileInterservice profileInterservice,
            string profileHostUrl,
            IAccessTokenService accessTokenSingletonWraper)

        {
            _credentialService = credentialService ?? throw new ArgumentNullException("credentialService");
            _bus = bus ?? throw new ArgumentNullException("bus");
            _profileInterservice = profileInterservice ?? throw new ArgumentNullException("profileInterservice");

            if (string.IsNullOrWhiteSpace(profileHostUrl))
                throw new ArgumentException("profileHostUrl is required.", profileHostUrl);
            else
                _profileHostUrl = profileHostUrl;

            _accessTokenService = accessTokenSingletonWraper ?? throw new ArgumentNullException("accessTokenSingletonWraper");
        }

        /// <summary>
        /// Executes the job to deselect the specified certificates for a particular diplomate
        /// </summary>
        /// <param name="memberId">The ID of the diplomate these credentials belong to</param>
        /// <param name="credentialsInfo">Information about the credentials to be deselected</param>
        /// <param name="deselectionEffectiveDate">The de-selection effective date to process de-selection for</param>
        /// <param name="context">A Hangfire PerformContext to be used by the job</param>
        /// <param name="cancellationToken">An IJobCancellationToken to be used to cancel the job if running</param>
        public void ExecuteChild(Guid memberId, List<CredentialInfoDTO> credentialsInfo, DateTime deselectionEffectiveDate, PerformContext context, IJobCancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                DeselectCertificates(credentialsInfo.Select(info => info.ExternalId).ToList(), deselectionEffectiveDate);
                
                //If any of these credentials had been active, we should send emails 
                //informing the diplomate of their deactivation
                if (credentialsInfo.Any(info => info.IsActive))
                    SendEmailNotificationOfDeselect(memberId, credentialsInfo);
            }
            catch (OperationCanceledException canceledEx)
            {
                Log.Error($"DeselectCertificateChildJob.ExecuteChild cancelled for member {memberId}, de-selection effective date {deselectionEffectiveDate}.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"DeselectCertificateChildJob.ExecuteChild failed for member {memberId}, de-selection effective date {deselectionEffectiveDate}, with exception \"{ex.Message}\".");
                throw;
            }
        }

        private void DeselectCertificates(List<Guid> credentialIds, DateTime deselectionEffectiveDate)
        {
            foreach(var credentialId in credentialIds)
            {
                Log.Info($"Creating command to deselect credential {credentialId} for deselection effective date {deselectionEffectiveDate}.");

                var command =
                    new DeselectCertificateCommand
                    {
                        CredentialId = credentialId,
                        ExpiredDate = deselectionEffectiveDate,
                        Username = "DeselectCertificateChildJob"
                    };

                DeselectCertificateCommandResult result =
                    (DeselectCertificateCommandResult)_credentialService.Handle(command);

                if (!result.Succeeded)
                    throw new ApplicationException($"Failed to deselect credential {command.CredentialId}: {result.FailureReason}.");
            }
        }

        private ProfileNestedResource GetProfileInfo(Guid memberId)
        {
            string accessToken = _accessTokenService.GetAccessToken();
            var profile = _profileInterservice.GetProfileById(accessToken, _profileHostUrl, memberId).Result;
            return profile;
        }

        private void SendEmailNotificationOfDeselect(Guid memberId, List<CredentialInfoDTO> credentialsInfo)
        {
            //We should only send email for credentials which were deactivated
            var credsWeDeactivated = credentialsInfo.Where(info => info.IsActive).ToList();
            var certNames = credsWeDeactivated.Select(info => info.CertificateName).ToList();
            var profile = GetProfileInfo(memberId);
            var notificationEvent = new NotificationEvent()
            {
                TemplateExternalKey = TriggeredCommunicationTemplateExternalKey.DeactivateCertification,
                EmailAddress = profile.EmailAddress.EmailAddress,
                Parameters = new Dictionary<string, string>
                {
                    { "LastName", profile.Name.LastName }, 
                    { "CertificationNames", GetDelimitedCertNames(certNames, "<br />", true) }, 
                    { "CertificationNames_TV", GetDelimitedCertNames(certNames, ", ", false) }, 
                    { "SubscriberKey", profile.EmailAddress.EmailAddress }, 
                    { "IID", profile.AbimId }
                }
            };

            Log.Info($"Publishing NotificationEvent for deselection email for member {memberId}.");

            _bus.Publish(notificationEvent);
        }

        private string GetDelimitedCertNames(List<string> certNames, string delimiter, bool addDelimiterToEnd)
        {
            return string.Join(delimiter, certNames) + (addDelimiterToEnd ? delimiter : "");
        }
    }
}
