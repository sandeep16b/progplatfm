using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using Abim.Platform.Product.Resources.Enums;
//using Abim.Enterprise.Core.ServiceBus.Registration.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Enums;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// IProgramRulesService interface.  
    /// </summary>
    public partial interface IProgramRulesService 
    {
        /// <summary>
        /// Handles the job.
        /// </summary>
        /// <param name="command">The command.</param>
        void HandleJob(RunRulesForMustBeMaintainedCertificateCommand command);

        /// <summary>
        /// Runs the corrective action. (Main function)
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        Task<bool> RunCorrectiveAction(List<Credential> credentials, 
                                        Guid memberId, 
                                        DateTime eventDate, 
                                        DateTime processingDate, 
                                        TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn,
                                        RegistrationData registration = null);

        /// <summary>
        /// Runs the corrective action for activity.
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <returns></returns>
        Task<bool> RunCorrectiveActionForActivity(Guid activityId, DateTime processingDate, TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn);

        /// <summary>
        /// RunCorrectiveActionForMember
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        Task<bool> RunCorrectiveActionForMember(
                                    Guid memberId, 
                                    DateTime eventDate, 
                                    DateTime processingDate, 
                                    TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn,
                                    RegistrationData registration = null);

        /// <summary>
        /// Runs the corrective action for new issuance.
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="memberId"></param>
        /// <param name="issuanceDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <returns></returns>
        Task<bool> RunCorrectiveActionForNewIssuance(Guid credentialId,Guid memberId, DateTime issuanceDate, DateTime processingDate, TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="processingDate"></param>
        /// <param name="examRegistration"></param>
        /// <returns></returns>
        Task RunProcessesOnExamResultEvent(Guid registrationId,
                                            DateTime processingDate,
                                            ExamRegistrationType examRegistration);

        /// <summary>
        /// creates an FPHM credential for the member if it does not already exist.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        Task RunCreateNewCredentials(Guid memberId, DateTime processingDate);

        /// <summary>
        /// HandleEarlyYearEndLookbackChildJob
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lookbackWindowType"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        Task HandleEarlyYearEndLookbackChildJob(Guid memberId,
                                                WindowsIntervalType? lookbackWindowType,
                                                DateTime processingDate);
        /// <summary>
        /// Runs the Year End Lookback process
        /// </summary>
        /// <param name="memberId">The GUID ID of the user that Year End Lookback is being run for</param>
        /// <param name="eventDate">The date for which Year End Lookback is being run (for example, 12/31/2018)</param>
        /// <param name="processingDate">The date of the Year End Lookback run</param>
        /// <returns></returns>
        Task<bool> RunYearEndLookback(Guid memberId, DateTime eventDate, DateTime processingDate);

        /// <summary>
        /// Runs the CoSponsored LockOut process
        /// </summary>
        /// <param name="credentialId">The GUID ID of the credential that CoSponsored LockOut is being run for</param>
        /// <param name="lockOutDate">The date for which CoSponsored LockOut is being run (for example, 12/31/2018)</param>
        /// <param name="processingDate">The date of the CoSponsored LockOut run</param>
        /// <returns></returns>
        Task<bool> RunCoSponsoredLockOut(Guid credentialId, DateTime lockOutDate, DateTime processingDate);
    }
}
