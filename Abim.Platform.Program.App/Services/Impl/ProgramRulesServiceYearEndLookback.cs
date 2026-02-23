using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService
    {
        /// <summary>
        /// Runs the Year End Lookback process
        /// </summary>
        /// <param name="memberId">The GUID ID of the user that Year End Lookback is being run for</param>
        /// <param name="lookBackDate">The date for which Year End Lookback is being run (for example, 12/31/2018)</param>
        /// <param name="processingDate">The date of the Year End Lookback run</param>
        /// <returns></returns>
        public async Task<bool> RunYearEndLookback(Guid memberId, DateTime lookBackDate, DateTime processingDate)
        {
            //PBI 134881 - Year End Lookback (Technical)
            try
            {
                //I think we want to set this here.
                ProcessingDate = processingDate;
                MemberId = memberId;
                ExecutingProcess = ExecutingProcessType.YearEndLookBack;
                // lookBackDate is currentYearLookbackDate in this context

                var credentials = GetCredentialsEligibleForLookback(memberId);

                if (!credentials.Any())
                {
                    Log.Info($"Member {memberId} does not have any credentials eligible for lookback.");
                    //The foreach below won't get executed, and we'll return await Task.FromResult<bool>(true); at the end.
                }

                foreach (var cred in credentials)
                {
                    /*
                     * THIS METHOD ASSUMES THAT THE UPDATES WILL BE MADE TO THE CREDENTIAL ITSELF, 
                     * AND THEN THE CREDENTIAL WILL BE USED TO POPULATE PROPERTIES ON A COMMAND OBJECT FOR 
                     * UPDATING THE RECORD IN THE DATABASE.
                     * IF YOU TAKE A DIFFERENT APPROACH, PLEASE SUPPLY RETURN VALUES AND MODIFY THE SAVE 
                     * METHOD AS NEEDED. THANKS!
                    */

                    

                    bool fiveYearReqMet = false;
                    bool assessmentReqMet = false;
                    bool attestationReqMet = false;
                    var participationStatusLogStatements = new List<AddParticipationLookbackLog>();

                    var credStartingIssuanceStatus = cred.NewestIssuance?.IssuanceStatus;
                    var credStartingMaintStatus = cred.NewestIssuance?.MaintenanceStatus;

                    // PBI 175873: (Proj 1474) Certification & Participation Status Changes for 2020 MOC Requirements
                    // PBI 208254 Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(Not COVID 4)
                    // PBI 216370 Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
                    // *** For 2020 and 2021 only 
                    //  2) Diplomates currently in the grace period in 2020 will have their grace period term extended to 12/31/2021.
                    //  3) Diplomates with an assessment due in 2020 who do not meet the assessment requirement in 2020 will not be put in the grace period in 2021.
                    if (IsEligibleForCOVIDExtension(lookBackDate, cred.IsCovid4))
                    {
                        ExtendGracePeriodIn2021Only(cred);
                    }
                    else
                    {
                        ClearGracePeriod(cred, lookBackDate);               //PBI 134885
                        SetGracePeriodIfApplicable(cred, lookBackDate, processingDate); //PBI 134113
                    }

                    SetConsecutiveKCIPassRequired(cred, lookBackDate);  //PBI 134114 : Set 2 Year Pass Due (Technical)
                    await KickOutOfCMPIfApplicable(cred, lookBackDate);       //PBI 159160 : Kick of out CMP Pathway

                    ClearAssessmentMetWhenApplicable(cred, lookBackDate);             //PBI 134115 : Clear Assessment Met (Technical)

                    EvaluateCertStatus(memberId, cred, lookBackDate, processingDate, out fiveYearReqMet, out assessmentReqMet, out attestationReqMet); //PBI 134116
                    LogCertStatus(cred, fiveYearReqMet, assessmentReqMet, attestationReqMet, lookBackDate, memberId);  //PBI 134886 / 137216
                    EvaluateParticipationStatus(cred, lookBackDate, participationStatusLogStatements);                // PBI 134117 : Participation Status Evaluation (Technical)
                    LogAllParticipationStatusEntries(participationStatusLogStatements);

                    RecordLookbackDate(cred, lookBackDate);

                    Update(cred);

                    if (ShouldWeRunCorrectiveAction(
                            credStartingIssuanceStatus,
                            cred.NewestIssuance?.IssuanceStatus,
                            credStartingMaintStatus,
                            cred.NewestIssuance?.MaintenanceStatus))
                        await RunCorrectiveAction(
                            credentialsIn : new List <Credential> { cred }, //Running corrective action just for this credential
                            memberId : memberId,
                            eventDate: lookBackDate,
                            processingDate: processingDate,
                            triggeringEvent: TriggeringEvent.YearEndLookBack); //TODO: Change processing date?
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

            return await Task.FromResult<bool>(true);
        }

        private void LogAllParticipationStatusEntries(List<AddParticipationLookbackLog> logList)
        {
            logList.ForEach(logEntry => 
            {
                var result = (AddLookbackLogCommandResult)LookbackLogService.Handle(logEntry);
                if (!result.Succeeded)
                {
                    string error = $"Failed to add LookbackLog record: {result.Message}. Command = {logEntry.Dump()}";
                    Log.Error(error);
                    throw new ApplicationException(error);
                }
            });
        }
        private List<Credential> GetCredentialsEligibleForLookback(Guid memberId)
        {
            //PBI 134881
            var creds = CredentialService.SearchByMemberId(memberId);

            if (creds == null)
                return new List<Credential>(0);

            return creds.Where(x => 
                x.Certification.Source.Code.ToUpper() == "ABIM"
                && !x.IsCosponsored) //PBI 210153 - Don't run YELB for cosponsored credentials
                .ToList();
        }

        private void ClearGracePeriod(Credential cred, DateTime lookbackDate)
        {
            //PBI 134885
            if (lookbackDate >= cred.GracePeriodEndDate)
            {
                cred.GracePeriodStartDate = null;
                cred.GracePeriodEndDate = null;
                cred.SetModified("ClearGrace"); //This also sets the modified date
                cred.HasChanged = true;
            }
        }

        /// <summary>
        /// Sets the grace period on a credential if applicable
        /// </summary>
        /// <param name="cred">The credential to set grace period on if applicable</param>
        /// <param name="lookbackDate">The most recent lookback date</param>
        /// <param name="processingDate">The date the process is occurring</param>
        /// <returns>true if the grace period was set, false otherwise</returns>
        private bool SetGracePeriodIfApplicable(Credential cred, DateTime lookbackDate, DateTime processingDate)
        {
            //PBI 134113
            if (!cred.ExamDueDate.HasValue) return false;
            var examDueDateYear = cred.ExamDueDate.Value.Year;

            //This PBI relies on PBI 135009 Non-Exam Requirements
            var meetsNonExamsRequirement = NonExamRequirement(cred, lookbackDate, processingDate,true);
            var hasCorrespondingRegOrLka = CorrespondingRegistration(cred, examDueDateYear);

            // pbi 179929 : Update Program Rule 12 Assessment Grace Period to Include LNG
            if (!hasCorrespondingRegOrLka)
                hasCorrespondingRegOrLka = CorrespondingLkaEnrollment(cred);

            if (ShouldGoIntoGracePeriod(cred, lookbackDate, meetsNonExamsRequirement, examDueDateYear, hasCorrespondingRegOrLka))
            {
                cred.GracePeriodStartDate = new DateTime(lookbackDate.Year + 1, 1, 1);
                cred.GracePeriodEndDate = new DateTime(lookbackDate.Year + 1, 12, 31);
                cred.SetModified("SetGrace");//This also sets the modified date
                cred.HasChanged = true;
                return true;
            }
            return false;
        }

        private bool ShouldGoIntoGracePeriod(
            Credential cred, 
            DateTime lookbackDate, 
            bool meetsNonExamRequirement, 
            int examDueDateYear, 
            bool hasCorrespondingExamRegistrationOrLkaEnrollment)
        {
            // going back to original version of this function (before Pbi 179929) since I moved the functionality to CorrespondingLkaEnrollment()
            return ((cred.IsActiveParticipating ||
                  // Certificate expired and expiration date=current lookback date and Met 2 year requirement (pbi 135236)
                  (cred.NewestIssuance?.IssuanceStatus == IssuanceStatusType.Expired
                  && cred.LookbackDate?.Date == lookbackDate.Date  // could only be true during CA -- would still be previous year end date during YELB
                  && MaintenanceStatus(cred, ProcessingDate, true, true).MeetStepRule))
              && meetsNonExamRequirement
              && lookbackDate.Date.Year == examDueDateYear
              && hasCorrespondingExamRegistrationOrLkaEnrollment);
        }

        /* 
         Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
            Diplomates in one of the COVID 4 disciplines (Infectious Disease, Hospital Medicine, Critical Care, Pulmonary Disease) currently in the grace period in 2020 
            will have their grace period term extended to 12/31/2023.
         ============================================================================================================================================================================================
          Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
            Diplomates  currently in the grace period in 2020 will have their grace period term extended to 12/31/2022.
        ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        */
        private void ExtendGracePeriodIn2021Only(Credential cred)
        {
            if (cred.GracePeriodStartDate?.Year == 2020)
            {
                cred.GracePeriodEndDate = cred.IsCovid4 ? new DateTime(2023, 12 , 31) : new DateTime(2022, 12, 31); // literally as in requirements above
                cred.SetModified("ExtendGracePeriod");
            }

        }

        private bool CorrespondingRegistration(Credential cred, int examDueDateYear)
        {
            var result = Registrations.Any(r => (r.ExamType != null && r.ExamType.ToEnum() == ExamType.Moc)
                                        && (r.CertificationId == cred.Certification.ExternalId)
                                        && (r.Result == ExamResultType.Fail.ToString()
                                                || r.Result == ExamResultType.Indeterminate.ToString()
                                                || r.Result == ExamResultType.Incomplete.ToString()
                                                || r.Result == ExamResultType.UnableToTest.ToString())
                                        && (r.AdministrationYear == examDueDateYear));

            // if still false then we need to check CMPRegistrations
            if (!result && CMPRegistrations != null)
            {
                //-- Pbi 150926 set grace period
                // OR a CMPRegistration that is for the crediential being checked which
                // Has a result of fail or unable to test(indeterminate / incomplete not valid results for CMP exam)
                // Year of the CMPRegistration = year of credential.ExamDueDate
                result = CMPRegistrations.Any(r => (r.CMPExam != null && r.CMPExam.CertificationId == cred.Certification.ExternalId)
                                && (r.ExamResult.ToEnum() == ExamResultType.Fail || r.ExamResult.ToEnum() == ExamResultType.UnableToTest)
                                && (r.TestDate.Year == examDueDateYear));
            }

            return result;
        }

        private bool CorrespondingLkaEnrollment(Credential cred)
        {
            var returnValue = false;
            //===============================================================
            // pbi 179929 : Update Program Rule 12 Assessment Grace Period to Include LNG
            // The diplomate is enrolled in the Longitudinal Assessment  AND did not meet the annual LNG participation requirement 
            //      OR the diplomate received a result of FAIL on the summative assessment
            var currentEnrollment = LongitudinalEnrollments.Where(a => a.Assessment.CertificationId == cred.Certification.ExternalId);

            if (currentEnrollment != null && currentEnrollment.Count() > 0)
            {
                var findDontMeetParticipation = currentEnrollment
                                                    .Where(a => (a.CurrentlyMeetingParticipation == false ||
                                                        ( a.EnrollmentStatus != null && a.SuspensionReason != null &&
                                                            a.EnrollmentStatus.ToEnum() == EnrollmentStatusType.Suspended &&                                                         
                                                            a.SuspensionReason.ToEnum() == SuspensionReasonType.FailedToMeetParticipation)))
                                                    .FirstOrDefault();

                Log.Info($"Credential {cred.ExternalId} findDontMeetParticipation = {findDontMeetParticipation != null}.");

                if (findDontMeetParticipation != null)
                    returnValue = true;
                else
                {
                    var findFailOnSummativeDecision = currentEnrollment
                                                    .Where(a => a.LongitudinalParticipations.Last().SummativeDecision.ToEnum() == SummativeDecisionType.Fail ||
                                                    ( a.EnrollmentStatus != null && a.SuspensionReason != null &&
                                                    a.EnrollmentStatus.ToEnum() == EnrollmentStatusType.Suspended && a.SuspensionReason.ToEnum() == SuspensionReasonType.FailedSummativeDecision))
                                                    .FirstOrDefault();

                    Log.Info($"Credential {cred.ExternalId} findFailOnSummativeDecision = {findFailOnSummativeDecision != null}.");

                    if (findFailOnSummativeDecision != null)
                        returnValue = true;
                }
            }

            return returnValue;
        }

        private bool NonExamRequirement(
            Credential cred,
            DateTime lookbackDate,
            DateTime processingDate,
            bool calledFromGrace)
        {
            //There is an overload of the NonExamRequirement() method that takes 2 output params.
            //But some code that uses that method have no need for that information.
            //In that case, this version of the method supplies the output params, which are ignored.
            bool fiveYearReqMet;
            bool attestationReqMet;

            return NonExamRequirement(cred, lookbackDate, processingDate, out fiveYearReqMet, out attestationReqMet, calledFromGrace);
        }

        /// <summary>
        /// PBI 135009 Non Exam Requirement
        /// </summary>
        /// <param name="cred"></param>
        /// <param name="lookbackDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="fiveYearReqMet">An output parameter indicating whether or not the five year requirement has been met</param>
        /// <param name="attestationReqMet">An output parameter indicating whether or not the attestation requirement has been met</param>
        /// <param name="calledFromGrace">True if caled from SetGracePeriod; false otherwise</param>
        /// <returns></returns>
        private bool NonExamRequirement(
            Credential cred, 
            DateTime lookbackDate, 
            DateTime processingDate, 
            out bool fiveYearReqMet, 
            out bool attestationReqMet,
            bool calledFromGrace)
        {
            //PBI 135012 (PBI 135024, 135025, 135026, 135044 are all encapsulated in FiveYearsLookBack method)
            //PBI 135024 & 134113: when invoking FiveYearsLookBack 100 pts calculation use lookbackDate as the endDate, 
            //EXCEPT when invoking from SetGracePeriod(and EXCEPT when invoked from Corrective Action)
            var fiveYearLookBackStep = FiveYearsLookBack(cred, lookbackDate, (calledFromGrace ? processingDate : lookbackDate), false);

            //PBI 135010 & 135011
            var attestationStep = Attestation(cred, lookbackDate, processingDate);
            //PBI 135009

            Log.Debug($"Credential {cred.ExternalId} Non Exam Requirements: Five Year Lookback = {fiveYearLookBackStep.MeetStepRule}, Attestation = {attestationStep.MeetStepRule}.");

            fiveYearReqMet = fiveYearLookBackStep.MeetStepRule;
            attestationReqMet = attestationStep.MeetStepRule;

            return fiveYearLookBackStep.MeetStepRule && attestationStep.MeetStepRule;
        }

        /// <summary>
        /// /PBI 134114 : Set 2 Year Pass Due (Technical)
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="lookbackDate"></param>
        /// <returns>A boolean specifiying whether or not the field was changed</returns>
        private bool SetConsecutiveKCIPassRequired(Credential credential, DateTime lookbackDate)
        {
            //This method was originally called Set2YearPassDue() but was renamed during coding for Bug 148899 because the original method 
            //name could imply that a due date was being set. In actuality, this is determining if the user needs to pass two consecutive KCI 
            //exams (and if we need to SET that value -- if already set, bail!).

            // if it is already set then we don't need to do anything
            if (credential.ConsecutiveKCIPassRequired)
                return false;

            // 1) Not( Current year lookback date >= credential.ExamDueDate )
            if (!(lookbackDate.Date >= credential.ExamDueDate?.Date))
                return false;

            var hasPendingExam = DoPendingExamResultsExistForCertification(
                                                        credential.Certification.ExternalId,
                                                        lookbackDate);

            // 2) && Not Pending exam results from previous year [pbi 134903]
            if (!hasPendingExam)
            {
                credential.ConsecutiveKCIPassRequired = true;
                credential.SetModified("Require2KCI");
                credential.HasChanged = true;
                return true;
            }
            return false;      
        }

        /// <summary>
        /// If credential.asssementmet is true  and actual Credential.ExamDueDate &lt; = current lookback date
        ///     Set credential.assessmentmet to false
        ///     Set credential.modified to current date
        ///      Set credential.ModifiedBy to ‘ClearAssessment’
        /// PBI 134115:Clear Assessment Met (Technical)
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="lookbackDate"></param>
        private void ClearAssessmentMetWhenApplicable(Credential credential, DateTime lookbackDate)
        {
            if (credential.AssessmentMet 
                && lookbackDate.Date >= credential.ExamDueDate?.Date
                && !IsEligibleForCOVIDExtension(lookbackDate, credential.IsCovid4))
            {
                credential.AssessmentMet = false;
                credential.SetModified("ClearAssessment");
            }
        }

        private bool EvaluateCertStatus(
            Guid memberId,
            Credential cred,
            DateTime lookbackDate,
            DateTime processingDate,
            out bool fiveYearReqMet,
            out bool assessmentReqMet,
            out bool attestationReqMet)
        {

            /* 
             Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
                For 2020, 2021, and 2022, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
                *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021 or 2022
                *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021 or 2022
                *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021 or 2022
             ============================================================================================================================================================================================
              Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
                  For 2020 and 2021, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
                  *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021
                  *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021
                  *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            */
            if (IsEligibleForCOVIDExtension(lookbackDate, cred.IsCovid4))
            {
                //NOTE: These are output variables only! These are not modifications to the credential.
                fiveYearReqMet = true;
                assessmentReqMet = true;
                attestationReqMet = true;
                return false;
            }

            //PBI 134116
            bool meetsExamReqs = MeetsExamRequirements(cred, memberId, lookbackDate, out assessmentReqMet);
            bool meetsNonExamReqs = NonExamRequirement(cred, lookbackDate, processingDate, out fiveYearReqMet, out attestationReqMet, false);

            if (meetsExamReqs & meetsNonExamReqs) 
            {
                Log.Debug($"Cert requirements met for member {memberId}, credential {cred.ExternalId}, lookback date {lookbackDate.ToShortDateString()}, processing date {processingDate.ToShortDateString()}.");
                return false;
            }
            //Bug 144627: Task 144628 : Do not expire issuances that have an issuance date > lookback date in pbi 142264
            // If found new issuance after lookback date during YELB, just stop further evaluation
            else if (ExecutingProcess == ExecutingProcessType.YearEndLookBack && cred.NewestIssuance?.IssuanceDate.Date > lookbackDate.Date)
            {
                Log.Debug($"YELB found new issuance {cred.NewestIssuance?.IssuanceDate.Date.ToShortDateString()} since lookback date {lookbackDate.ToShortDateString()} with processing date {processingDate.ToShortDateString()}.");
                return false;
            }

            Log.Debug($"Cert requirements NOT met for member {memberId}, credential {cred.ExternalId}, lookback date {lookbackDate.ToShortDateString()}, processing date {processingDate.ToShortDateString()}. Meets Exam Reqs = {meetsExamReqs}, Meets Non Exam Reqs = {meetsNonExamReqs}.");

            //Requirements not met. Get the issuances to update.
            var issuancesToUpdate = 
                cred.Issuances.Where(
                    x =>
                    ((x.Duration == DurationType.Timelimited && lookbackDate >= x.ExpirationDate)
                    || x.Duration == DurationType.Continuous)
                    && x.IssuanceStatus == IssuanceStatusType.Active);

            foreach (var issuance in issuancesToUpdate)
            {
                issuance.IssuanceStatus = IssuanceStatusType.Expired;

                if (issuance.ExpirationDate == null)
                    issuance.ExpirationDate = lookbackDate;

                issuance.ExpiredDate = lookbackDate; //PBI 142264
                issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
                issuance.SetModified("CertStatus");
                issuance.HasChanged = true;
            }

            //Any active issuances left? (Would be for lifetime cert.)
            //If not, the credential is expired.
            if (!cred.Issuances.Any(x => x.IssuanceStatus == IssuanceStatusType.Active))
            {
                cred.IsActive = false;
                cred.SetModified("CertStatus");
                cred.HasChanged = true;
            }

            return cred.Issuances.Any(a => a.HasChanged) || cred.HasChanged;
        }

        private void LogCertStatus(
            Credential cred, 
            bool fiveYearReqMet, 
            bool assessmentReqMet, 
            bool attestationReqMet, 
            DateTime lookbackDate, 
            Guid memberId)
        {
            //PBI 134886 / 137216

            //Determine other factors
            bool potentiallyGoingIntoGracePeriod = //Yeah, it's a long variable name, but it's descriptive...
                IsPotentiallyGoingIntoGracePeriod(cred, lookbackDate, memberId);

            bool pendingResultsExist = 
                DoPendingExamResultsExistForCertification(cred.Certification.ExternalId, lookbackDate);

            Log.Debug($"Logging cert status for member {memberId}, credential {cred.ExternalId}. fiveYearReqMet = {fiveYearReqMet}, assessmentReqMet = {assessmentReqMet}, attestationReqMet = {attestationReqMet}, potentiallyGoingIntoGracePeriod = {potentiallyGoingIntoGracePeriod}, pendingResultsExist = {pendingResultsExist}.");

            if (!cred.IsActive || potentiallyGoingIntoGracePeriod)
            {
                //Log when applicable
                if (!fiveYearReqMet)
                    LogCertStatusFailure(cred.ExternalId, LookbackReasonType.FiveYear, pendingResultsExist, lookbackDate);

                if (!assessmentReqMet || potentiallyGoingIntoGracePeriod)
                    LogCertStatusFailure(cred.ExternalId, LookbackReasonType.Assessment, pendingResultsExist, lookbackDate);

                if (!attestationReqMet)
                    LogCertStatusFailure(cred.ExternalId, LookbackReasonType.Attestation, pendingResultsExist, lookbackDate);
            }
        }

        /// <summary>
        /// PBI 134117 : Participation Status Evaluation (Technical)
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="lookbackDate"></param>
        /// <param name="logList">logList is used to build logging statements used by YearEndLookback for anyone who fails the criteria in this method.  
        /// Currently corrective action is not required to log, so does nothing with logList upon return from this method</param>
        private bool EvaluateParticipationStatus(Credential credential, DateTime lookbackDate, List<AddParticipationLookbackLog> logList )
        {

            /* 
             Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
                For 2020, 2021, and 2022, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
                *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021 or 2022
                *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021 or 2022
                *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021 or 2022
             ============================================================================================================================================================================================
              Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
                  For 2020 and 2021, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
                  *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021
                  *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021
                  *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021
            ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            */
            if (IsEligibleForCOVIDExtension(lookbackDate, credential.IsCovid4))
                return false;

            if (logList == null)
                logList = new List<AddParticipationLookbackLog>();

            bool assessmentReqMet;
            bool fiveYearReqMet;
            bool attestationReqMet;
            // For grandfather certificates to be considered meeting the participation status
            // There is an issuance that corresponds to the credential being checked with issuance.duration =’Lifetime’ and issuance.issuancestatus =’Active’
            //     They must have met all the exam and non - exam requirements and the 2 year requirements
            //      All of the following must be true
            //          Met the exam requirements (pbi 135089) and
            //          Met all the non - exam requirements(pbi 135009) and
            //          Met the 2 year requirement (pbi 135236)
            if (credential.IsGrandfather  ) // active and grandfather
            {
                var meetsExamRequirements = MeetsExamRequirements(credential, MemberId, lookbackDate, out assessmentReqMet);
                var meetsNonExamRequirements = NonExamRequirement(credential, lookbackDate, ProcessingDate, out fiveYearReqMet, out attestationReqMet, false);
                var meets2YearRequirements = MaintenanceStatus(credential, ProcessingDate, true, true);

                if (meetsExamRequirements && meetsNonExamRequirements && meets2YearRequirements.MeetStepRule)
                {
                    Log.Debug($"Grandfather requirements met for member '{MemberId}', credential '{credential.ExternalId}', lookback date '{lookbackDate.ToShortDateString()}', processing date '{ProcessingDate.ToShortDateString()}'.");
                    return false;
                }
                LogParticipationStatusFailureReasons(assessmentReqMet:assessmentReqMet, fiveYearReqMet: fiveYearReqMet, twoYearReqMet: meets2YearRequirements.MeetStepRule, 
                    attestationReqMet:attestationReqMet, activeCredentialReqMet:true, credential:credential, lookbackDate:lookbackDate, logList:logList);
            }
            /*
             For time-limited certificates to be considered meeting the participation status
             There is NOT an issuance that corresponds to the credential being checked with issuance.duration =’Lifetime’ and issuance.issuancestatus =’Active’ and is an issuance with issuance.duration =’Timelimited’

                // Note: grandfathers can have a time - limited which may or may not be expired and a lifetime cert – so TL is somone with a TL and no active lifetime
                // They must have an active certificate and met the 2 year requirement
                // All of the following must be true
                // Credential.isActive = true and
                // Met 2 year requirement (pbi 135236)
                // Met all the non-exam requirements(pbi 135009)
             */
            else if (credential.IsTimelimited && credential.IsActive)
            {
                //  Met 2 year requirement (pbi 135236)
                var meets2YearRequirements = MaintenanceStatus(credential, ProcessingDate, true, true);
                // Met all the non-exam requirements(pbi 135009)
                var meetsNonExamRequirements = NonExamRequirement(credential, lookbackDate, ProcessingDate, out fiveYearReqMet, out attestationReqMet,false);
                if (meets2YearRequirements.MeetStepRule && meetsNonExamRequirements)
                {
                    Log.Debug($"TL requirements met for member '{MemberId}', credential '{credential.ExternalId}', lookback date '{lookbackDate.ToShortDateString()}', processing date '{ProcessingDate.ToShortDateString()}'.");
                    return false;
                }
                LogParticipationStatusFailureReasons(assessmentReqMet:true, fiveYearReqMet:fiveYearReqMet, twoYearReqMet:meets2YearRequirements.MeetStepRule, 
                    attestationReqMet:attestationReqMet, activeCredentialReqMet:credential.IsActive,credential: credential, lookbackDate: lookbackDate, logList: logList);
            }
            /*
            For mbm certificates to be considered meeting the participation status 
                There is an issuance that corresponds to the credential being checked with issuance.duration=’Continous’
                They must have an active certificate and met the 2 year requirement
                All of the following must be true
                    Credential.isActive=true and
                    Met 2 year requirement (pbi 135236)
            */
            else if (credential.IsMBM && credential.IsActive)
            {
                // Met 2 year requirement (pbi 135236)
                var meets2YearRequirements = MaintenanceStatus(credential, ProcessingDate, true, true);

                if (meets2YearRequirements.MeetStepRule)
                {
                    Log.Debug($"MBM requirements met for member '{MemberId}', credential '{credential.ExternalId}', lookback date '{lookbackDate.ToShortDateString()}', processing date '{ProcessingDate.ToShortDateString()}'.");
                    return false;
                }
                LogParticipationStatusFailureReasons(assessmentReqMet: true, fiveYearReqMet:true, twoYearReqMet:meets2YearRequirements.MeetStepRule, 
                    attestationReqMet:true, activeCredentialReqMet:credential.IsActive, credential:credential, lookbackDate: lookbackDate, logList: logList);
            }
            //PBI 137217: For Time-limited (with no lifetime) and must-be-maintained: Log certification when NOT certified
            //This added clause is just for logging.  InActive MBM/TL creds would fall through to the issuance update below but 
            //we detect the specific case for an inactive cred here and log as such.
            else if ((credential.IsMBM || credential.IsTimelimited) && !credential.IsActive)
            {
                LogParticipationStatusFailureReasons(assessmentReqMet:true, fiveYearReqMet:true, twoYearReqMet:true, attestationReqMet:true, 
                    activeCredentialReqMet:credential.IsActive, credential:credential, lookbackDate:lookbackDate, logList: logList);
            }

            /*
             If Not meeting the participation status          
                set issuance.maintenancestatus=0
                set issuance.modifieddate to the current date
                set issuance.modifledby to ‘ClearMaint’
             */
            // * Requirements not met. Update any active issuance (issuance.issuancestatus=’Active’) that matches the credential being checked

            var issuancesToUpdate = credential.Issuances.Where(x => ( x.IssuanceStatus == IssuanceStatusType.Active 
                        && x.MaintenanceStatus!= MaintenanceStatusType.NotMaintained));

            foreach (var issuance in issuancesToUpdate)
            {
               issuance.MaintenanceStatus= MaintenanceStatusType.NotMaintained;
               issuance.SetModified("ClearMaint");
               issuance.HasChanged = true;
            }

            return credential.Issuances.Any(i => i.HasChanged);
        }

        /// <summary>
        /// PBI 137217: Log Participation Status Not Met - when a credential is not participating we should log all subcomponents that are not met.
        /// There are different requirements for Grandfather/TL/MBM credentials.  In cases where a requirement does not apply to a credential
        /// the corresponding reqMet parameter will be passed in as true to avoid logging in these cases.
        /// </summary>
        /// <param name="assessmentReqMet"></param>
        /// <param name="fiveYearReqMet"></param>
        /// <param name="twoYearReqMet"></param>
        /// <param name="attestationReqMet"></param>
        /// <param name="activeCredentialReqMet"></param>
        /// <param name="credential"></param>
        /// <param name="lookbackDate"></param>
        /// <param name="logList"></param>
        private void LogParticipationStatusFailureReasons(bool assessmentReqMet, bool fiveYearReqMet, bool twoYearReqMet, bool attestationReqMet, 
            bool activeCredentialReqMet, Credential credential, DateTime lookbackDate, List<AddParticipationLookbackLog> logList)
        {
            if (!assessmentReqMet)
                LogParticipationStatusFailure(credential, LookbackReasonType.Assessment, lookbackDate, MemberId, logList);
            if (!fiveYearReqMet)
                LogParticipationStatusFailure(credential, LookbackReasonType.FiveYear, lookbackDate, MemberId, logList);
            if (!twoYearReqMet)
                LogParticipationStatusFailure(credential, LookbackReasonType.TwoYear, lookbackDate, MemberId, logList);
            if (!attestationReqMet)
                LogParticipationStatusFailure(credential, LookbackReasonType.Attestation, lookbackDate, MemberId, logList);
            if (!activeCredentialReqMet)
                LogParticipationStatusFailure(credential, LookbackReasonType.Certification, lookbackDate, MemberId, logList);
        }

        private bool ShouldWeRunCorrectiveAction(
            IssuanceStatusType? startingIssuanceStatus, 
            IssuanceStatusType? revisedIssuanceStatus, 
            MaintenanceStatusType? startingMaintStatus, 
            MaintenanceStatusType? revisedMaintStatus)
        {
            //PBI 134881
            //If we have changed the certification from active to expired or participation status 
            //was set to not met during the lookback, Invoke corrective action with an event date 
            //of 1/1/current year
            return
                (startingIssuanceStatus == IssuanceStatusType.Active && revisedIssuanceStatus == IssuanceStatusType.Expired)
                ||
                (startingMaintStatus != MaintenanceStatusType.NotMaintained && revisedMaintStatus == MaintenanceStatusType.NotMaintained);
        }

        private void RecordLookbackDate(Credential cred, DateTime lookbackDate)
        {
            //PBI 134881
            cred.LookbackDate = lookbackDate;
            cred.SetModified("YearEnd"); //This also sets the modified date
        }

        private void Update(Credential cred)
        {
            var command = new UpdateCredentialFromLookbackCommand
            {
                Credential = cred, //Because we need to save this as we modified it: expired issuances, CredentialDateLog objects, and all
                ModifiedBy = "YearEnd"
            };
            CredentialService.Handle(command);
        }

        /// <summary>
        /// Determines whether or not pending exam results exist for a specific 
        /// member and certification as they
        /// </summary>
        /// <param name="certificationId">The external ID of the certification</param>
        /// <param name="lookbackDate">The lookback date</param>
        /// <returns>A Task&lt;bool&gt;indicating the existence or lack of pending 
        /// exam results for the specified member and certification as they 
        /// relate to a lookback</returns>
        /// <remarks>This method was coded initially as public to allow for 
        /// unit tests of it prior to the implementation of the methods that 
        /// would make use of it. This method could be refactored to private 
        /// after the implementing methods have been completed, or left public 
        /// if it is deemed useful for other purposes.</remarks>
        private bool DoPendingExamResultsExistForCertification(
            Guid certificationId, 
            DateTime lookbackDate)
        {
            //PBI 134903
            /*
            Determine if any registration.registration record exists 
            that has a corresponding registration.exam that matches the 
            registration.certification of credential being evaluated where:
                The registration.exam.examtype in moc,kci
                The registration.administration.administrationyear <= year of 
                current lookback date
                The registration.examresult is Pending or Hold or PendingAdditionalTake

            If any exists, there is a pending result otherwise there isn’t      
            */
            string[] validExamTypes = new string[] { "MOC", "KCI" };
            string[] pendingResultTypes = new[] { "PENDING", "HOLD", "PENDINGADDITIONALTAKE" };

            //A note about RegistrationResource.Result shown below: Registration Platform's AutoMapper mapping for this 
            //property sets it to the value of the domain Registration object's Result.Result (dest.Result = src.Result.Result.Value.ToString();).
            //There's also an ExamResult property on RegistrationResource which is a more complex type.
            var output = 
                Registrations
                    .Where(reg => reg.CertificationId == certificationId)
                    .Any(reg =>
                        validExamTypes.Contains(reg.ExamType.Value.ToUpper())
                        && reg.AdministrationYear <= lookbackDate.Year
                        && reg.Result != null && pendingResultTypes.Contains(reg.Result.ToUpper()));

            Log.Debug($"Pending exam results {(output ? "exist" : "don't exist")} for memberId {MemberId}, certificationId {certificationId}, lookbackDate {lookbackDate}");

            return output;
        }

        private bool MeetsExamRequirements(Credential cred, Guid memberId, DateTime lookbackDate, out bool assessmentReqMet)
        {
            //PBI 135089
            /*
             * The exam requirements are met if any of the following are true:
             * - Credential.AssessmentMet = true and Credential.AssessmentMetDate <= lookback date
             * - Credential.GracePeriodEndDate is not null and is >= current lookback date and Credential.GracePeriodStartDate <= lookback date and not null
             * - Has pending exam results
            */

            assessmentReqMet = (cred.AssessmentMet && cred.AssessmentMetDate <= lookbackDate);

            //Subtracting one day from Grace Period Start per recommendation from Don after we discovered during 
            //UAT that this check would never pass due to Grace Period being 1/1/2019 - 12/31/2019, but lookback 
            //date was 12/31/2018. :/
            bool inGracePeriod =
                (cred.GracePeriodStartDate.HasValue && cred.GracePeriodEndDate.HasValue
                && cred.GracePeriodStartDate.Value.AddDays(-1) <= lookbackDate
                && cred.GracePeriodEndDate >= lookbackDate);

            bool pendingExamResultsExist = DoPendingExamResultsExistForCertification(cred.Certification.ExternalId, lookbackDate);

            bool meetsReqs = (assessmentReqMet || inGracePeriod || pendingExamResultsExist);

            Log.Debug($"Credential {cred.ExternalId} for member {memberId} {(meetsReqs ? "meets" : "does not meet")} requirements for lookback date {lookbackDate}. Assessment Met OK = {assessmentReqMet}, In Grace Period = {inGracePeriod}, Pending Exam Results Exist = {pendingExamResultsExist}.");

            return meetsReqs;
        }

        private bool IsPotentiallyGoingIntoGracePeriod(Credential cred, DateTime lookbackDate, Guid memberId)
        {

            if (!cred.HasIssuances) return false;

            //PBI 137216
            /*
            Potentially going into the grace period is defined as time-limited, no other lifetime issuances, 
            expiring at the date of the lookback. This equates to:

            Issuance.duration=Timelimited and
            issuance.expirationdate=current lookback date and
            issuance.issuancestatus=Active
            pending result (pbi 134903) and
            no issuance for credential that has Issuance.duration=Lifetime and issuance.issuancestatus=Active
            */
            var latestIssuance = cred.NewestIssuance;

            var output = latestIssuance.Duration == DurationType.Timelimited
                && latestIssuance.ExpirationDate.HasValue
                && latestIssuance.ExpirationDate.Value == lookbackDate
                && latestIssuance.IssuanceStatus == IssuanceStatusType.Active
                && DoPendingExamResultsExistForCertification(cred.Certification.ExternalId, lookbackDate)
                && !cred.Issuances.Any(x => x.Duration == DurationType.Lifetime && x.IssuanceStatus == IssuanceStatusType.Active);

            Log.Debug($"Credential {cred.Id} for member {memberId}, lookback date {lookbackDate} {(output ? "potentially" : "not")} going into grace period");

            return output;
        }

        private void LogCertStatusFailure(Guid credId, LookbackReasonType failureReason, bool pendingResultsExist, DateTime lookbackDate)
        {
            var cmd = new AddCertificationLookbackLog
            {
                CredentialId = credId,
                Action = LookbackActionType.FailurePoint,
                Reason = failureReason,
                //Status is set by the Handle() method
                IsPendingAction = pendingResultsExist,
                LookbackLogDate = lookbackDate,
                UserName = "CertYearly"
            };

            var result = (AddLookbackLogCommandResult)LookbackLogService.Handle(cmd);
            if (!result.Succeeded)
            {
                string error = $"Failed to add LookbackLog record: {result.Message}. Command = {cmd.Dump()}";
                Log.Error(error);
                throw new ApplicationException(error);
            }
        }

        private void LogParticipationStatusFailure(Credential cred, LookbackReasonType failureReason, DateTime lookbackDate, Guid memberId, List<AddParticipationLookbackLog> logList)
        {
            bool pendingResultsExist =
                DoPendingExamResultsExistForCertification(cred.Certification.ExternalId, lookbackDate);

            var cmd = new AddParticipationLookbackLog
            {
                CredentialId = cred.ExternalId,
                Action = LookbackActionType.FailurePoint,
                Reason = failureReason,
                //Status is set by the Handle() method
                IsPendingAction = pendingResultsExist,
                LookbackLogDate = lookbackDate,
                UserName = "ParticipationYearly"
            };

            //LookbackLogService.Handle(cmd);
            logList.Add(cmd);
        }

        private async Task KickOutOfCMPIfApplicable(Credential credential, DateTime lookbackDate)
        {
            try
            {
                if (credential.Pathway == Resources.PathwayType.OneYear && credential.IsInCMP && credential.ExamDueDate <= lookbackDate
                    // Keep lookbackDate as 2020 even it is in the past just in case we would re-run for some diplomates
                    && !(( credential.ExamDueDate?.Year == 2020 || credential.ExamDueDate?.Year == 2021) && (lookbackDate.Year == 2020 || lookbackDate.Year == 2021)))  //PBI 209119 : (Release 2.35) CMP (ACC) Pathway – COVID Extension
                {
                    Log.Debug($"Removing credential {credential.ExternalId} for member {credential.MemberId} from CMP.");
                    var command = new UnEnrollInCMPCommand();

                    command.MemberId = credential.MemberId;
                    command.RequestingUserName = "KickCMP";
                    command.SubspecialtyCertCode = credential.Certification.Code;
                    command.UnEnrollmentDate = DateTime.Now;

                    //It doesn't appear we actually make use of this anymore, but command validation is still requiring it, 
                    //so I'm supplying a value
                    command.UserInfo = new UserInfo { Username = "YearEndLookBack" };

                    var result = await CredentialService.Handle(command);
                    if (!result.Succeeded)
                        throw new ApplicationException($"Attempt to unenroll user {credential.MemberId} credential {credential.ExternalId} from CMP failed: {result.FailureReason.ToString()}");

                    credential.SetModified("KickCMP"); //This ends up getting overwritten when we set the lookback date in RecordLookbackDate().
                                                       //If we find records in the database with this ModifiedBy value, it means the process crapped out before 
                                                       //it could set the lookback date. :O
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error attempting to unenroll credential {credential.ExternalId} for user {credential.MemberId} from CMP!");
                throw;
            }
        }

        private bool IsEligibleForCOVIDExtension(DateTime lookBackDate, bool credentialIsInCOVID4)
        {
            return lookBackDate.Year == 2021 || (lookBackDate.Year == 2022 && credentialIsInCOVID4);
        }
    }
}
