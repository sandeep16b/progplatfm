using Abim.Platform.Product.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// Program Rules Service
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.App.Services.IProgramRulesService" />
    public partial class ProgramRulesService
    {
        #region Corrective Action

        /// <summary>
        /// RunCorrectiveAction for input Activity Id
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <returns></returns>
        public async Task<bool> RunCorrectiveActionForActivity(Guid activityId, DateTime processingDate, TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn)
        {
            try
            {
                // get activity details for given activityId
                var activity = await RetryHelper.RetryTask(() => ProductInterservice.GetActivityById(AccessToken, activityId), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

                if (activity == null)
                {
                    Log.Error(ErrorMessages.NotFound(typeof(ActivityResource), activityId));

                    throw new Exception(ErrorMessages.NotFound(typeof(ActivityResource), activityId));
                }

                // if CompletedDate is null then we don't need to run corrective actions
                if (!activity.CompletedDate.HasValue)
                    return true;

                MemberId = activity.MemberId;

                // get all credentials for given memberId
                var credentials = Credentials.ToList();

                if (credentials.Count == 0)
                {
                    Log.Info($"No credentials were found for MemberId:'{MemberId}'");
                    return false;
                }

                return await RunCorrectiveAction(credentials, MemberId, activity.CompletedDate.Value, processingDate, triggeringEvent);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        public async Task<bool> RunCorrectiveActionForMember(Guid memberId,
                                                            DateTime eventDate,
                                                            DateTime processingDate,
                                                            TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn,
                                                            RegistrationData registration = null)
        {
            try
            {

                // get all credentials for given memberId
                MemberId = memberId;
                var credentials = Credentials.ToList();

                if (credentials.Count == 0)
                {
                    Log.Info($"No credentials were found for MemberId:'{memberId}'");
                    return false;
                }

                return await RunCorrectiveAction(credentials, memberId, eventDate, processingDate, triggeringEvent, registration);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="memberId"></param>
        /// <param name="IssuanceDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <returns></returns>
        public async Task<bool> RunCorrectiveActionForNewIssuance(Guid credentialId,
                                                                  Guid memberId,
                                                                  DateTime IssuanceDate,
                                                                  DateTime processingDate,
                                                                  TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn)
        {
            try
            {

                // get all credentials for given memberId
                MemberId = memberId;
                var credentials = Credentials.ToList();

                if (credentials.Count == 0)
                {
                    Log.Info($"No credentials were found for MemberId:'{memberId}'");
                    return false;
                }

                var issuance = Credentials.Where(c => c.ExternalId == credentialId)
                               .FirstOrDefault()
                               .Issuances
                               .Where(i => DateTime.Compare(i.IssuanceDate.Date, IssuanceDate.Date) == 0)
                               .OrderByDescending(d => d.IssuanceDate)
                               .FirstOrDefault();

                if (issuance == null)
                {
                    Log.Warn($"No Issuance was found for CredentialId:'{credentialId}' and IssuanceDate:'{IssuanceDate.ToShortDateString()}'");
                    return false;
                }

                return await RunCorrectiveAction(credentials, memberId, issuance.IssuanceDate, processingDate, triggeringEvent);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialsIn"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="triggeringEvent"></param>
        /// <param name="registration"></param>
        /// <returns></returns>
        public async Task<bool> RunCorrectiveAction(List<Credential> credentialsIn,
                                                    Guid memberId,
                                                    DateTime eventDate,
                                                    DateTime processingDate,
                                                    TriggeringEvent triggeringEvent = TriggeringEvent.Unkonwn,
                                                    RegistrationData registration = null)
        {
            #region declarations
            Log.Debug($"Started RunCorrectiveAction with parameters credentials='{string.Join(",", credentialsIn.Select(a => a.Id).ToArray())}', memberId='{memberId}', eventDate='{eventDate.ToShortDateString()}', processingDate='{processingDate.ToShortDateString()}'");
            #endregion

            bool allSucceeded = true;
            MemberId = memberId;
            ProcessingDate = processingDate;
            EventDate = eventDate;
            ExecutingProcess = ExecutingProcessType.CorrectiveAction;
            DateTime evaluationDate = processingDate;

            try
            {
                //PBI 161292 -- If this is a CMP which is on hold, no soup for you!
                if (IsOnHoldCMPRegistration(registration))
                {
                    Log.Info($"Not performing corrective action for registration {registration.Id} because it as CMPRegistration which is on hold.");
                    return await Task.FromResult(allSucceeded);
                }

                // filter only none-inactive issuances. 
                var credentials = credentialsIn.Where(i => (i.HasIssuances 
                                                                                   && i.NewestIssuance.IssuanceStatus != IssuanceStatusType.Inactive
                                                                                   )
                                                                                   || (i.Certification.Type == CertificationType.FocusPractice 
                                                                                       && !i.HasIssuances
                                                                                       ) 
                                                                   ).ToList();

                if (!credentials.Any())
                {
                    Log.Info($"MemberId: '{memberId}' does not have any none-inactive credentials");
                    return await Task.FromResult(allSucceeded);
                }

                //PBI 183137 - Corrective Action should ignore deselected certs
                //BUG 195119 - Take into account certs which had previously been selected but are no 
                //longer so.
                credentials = credentials.Where(cred => !cred.DeselectionProcessed || (cred.DeselectionProcessed && !cred.DeselectionElected)).ToList();

                if (!credentials.Any())
                {
                    Log.Info($"MemberId: '{memberId}' does not have any selected certs");
                    return await Task.FromResult(allSucceeded);
                }

                // PBI 223574 : (2.37) Process Exam Results for Cosponsored Certificates
                if (triggeringEvent == TriggeringEvent.ExamResultMocKci && registration != null) 
                {
                    var credential = credentials.FirstOrDefault(a => a.Certification.ExternalId == registration.CertificationId);

                    if (credential == null)
                    {
                        Log.Warn($"ExamResultMocKci : No Credential found for MemberId: {registration.MemberId} and CertificationId: {registration.CertificationId}");
                        return await Task.FromResult(false);
                    }
                    else if (credential.IsCosponsored) 
                    {

                        //**  Update exam fields (pbi 134151) (we re-use existing for CoSponsored and it should work fine ...
                        //** PBI 223713 Remove lock out period : The diplomate receives a result of pass on the long form MOC assessment.
                        //          UpdateCredentialOnLoadingExamResult >> SetCredentialDueDatesFromExamResult >> SetAssessmentMet (GracePeriodStartDate=null && GracePeriodEndDate=null)     
                        UpdateCredentialOnLoadingExamResult(registration, ref credential, processingDate);
                    }
                }

                //PBI 210153 - Don't run CA for cosponsored credentials
                credentials = credentials.Where(cred => !cred.IsCosponsored).ToList();

                if (!credentials.Any())
                {
                    Log.Info($"MemberId: '{memberId}' has only cosponsored credentials.");
                    return await Task.FromResult(allSucceeded);
                }

                //-------------------------------------------------------------------------------------------------------------
                // PBI 136507 : Processing of Events that can trigger corrective action (technical)
                //-------------------------------------------------------------------------------------------------------------
                if (triggeringEvent != TriggeringEvent.YearEndLookBack)
                {
                    if (triggeringEvent == TriggeringEvent.Unkonwn
                            || (triggeringEvent != TriggeringEvent.ExamResultMocKci && triggeringEvent != TriggeringEvent.ExamResultInitial))
                    {
                        //--------------------------------------------------------------------------------------------------------------
                        //** Update of 2 and 5 year dates (pbi 133037 and 133038).
                        //```` NOT an exam events ~~~~~~
                        await LookBackDatesEvaluation();
                    }

                    foreach (var credential in credentials.AbimCredentials())
                    {
                        /* 
                         Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
                            Diplomates with an assessment due in 2020, 2021 or 2022 who do not meet the assessment requirement in 2020 or 2021 or 2022 will not be put in the grace period in 2023.
                         ============================================================================================================================================================================================
                          Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
                            Diplomates with an assessment due in 2020, 2021 who do not meet the assessment requirement in 2020 or 2021 will not be put in the grace period in 2022.
                        ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                        */

                        if (credential.LookbackDate.HasValue &&
                            !(credential.ExamDueDate?.Year == 2022 && credential.IsCovid4))
                        {
                            //** SetGracePeriod  (PBI 134113)
                            //```` All events ~~~~~~
                            bool setGracePeriod = SetGracePeriodIfApplicable(credential, credential.LookbackDate.Value, processingDate);

                            //** Reinstate certificate (pbi 136522)
                            //```` those who are put in the grace period ~~~~~~
                            bool reinstateCertificate = ReinstateCertificate(credential, credential.LookbackDate.Value);

                            if (setGracePeriod || reinstateCertificate)
                            {
                                CredentialService.Handle(new UpdateCredentialFromObjectCommand
                                {
                                    Credential = credential,
                                    SetGracePeriod = setGracePeriod,
                                    ReistateCertificate = reinstateCertificate
                                });
                            }
                        }
                    } //end of foreach (var credential in credentials.AbimCredentials())

                    #region ExamResult processing
                    if (triggeringEvent == TriggeringEvent.ExamResultMocKci && registration != null) // triggering exam for one descipline 
                    {
                        var credential = credentials.FirstOrDefault(a => a.Certification.ExternalId == registration.CertificationId);

                        if (credential == null)
                        {
                            Log.Warn($"ExamResultMocKci : No Credential found for MemberId: {registration.MemberId} and CertificationId: {registration.CertificationId}");
                            return await Task.FromResult(false);
                        }

                        //**  Update exam fields (pbi 134151)
                        //```` triggering event is an exam result ~~~~~~
                        UpdateCredentialOnLoadingExamResult(registration, ref credential, processingDate);

                        // Pre-LookBack events only
                        if (eventDate.Date <= credential.LookbackDate?.Date)
                        {
                            //**  Set Consecutive KCI (pbi 134114)
                            //```` triggering event is an exam result and a pre-lookback event ~~~~~~
                            var consecutiveKCIPassReqModified = SetConsecutiveKCIPassRequired(credential, credential.LookbackDate.Value);

                            //NOTE: The variable above determines if the property was MODIFIED. If ConsecutiveKCIPassRequired was already TRUE, 
                            //we wouldn't have modified it.

                            //**  Year end Certification status check (pbi 134116)
                            //```` triggering event is an exam result and a pre-lookback event ~~~~~~
                            bool fiveYearReqMet = false, assessmentReqMet = false, attestationReqMet = false;
                            var setCertStatus = EvaluateCertStatus(memberId, credential, credential.LookbackDate.Value, processingDate, out fiveYearReqMet, out assessmentReqMet, out attestationReqMet);

                            //**  Year end Participation status check (pbi 134117)
                            //```` triggering event is an exam result and a pre-lookback event ~~~~~~
                            var setParticipationStatus = EvaluateParticipationStatus(credential, credential.LookbackDate.Value, null);

                            if (consecutiveKCIPassReqModified || setCertStatus || setParticipationStatus)
                            {
                                CredentialService.Handle(new UpdateCredentialFromObjectCommand
                                {
                                    Credential = credential,

                                    //For clarity: consecutiveKCIPassReqModified will only be true if it was modified AND set to true.
                                    //And the handler only sets it if the value is TRUE. So if ConsecutiveKCIPassRequired was already 
                                    //TRUE, and consecutiveKCIPassReqModified is FALSE, passing FALSE below won't change 
                                    //SetConsecutiveKCIPassRequired.
                                    SetConsecutiveKCIPassRequired = consecutiveKCIPassReqModified,

                                    SetCertStatus = setCertStatus,
                                    SetParticipationStatus = setParticipationStatus
                                });
                            }
                        }
                    }
                    #endregion // ExamResult processing
                }

                //---------------------------------------------------------------------------------------------------------------------------------
                //+++ PBI:73194 "MBMforTL" : Find Active/Expired TL Credentials and Issue new MBM 
                //---------------------------------------------------------------------------------------------------------------------------------
                var ActiveOrExpiredTLCredentials = credentials
                    .Where(c => c.HasIssuances
                        //Requirement: The certificate status is Active or Expired (eg.not revoked / surrendered / suspended)
                        && (c.NewestIssuance.IssuanceStatus == IssuanceStatusType.Active || c.NewestIssuance.IssuanceStatus == IssuanceStatusType.Expired)
                        //Requirement: [P003] [C001] The certificate is time limited (eg [P041] doesn’t also hold a grandfather or a must be maintained issuance for the same credential) 
                        && c.IsTimelimited
                        //Requirement: The evaluation date is on or after 9/1/year of certificate expiration date 
                        // A) after expiration date
                        && ((c.ExpirationDate.HasValue && processingDate.Date > c.ExpirationDate.Value.Date)
                            // B) after 9/1/year of certificate expiration date 
                            || (processingDate.Date >= new DateTime(processingDate.Year, 9, 1)
                                    && c.ExpirationDate.HasValue && c.ExpirationDate.Value.Year == processingDate.Year))).ToList();

                if (ActiveOrExpiredTLCredentials.Any())
                {
                    // ****** new changes ****
                    await IssueNewCredentialForTLPC_(ActiveOrExpiredTLCredentials,
                                 memberId,
                                 eventDate,
                                 processingDate)
                       .UpdateResult((c, p) => UpdateTLPCCredential(c, p), processingDate)
                       .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));
                }

                //---------------------------------------------------------------------------------------------------------------------------------
                //+++ PBI:xxxxx "MBMforXGF" : Find Expired GF Credentials and Issue new MBM 
                //---------------------------------------------------------------------------------------------------------------------------------
                var ExpiredGFCredentials = credentials.Where(a => a.IsExpiredGrandfather).ToList();

                if (ExpiredGFCredentials.Any())
                {
                    await IssueMBMIssuanceForExpiredGF(ExpiredGFCredentials,
                                 memberId,
                                 eventDate,
                                 processingDate)
                       .UpdateResult((c, p) => UpdateXGFCredentials(c, p), processingDate)
                       .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));
                }

                //---------------------------------------------------------------------------------------------------------------------
                // *** Reinstate TL Certs that are expired where assessment met but can't be issued mbm *** PBI 142201 - ReinstateTL
                //---------------------------------------------------------------------------------------------------------------------
                var ExpiredTLCredentials = credentials
                        .Where(a => a.IsTimelimited)
                        .Where(c => c.NewestIssuance.IssuanceStatus == IssuanceStatusType.Expired).ToList();

                if (ExpiredTLCredentials.Any())
                {
                    await CorrectiveActionReinstateTLCert(ExpiredTLCredentials,
                                                    memberId,
                                                    eventDate,
                                                    evaluationDate)
                            .UpdateResult(r => UpdateTLCertStatusCredentials(r))
                            .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));
                }
              
                //--------------------------------------------------------------------------------------------------------------
                //+++ Check for Not Maintained Certs ++++ PBI: 86857, 86853, 86855
                //--------------------------------------------------------------------------------------------------------------
                var ActiveNotMaintainedCredentials = credentials.Where(c => c.HasIssuances && c.IsNewestIssuanceABIM &&
                                                                            c.Issuances.Any(i => i.IssuanceStatus == IssuanceStatusType.Active &&
                                                                                i.MaintenanceStatus == MaintenanceStatusType.NotMaintained)).ToList();

                if (ActiveNotMaintainedCredentials.Any())
                {
                    await CorrectiveActionParticipationStatus_(ActiveNotMaintainedCredentials,
                                                                memberId,
                                                                eventDate,
                                                                evaluationDate,
                                                                processingDate)
                            .UpdateResult(r => UpdateParticipationStatusCredentials(r))
                            .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));
                }

                //--------------------------------------------------------------------------------------------------------------
                //+++ Check for Inactive or Expired Certs ++++ Pbi: 86846. Corrective action - MBM cert status
                //--------------------------------------------------------------------------------------------------------------
                var InactiveOrExpiredMBMCredentials = credentials
                                .Where(a => a.IsMBM)
                                .NewestIssuanceABIM()
                                .Where(c => c.NewestIssuance.IssuanceStatus == IssuanceStatusType.Expired && c.SelectedToMaintain).ToList(); // only  SelectedToMaintain
                // PBI 254149 : (2.45) When checking for Inactive or Expired MBM Certs, exclude SelectedToMaintain=0
                // if diploymate has  IM cert and then pass FPHM init cert, then we expire IM and SelectedToMaintain to 0, but this above process can put them in activa status again.

                if (InactiveOrExpiredMBMCredentials.Any())
                {
                    await CorrectiveActionCertStatus_(InactiveOrExpiredMBMCredentials,
                                                    memberId,
                                                    eventDate,
                                                    evaluationDate)
                            .UpdateResult(r => UpdateCertStatusCredentials(r))
                            .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));
                }

               

            }
            catch (Exception ex)
            {
                // skip if exception was thrown intentionally to stop firther processing of the code above
                if (!ex.Message.StartsWith(Constants.StopFurtherExecutionMessage))
                {
                    Logger.Error(ex);
                    throw;
                }
            }

            return await Task.FromResult<bool>(allSucceeded);
        }

        #region Update <<>>>

        /// <summary>
        /// Update Credentitals based on Grace Period rule results evaluations
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        /// <param name="processingDate"></param>
        public void UpdateGracePeriodCredentials(IEnumerable<CredentialToUpdate> credentialsToUpdate, DateTime processingDate)
        {
            foreach (var credential in credentialsToUpdate)
            {
                string ModifiedBy = string.Format("Corrective{0}Grace", GetCredentialInitials(credential.CredentialCategory));

                // ****  Must Be Maintained & Timelimited certificates ****
                if (credential.CredentialCategory == CredentialCategoryType.MustBeMaintained ||
                    credential.CredentialCategory == CredentialCategoryType.TimeLimited)
                {

                    //perform the update on the credential
                    var command = new UpdateCredentialGracePeriodTLandMBMCommand()
                    {
                        CredentialId = credential.CredentialId,
                        ModifiedBy = ModifiedBy,
                        MaintenanceStatus = credential.MaintenanceStatus,
                        ProcessingDate = processingDate
                    };

                    var result = (UpdateCredentialGracePeriodTLandMBMCommandResult)(CredentialService.Handle(command));

                    if (result.Succeeded)
                        Logger.Debug($"[PBI:91092,91093] Updated Grace Period Credential:'{credential.CredentialId}' {command.Dump()}");
                    else if (result.Status == CommandStatus.Rejected)
                        Log.Error($"UpdateGracePeriodCredentials's call to Handle(UpdateCredentialGracePeriodTLandMBMCommand) failed with the following error:'{result.Message}'");

                }
                // ****  GrandFather *****
                else if (credential.CredentialCategory == CredentialCategoryType.GrandFather)
                {
                    //perform the update on the credential
                    var command = new UpdateCredentialGracePeriodGFCommand()
                    {
                        CredentialId = credential.CredentialId,
                        ModifiedBy = ModifiedBy,
                        MaintenanceStatus = credential.MaintenanceStatus,
                        ProcessingDate = credential.ProcessingDate
                    };

                    var result = (UpdateCredentialGracePeriodGFCommandResult)(CredentialService.Handle(command));

                    if (result.Succeeded)
                        Logger.Debug($"[PBI:91562] Updated Grace Period Credential:'{credential.CredentialId}' {command.Dump()}");
                    else if (result.Status == CommandStatus.Rejected)
                        Log.Error($"UpdateGracePeriodCredentials's call to Handle(UpdateCredentialGracePeriodGFCommand) failed with the following error:'{result.Message}'");

                }
            } //... end foreach()
        }

        /// <summary>
        /// Update Credentitals based on Participation Status results evaluations
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        public void UpdateParticipationStatusCredentials(IEnumerable<CredentialToUpdate> credentialsToUpdate)
        {
            foreach (var credential in credentialsToUpdate)
            {
                string ModifiedBy = string.Format("Corrective{0}", GetCredentialInitials(credential.CredentialCategory));

                if (credential.CredentialCategory == CredentialCategoryType.MustBeMaintained ||
                   credential.CredentialCategory == CredentialCategoryType.TimeLimited ||
                   credential.CredentialCategory == CredentialCategoryType.GrandFather)
                {

                    //perform the update on the credential
                    var command = new SetIssuanceToMaintainedCommand()
                    {
                        CredentialId = credential.CredentialId,
                        ModifiedBy = ModifiedBy
                    };

                    var result = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(command));

                    if (result.Succeeded)
                        Logger.Debug($"[PBI:86857,86853,86855] Added New issuance To DB for credentialId:'{credential.CredentialId}' {command.Dump()}");
                    else if (result.Status == CommandStatus.Rejected)
                        Log.Error($"UpdateParticipationStatusCredentials's call to Handle(SetIssuanceToMaintainedCommand) failed with the following error:'{result.Message}'");
                }

            } //... end foreach()
        }

        /// <summary>
        /// Update Credentitals based on Certification Status results evaluations
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        public void UpdateCertStatusCredentials(IEnumerable<CredentialToUpdate> credentialsToUpdate)
        {
            var credentialToUpdatesLst = credentialsToUpdate.ToList();
            DateTime ScheduledUpdate = ProgramRulesHelpers.ComputeScheduleUpdateDateForIssuance(credentialToUpdatesLst.First().ProcessingDate);

            foreach (var credential in credentialToUpdatesLst)
            {
                string ModifiedBy = string.Format("Corrective{0}Cert", GetCredentialInitials(credential.CredentialCategory));

                // ****  Must Be Maintained (ONLY)
                if (credential.CredentialCategory == CredentialCategoryType.MustBeMaintained)
                {
                    //perform the update on the credential
                    var command = new ReissueCommand()
                    {
                        CredentialId = credential.CredentialId,
                        IssuanceDate = credential.IssuanceDate,
                        ScheduledUpdate = ScheduledUpdate,
                        CreatedBy = ModifiedBy,
                        ProcessingDate = credential.ProcessingDate,
                        MaintenanceStatus = credential.MaintenanceStatus
                    };

                    var result = (ReissueCommandResult)(CredentialService.Handle(command));

                    if (result.Succeeded)
                        Logger.Debug($"[PBI:86846] Added New issuance To DB for credentialId:'{credential.CredentialId}' {command.Dump()}");
                    else if (result.Status == CommandStatus.Rejected)
                        Log.Error($"UpdateCertStatusCredentials's call to Handle(ReissueCommand) failed with the following error:'{result.Message}'");

                }
            } //... end foreach()
        }


        /// <summary>
        /// Update TL Credentitals based on Certification Status results evaluations
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        public void UpdateTLCertStatusCredentials(IEnumerable<CredentialToUpdate> credentialsToUpdate)
        {
            foreach (var credential in credentialsToUpdate)
            {
                //perform the update on the credential
                var command = new ReinstateTLCommand()
                {
                    CredentialId=credential.CredentialId,
                    IssuanceStatus = IssuanceStatusType.Active,
                    MaintenanceStatus = credential.MaintenanceStatus,
                    ModifiedBy = "ReinstateTL"
                };

                var result = (ReinstateTLCommandResult)(CredentialService.Handle(command));

                if (result.Succeeded)
                {
                    // -- don't need to publish event since we just extend expiration date // Task.Run(() => CredentialService.PublishIssueChangedEvent(result.Data));
                    Logger.Debug($"[PBI:142201] Reinstated TL for credentialId:'{credential.CredentialId}' {command.Dump()}");
                }
                else if (result.Status == CommandStatus.Rejected)
                    Log.Error($"UpdateTLCertStatusCredentials's call to Handle(ReinstateTLCommand) failed with the following error:'{result.Message}'");

            } //... end foreach()
        }

        /// <summary>
        /// Updates the TLPC credential.
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        /// <param name="processingDate"></param>
        public void UpdateTLPCCredential(IEnumerable<CredentialToUpdate> credentialsToUpdate, DateTime processingDate)
        {
            DateTime ScheduledUpdate = ProgramRulesHelpers.ComputeScheduleUpdateDateForIssuance(processingDate);

            foreach (var credential in credentialsToUpdate)
            {
                // Log current information
                Log.Info($"UpdateTLPCCredential CredenitalId:{credential.CredentialId} IssuanceDate:{credential.IssuanceDate.ToShortDateString()} on Thread:{Thread.CurrentThread.ManagedThreadId}");

                var command = new ExpireAndReissueCommand()
                {
                    CredentialId = credential.CredentialId,
                    IssuanceDate = credential.IssuanceDate,
                    MaintenanceStatus = credential.MaintenanceStatus,
                    ScheduledUpdate = ScheduledUpdate,
                    CreatedBy = "MBMforTL"
                };

                var result = (ExpireAndReissueCommandResult)(CredentialService.Handle(command));

                if (result.Succeeded)
                    Logger.Debug($"[PBI:73194] Added New issuance To DB for credentialId:'{credential.CredentialId}' {command.Dump()}");
                else if (result.Status == CommandStatus.Rejected)
                    Log.Error($"UpdateTLPCCredential's call to Handle(ExpireAndReissueCommand) failed with the following error:'{result.Message}'");

            }

        }

        /// <summary>
        /// Updates the expired GF credentials.
        /// </summary>
        /// <param name="credentialsToUpdate"></param>
        /// <param name="processingDate"></param>
        public void UpdateXGFCredentials(IEnumerable<CredentialToUpdate> credentialsToUpdate, DateTime processingDate)
        {
            DateTime ScheduledUpdate = ProgramRulesHelpers.ComputeScheduleUpdateDateForIssuance(processingDate);

            foreach (var credential in credentialsToUpdate)
            {
                // Log current information
                Log.Info($"UpdateXGFCredentials CredenitalId:{credential.CredentialId} IssuanceDate:{credential.IssuanceDate.ToShortDateString()} on Thread:{Thread.CurrentThread.ManagedThreadId}");

                var command = new ReissueCommand()
                {
                    CredentialId = credential.CredentialId,
                    IssuanceDate = credential.IssuanceDate,
                    MaintenanceStatus = credential.MaintenanceStatus,
                    ScheduledUpdate = ScheduledUpdate,
                    ProcessingDate=processingDate,
                    CreatedBy = "MBMforXGF"
                };
              
                var result = (ReissueCommandResult)(CredentialService.Handle(command));

                if (result.Succeeded)
                    Logger.Debug($"Added New issuance To DB for credentialId:'{credential.CredentialId}' {command.Dump()}");
                else if (result.Status == CommandStatus.Rejected)
                    Log.Error($"UpdateXGFCredentials's call to Handle(ExpireAndReissueCommand) failed with the following error:'{result.Message}'");
            }
        }



        /// <summary>
        /// TryUpdateLookBackDatesInfo
        /// </summary>
        /// <param name="calculatedLookBackDatesInfo"></param>
        /// <param name="UserName"></param>
        /// <param name="currentLookBackDatesInfo"></param>
        /// <returns></returns>
        internal async Task TryUpdateLookBackDatesInfo(LookBackDatesInfo calculatedLookBackDatesInfo,
                                                              string UserName,
                                                              LookBackDatesInfo currentLookBackDatesInfo = null)
        {
            Tuple<DateTime, DateTime> twoYearLookBackWindow = null;
            Tuple<DateTime, DateTime> fiveYearLookBackWindow = null;

            // -- Two year look back ++++
            if (calculatedLookBackDatesInfo != null && // we have calculated value
                ((ProcessingDate.Date > currentLookBackDatesInfo?.Lookback2YearEndDate?.Date && // and current value is expired
                    calculatedLookBackDatesInfo?.Lookback2YearEndDate?.Date != currentLookBackDatesInfo?.Lookback2YearEndDate?.Date) // calculated and current end date is diferent
                        || (calculatedLookBackDatesInfo.Lookback2YearEndDate.HasValue && (currentLookBackDatesInfo == null || currentLookBackDatesInfo?.Lookback2YearEndDate == null)))) // or current 2y look back is null
            {
                twoYearLookBackWindow = new Tuple<DateTime, DateTime>(
                            calculatedLookBackDatesInfo.Lookback2YearStartDate.Value,
                            calculatedLookBackDatesInfo.Lookback2YearEndDate.Value);
            }

            // -- Five year look back ++++
            if (calculatedLookBackDatesInfo != null && // we have calculated value
                ((ProcessingDate.Date > currentLookBackDatesInfo?.Lookback5YearEndDate?.Date && // and current value is expired
                    calculatedLookBackDatesInfo?.Lookback5YearEndDate?.Date != currentLookBackDatesInfo?.Lookback5YearEndDate?.Date) // end date is different and after current end date
                        || (calculatedLookBackDatesInfo.Lookback5YearEndDate.HasValue && (currentLookBackDatesInfo == null || currentLookBackDatesInfo?.Lookback5YearEndDate == null)))) // or current 5y look back is null
            {
                fiveYearLookBackWindow = new Tuple<DateTime, DateTime>(
                            calculatedLookBackDatesInfo.Lookback5YearStartDate.Value,
                            calculatedLookBackDatesInfo.Lookback5YearEndDate.Value);
            }

            // check if anything calculated is different from Db values
            if (twoYearLookBackWindow == null && fiveYearLookBackWindow == null)
            {
                Log.Info($"No changes for LookBackWindows for MemberId:'{MemberId}'");
                return;
            }

            Log.Info($"UpdateLookBackDatesInfoCommands# MemberId:'{MemberId}', " +
                $"2YearStartDate:'{twoYearLookBackWindow?.Item1.ToShortDateString()}', " +
                $"2YearEndDate:'{twoYearLookBackWindow?.Item2.ToShortDateString()}', " +
                $"5YearStartDate:'{fiveYearLookBackWindow?.Item1.ToShortDateString()}', " +
                $"5YearEndDate:'{fiveYearLookBackWindow?.Item2.ToShortDateString()}'");

            await LookBackDatesInfoService.Handle(new UpdateLookBackDatesInfoCommand()
            {
                MemberId = MemberId,
                Lookback2YearStartDate = twoYearLookBackWindow?.Item1,
                Lookback2YearEndDate = twoYearLookBackWindow?.Item2,
                Lookback5YearStartDate = fiveYearLookBackWindow?.Item1,
                Lookback5YearEndDate = fiveYearLookBackWindow?.Item2,
                UserName = UserName
            });
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Return Credential Category Type Initials (used in computing ModifiedBy or CreatedBy)
        /// </summary>
        /// <param name="CredentialCategoryType"></param>
        /// <returns></returns>
        public string GetCredentialInitials(CredentialCategoryType CredentialCategoryType)
        {
            if (CredentialCategoryType == CredentialCategoryType.GrandFather)
                return "GF";
            else if (CredentialCategoryType == CredentialCategoryType.TimeLimited)
                return "TL";
            else if (CredentialCategoryType == CredentialCategoryType.MustBeMaintained)
                return "MBM";
            else
                return "";
        }
        /// <summary>
        /// IsOnHoldCMPRegistration
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public bool IsOnHoldCMPRegistration(RegistrationData registration)
        {
            return registration != null
                && registration.IsCmp
                && registration.OnHold;
        }

        #endregion Helpers

        #endregion
    }
}
