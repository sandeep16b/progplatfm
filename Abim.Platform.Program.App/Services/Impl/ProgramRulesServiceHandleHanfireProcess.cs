using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Enums;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PathwayType = Abim.Platform.Program.Resources.PathwayType;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService
    {
        private static readonly string[] EffectivePassingResultTypes =
            new string[] { "FAIL", "INDETERMINATE", "INCOMPLETE", "UNABLETOTEST" };

        #region Local Run Functions
        /// <summary>
        ///  When an abim physician passes an initial certification exam, a new credential/issuance should be created
        ///  PBI 94844:back end process: create credential/issuance on passing initial cert
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        private Task IssueNewCredentialOnPassingInitialCert_(RegistrationResource registration,
                                                           DateTime processingDate)
        {
            #region declarations
            var inputs = $"RegistrationId={registration.Id}, CertificationId:{registration.CertificationId}, MemberId={registration.MemberId}, Result={registration.Result}, processingDate={processingDate.ToShortDateString()}";
            #endregion

            try
            {

                var certification = CertificationService.Load(registration.CertificationId);

                if (certification == null)
                {
                    Log.Error($"CertificationId '{registration.CertificationId}' is not found");
                    return Task.FromResult<object>(null);
                }

                DateTime AssessmentMetDate = registration.ExamTestDate();

                string onBehalfBoardName = null;
                MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.NotMaintained;

                // it is CoSponsored registration if OnBehalfOf is not empty 
                if (!String.IsNullOrWhiteSpace(registration.OnBehalfOf))
                    onBehalfBoardName = GetOnBehalfBoardName(registration.OnBehalfOf);
                else // determine MaintenanceStatus only for regular diplomate registration (not CoSponsored as above)
                    maintenanceStatus = DetermineMaintenanceStatusOnPassingInitialCert_(certification.Code, processingDate);

                var command = new CreateCredentialIssuanceCommand()
                {
                    CertificationId = certification.ExternalId,
                    MemberId = MemberId,
                    Type = certification.Code == "IM" ? CredentialType.General : CredentialType.Subspecialty,
                    Pathway = PathwayType.MOC,
                    AssessmentMetDate = AssessmentMetDate,
                    ExamDueDate = new DateTime(AssessmentMetDate.Year + 10, 12, 31),
                    DisplayExamDueDate = new DateTime(AssessmentMetDate.Year + 10, 12, 31), //PBI 135987
                    KCIExamDueDate = new DateTime(AssessmentMetDate.Year + 10, 12, 31),//PBI 135987
                    MOCExamDueDate = new DateTime(AssessmentMetDate.Year + 10, 12, 31),//PBI 135987
                    ConsecutiveKCIPassRequired = false,//PBI 135987
                    MaintenanceStatus = maintenanceStatus,
                    ScheduledUpdate = ProgramRulesHelpers.ComputeScheduleUpdateDateForIssuance(processingDate),
                    CredentialCreatedBy = "NewInitial",
                    IssuanceCreatedBy = "NewInitial", 
                    OnBehalfBoardCode = registration.OnBehalfOf,    //PBI 215404
                    OnBehalfBoardName = onBehalfBoardName           //PBI 215404
                };

                if (certification.IsICARD())
                    command.ReAttestationDueDate = new DateTime(AssessmentMetDate.Year + 10, 12, 31);

                var result = (CreateCredentialIssuanceCommandResult)(CredentialService.Handle(command));

                if (result.Succeeded)
                    Logger.Debug($"Added New Credential To DB: {inputs}");
                else if (result.Status == CommandStatus.Rejected)
                    Log.Error($"IssueNewCredentialOnPassingInitialCert's call to Handle(CreateCredentialIssuanceCommandResult) failed with the following error:'{result.Message}'");

            }
            catch (Exception ex)
            {

                Logger.Error(ex);
                throw;
            }

            return Task.FromResult<object>(null);
        }

        #region Update Credential On Loading Exam Result
        private void UpdateCredentialOnLoadingExamResult(RegistrationData registration, ref Credential credential, DateTime processingDate)
        {
            //PBI 134151
            try
            {

                DateTime examTestDate = registration.MinSeatOrDeliveryDate;
                ExamResultType effectiveExamResult;
                if (credential.ExamDueDate.HasValue)
                    effectiveExamResult = registration.GetEffectiveExamResult(credential.ExamDueDate.Value, credential.ConsecutiveKCIPassRequired);
                else
                    effectiveExamResult = registration.ExamResult.Result.ToEnum();

                SetCredentialPathwayFromExamResultIfApplicable(
                    credential, registration.GetExamType(), registration.ExamResult.Result.ToEnum());

                SetCredentialDueDatesFromExamResult(
                    credential, registration, effectiveExamResult, examTestDate);

                //For Bug 160589
                bool expireActiveIssuances = ShouldWeExpireActiveIssuances(registration, credential, processingDate);

                if (credential.HasChanged)
                    UpdateCredentialOnExamResult(ref credential, expireActiveIssuances, registration.AdministrationDate);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error updating credential on loading exam result");
                throw;
            }
        }

        private void SetCredentialPathwayFromExamResultIfApplicable(
            Credential cred, string examTypeValue, ExamResultType actualExamResult)
        {
            bool pathwayChanged = false;

            Log.Info($"In SetCredentialPathwayFromExamResultIfApplicable() for credential {cred.ExternalId}");

            //The following exam results are eligible for changing the pathway
            bool eligibleExamResult =
                actualExamResult == ExamResultType.Pass
                || actualExamResult == ExamResultType.Fail
                || actualExamResult == ExamResultType.Indeterminate
                || actualExamResult == ExamResultType.Incomplete
                || actualExamResult == ExamResultType.UnableToTest
                || actualExamResult == ExamResultType.Invalidated;

            examTypeValue = examTypeValue.ToUpper();

            //Took the MOC exam and they are on the KCI pathway 
            if (eligibleExamResult && examTypeValue == "MOC" && cred.Pathway != PathwayType.MOC)
            {
                //Switch to 10 year pathway
                Log.Info($"Changing pathway for credential {cred.ExternalId} to MOC.");
                cred.Pathway = PathwayType.MOC;

                // Bug 230675 : Exam Result Processing Switches Pathway But Does Not Clear Is In CMP Flag
                cred.IsInCMP = false;

                //PBI 150794 - No longer change DisplayExamDueDate here, was max DisplayExamDueDate/MOCExamDueDate
                pathwayChanged = true;
            }

            //Took the KCI and they are on the MOC pathway
            else if (eligibleExamResult && examTypeValue == "KCI" && cred.Pathway != PathwayType.KCI)
            {
                //Switch to 2 year pathway
                Log.Info($"Changing pathway for credential {cred.ExternalId} to KCI.");
                cred.Pathway = PathwayType.KCI;
                // Bug 230675 : Exam Result Processing Switches Pathway But Does Not Clear Is In CMP Flag
                cred.IsInCMP = false;
                pathwayChanged = true;
            }
            else if (eligibleExamResult && examTypeValue == "ACC" && cred.Pathway != PathwayType.OneYear)
            {
                //PBI 150806
                //Switch to 1 year pathway
                Log.Info($"Changing pathway for credential {cred.ExternalId} to 1 YEAR.");
                cred.Pathway = PathwayType.OneYear;
                pathwayChanged = true;
            }

            if (pathwayChanged)
            {
                cred.SetModified("ExamResult");
                cred.HasChanged = true;
            }
        }

        private void SetCredentialDueDatesFromExamResult(
            Credential cred,
            RegistrationData registration,
            ExamResultType effectiveExamResult,
            DateTime examDate)
        {
            Log.Info($"In SetCredentialDueDatesFromExamResult() for registration {registration.Id}");

            var adminYear =
                (registration.AdministrationYear != 0 ? registration.AdministrationYear : examDate.Year);

            if (registration.IsMoc && effectiveExamResult == ExamResultType.Pass)
            {
                //They have 10 more years and they met the assessment requirements
                cred.ExamDueDate = new DateTime(adminYear + 10, 12, 31);
                cred.DisplayExamDueDate = new DateTime(adminYear + 10, 12, 31);
                cred.KCIExamDueDate = new DateTime(adminYear + 10, 12, 31);
                cred.MOCExamDueDate = new DateTime(adminYear + 10, 12, 31);
                cred.SetModified("ExamResult");
                SetAssessmentMet(cred, examDate);
                cred.HasChanged = true;
                Log.Info($"Registration {registration.Id} is MOC and PASS");
            }

            // PBI 151802 : Update Assessment Due Date After 2nd KCI Pass 
            // If a diplomate is in the grace period or whose certificate status is currently Not Certified, due to not meeting the assessment requirement , 
            //  their assessment due date will not advance until they pass their second consecutive KCI.  
            //                      Note: current functionality is to advance the assessment due date after the first KCI pass."
            // Unit tests in UpdateCredentialOnLoadingKCIExamResultOnlySpec.cs
            else if (registration.IsKci &&
                     (cred.IsInGracePeriod || (cred.HasIssuances && cred.NewestIssuance.IssuanceStatus != IssuanceStatusType.Active && !cred.AssessmentMet)))
            {
                // we need only PASS and KCI (meaning previous exam was KCI too)
                if (effectiveExamResult == ExamResultType.Pass && cred.Pathway == PathwayType.KCI)
                {
                    // find if previous KCI exam
                    var previousKCI = Registrations
                            .Where(x => x.CertificationId == cred.Certification.ExternalId)
                            .Where(n => n.Id != registration.Id)
                            .Where(m => m.IsKci())
                            .Where(t => t.ExamTestDate() < registration.ExamTestDate) // exam was before current Test date
                            .Where(t => t.ExamTestDate().Year >= ProcessingDate.Date.AddYears(-4).Year) // within last 4 years
                            .Where(r => r.IsPassExam() || r.IsFailExam()) //"Only consider PASS or FAIL when determining consecutive PASSes"
                            .OrderByDescending(a => a.AdministrationDate)
                            .FirstOrDefault();

                    // if most recent KCI exam is pass
                    if (previousKCI != null && previousKCI.IsPassExam())
                    {
                        cred.SetModified("UpdateAssessment2Pass");

                        cred.ExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.ExamDueDate }.Max();
                        cred.KCIExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.ExamDueDate }.Max();
                        cred.DisplayExamDueDate = cred.ExamDueDate;
                        SetAssessmentMet(cred, examDate);

                        Log.Info($"KCI Assessment Requirement was met for {registration.Id}  since the diplomate {cred.MemberId} passes 2nd KCI.");
                    }
                    else
                    {
                        Log.Info($"KCI Assessment Requirement was NOT met for {registration.Id}  since the diplomate does not have previous KCI pass.");
                    }

                }
                else
                {
                    //*** do  nothing, don't advance exam due date
                    Log.Info($"TL Cert in Grace period or expired and current exam is not pass. Don't change anything  for the diplomate {cred.MemberId}.");
                }

            }

            /* ~~~~ Bug 165060 : reverse below PBI ~~~~
            // (PBI 151802 : Update Rule 30 KCI Assessment Requirement for Time Limited Certs
            // Description : "Currently, diplomates with Time Limited certificates who are in the grace period and are taking two KCIs to meet their assessment requirements are having their assessment due dates advanced after the pass of the first KCI. 
            // The due date should remain the cert expiration date until after the diplomate passes the second KCI."
            //  We try early to catch KCI exam and TL cert and in Grace period  and rest of the logic don't need to be changed
            else if (registration.IsKci &&
                     cred.IsTimelimited &&
                     cred.IsInGracePeriod)
            {
                // we need only PASS and KCI (meaning previous exam was KCI too)
                if (effectiveExamResult == ExamResultType.Pass && cred.Pathway== PathwayType.KCI)
                {
                    // find if previous exam was pass KCI 
                    var previousKCI = Registrations
                            .Where(x => x.CertificationId == cred.Certification.ExternalId)
                            .Where(n => n.Id != registration.Id)
                            .Where(m => m.IsKci())
                            .Where(k => k.IsPassExam())
                            .Where(t => t.ExamTestDate() < registration.ExamTestDate) // exam was before current Test date
                            .Where(t => t.ExamTestDate().Year >= ProcessingDate.Date.AddYears(-4).Year) // within last 4 years
                            .OrderByDescending(a => a.AdministrationDate)
                            .FirstOrDefault();

                    if (previousKCI != null)
                    {
                        cred.SetModified("ExamResult_Rule30");

                        cred.ExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.ExamDueDate }.Max();
                        cred.KCIExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.KCIExamDueDate }.Max();
                        SetAssessmentMet(cred, examDate);

                        Log.Info($"KCI Assessment Requirement was met for {registration.Id}  since the diplomate {cred.MemberId} passes 2 KCI.");
                    }

                }
                else
                {
                    //*** do  nothing, don't advance exam due date
                    Log.Debug($"TL Cert in Grace period and it is not pass. Don't change anything  for the diplomate {cred.MemberId}.");
                }

            }
            */

            /*
            PBI 178847 : (Proj 1474) Display Consequential Due Dates for KCI exams:
            •	Pass (Consequence or No Consequence) exam = + 4 years from Exam date year
            •	Fail/Ind/Inc/Utt (No Consequence) exam = + 2 years from Exam date year
            •	Fail/Ind/Inc/Utt (Consequence) exam = no changes to Due Date

            */
            else if (registration.IsKci && effectiveExamResult == ExamResultType.Pass)
            {

                // PBI 178847 Proj 1474) Display Consequential Due Dates for Two Year Assessments
                // *** for failed NoConsequence  exam diplomate get 2 years
                // since Fail/Inc/Ind/Utt for NoConsequence would be counted as Pass in  effectiveExamResult before.
                if (registration.NoConsequence && 
                    (      registration.ExamResult.Result.ToEnum() == ExamResultType.Fail  
                        || registration.ExamResult.Result.ToEnum() == ExamResultType.Incomplete
                        || registration.ExamResult.Result.ToEnum() == ExamResultType.Indeterminate
                        || registration.ExamResult.Result.ToEnum() == ExamResultType.UnableToTest) &&
                    !cred.ConsecutiveKCIPassRequired) // just for readibility of the code, cannot be ConsecutiveKCIPassRequired=true here with Pass results
                {
                    Log.Info($"Registration {registration.Id} is KCI and NoConsequence and result Fail/Inc/Ind/Utt; adding 2 years");
                    cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 2, 12, 31), cred.DisplayExamDueDate }.Max();
                }
                else if (!cred.ConsecutiveKCIPassRequired) // this if/else only for NOT ConsecutiveKCIPassRequired.
                {

                    // PBI 178847 Proj 1474) Display Consequential Due Dates for Two Year Assessments
                    // **** If the assessment due date was in 2018 and the diplomate passed a two-year assessment in 2018 then the assessment due date is 2022. 
                    // for each pass they get 4 years from admin date, before was this formula = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.DisplayExamDueDate }.Max();
                    //  but that would not work for Test case 1 : Pass in  2018 (moved to 2022) and then Pass in 2020 (moved to 2024).
                    Log.Info($"Registration {registration.Id} is KCI and Pass; adding 4 years");
                    cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.DisplayExamDueDate }.Max();
                    //ar@3/11/2021: replaced with the code above
                    // cred.DisplayExamDueDate = new DateTime(adminYear + 4, 12, 31);
                }

                /* ar@7/7/2020 removed but please keep it for references
                //PBI 150794 Update the display due date to the maximum of adminYear+2, mocExamDueDate
                //Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate
                //Revised again to be greatest of adminYear+2 or current DisplayDueDate
                cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 2, 12, 31), cred.DisplayExamDueDate }.Max();
                */

                //If required to pass two consecutive kci
                if (cred.ConsecutiveKCIPassRequired)
                {
                    //Is this the first pass?
                    if ((!cred.ExamDueDate.HasValue) || ((cred.ExamDueDate.HasValue) && (adminYear > cred.ExamDueDate.Value.Year)))
                    {
                        //Due date is 2 more years to try to pass the 2nd one
                        cred.ExamDueDate = cred.MOCExamDueDate = cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 2, 12, 31), cred.ExamDueDate }.Max();
                        //ar@3/11/2021: replaced with the code above
                        //cred.ExamDueDate = new DateTime(adminYear + 2, 12, 31);
                        //cred.MOCExamDueDate = new DateTime(adminYear + 2, 12, 31);

                        Log.Info($"Registration {registration.Id} is KCI and PASS (consecutive KCI pass required, first attempt)");
                    }
                    else
                    {
                        //Else due date is 4 years to pass next kci and they have met the assessment requirement
                        cred.ExamDueDate = cred.MOCExamDueDate = cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.ExamDueDate }.Max();
                        //ar@3/11/2021: replaced with the code above
                        //cred.ExamDueDate = new DateTime(adminYear + 4, 12, 31);
                        //cred.MOCExamDueDate = new DateTime(adminYear + 4, 12, 31);

                        SetAssessmentMet(cred, examDate);
                        Log.Info($"Registration {registration.Id} is KCI and PASS (consecutive KCI pass required, not first attempt)");
                    }
                }
                else
                {
                    /* 
                    //They have 4 years to pass again extended thru the date their 10 year pass is good thru and 
                    //they’ve met the assessment requirements.
                    //Due date is 12/31/admin year + 4 of ExamDueDate, whichever is greater
                    //(Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate)
                    var examDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.ExamDueDate }.Max();
                    var kciExamDueDate = new List<DateTime?> { new DateTime(adminYear + 4, 12, 31), cred.KCIExamDueDate }.Max();
                    cred.ExamDueDate = examDueDate;
                    cred.KCIExamDueDate = kciExamDueDate;
                    */

                    // PBI 178847 Proj 1474) Display Consequential Due Dates for Two Year Assessments
                    // *** pretty much as it was before but it is more clear ...
                    cred.ExamDueDate = cred.KCIExamDueDate = cred.MOCExamDueDate = cred.DisplayExamDueDate;

                    SetAssessmentMet(cred, examDate);
                    Log.Info($"Registration {registration.Id} is KCI and PASS (effectiveExamResult) (cred.ConsecutiveKCIPassRequired=false)");
                }

                cred.SetModified("ExamResult");
                cred.HasChanged = true;
            }
            // PBI 178847 Proj 1474) Display Consequential Due Dates for Two Year Assessments
            // for Fail diplomate did not advance due date
            else if (registration.IsKci && effectiveExamResult == ExamResultType.Fail)
            {
                //If required to pass 2 kcis in a row(Credential.ConsecutiveKCIPassRequired )
                if (cred.ConsecutiveKCIPassRequired)
                {
                    //Start the cycle over again.
                    //User has no definitive date, they need to pass as they are already negative.
                    cred.ExamDueDate = null;
                    cred.MOCExamDueDate = null;
                    cred.HasChanged = true;
                    Log.Info($"Registration {registration.Id} is KCI and FAIL (consecutive KCI pass required)");
                }
                else
                {
                    //PBI 150794 Update the display due date to the maximum of adminYear+2, mocExamDueDate
                    //Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate
                    //Revised again to be greatest of adminYear+2 or current DisplayDueDate
                    //cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 2, 12, 31), cred.DisplayExamDueDate }.Max();
                    //Revised again per PSR 173312 - Per the business, we should not be advancing the DisplayExamDueDate 
                    //if they failed.
                    Log.Info($"Registration {registration.Id} is KCI and FAIL (consecutive KCI pass not required)");
                }

                cred.SetModified("ExamResult");

            }
            // PBI 178847 Proj 1474) Display Consequential Due Dates for Two Year Assessments
            //  IND, INC and UTT is considered Unsuccessfull
            else if (registration.IsKci
                && (effectiveExamResult == ExamResultType.Indeterminate
                || effectiveExamResult == ExamResultType.Incomplete
                || effectiveExamResult == ExamResultType.UnableToTest))
            {
                //This is the section that was revised after 134151 was started
                if (cred.ConsecutiveKCIPassRequired)
                {
                    //If they've already passed one
                    if ((cred.ExamDueDate.HasValue) && (adminYear <= cred.ExamDueDate.Value.Year))  //!IsExamAttemptFirstPass(adminYear, cred.ExamDueDate.Value.Year))
                    {
                        //They get 2 more years to keep trying
                        cred.ExamDueDate = new DateTime(adminYear + 2, 12, 31);
                        cred.KCIExamDueDate = new DateTime(adminYear + 2, 12, 31);
                        cred.DisplayExamDueDate = new DateTime(adminYear + 2, 12, 31);
                        cred.SetModified("ExamResult");
                        cred.HasChanged = true;
                        Log.Info($"Registration {registration.Id} is KCI and {effectiveExamResult} (consecutive KCI pass required and this is not their first pass)");
                    }
                }
                else
                {
                    /* per PBI 178847 : (Proj 1474) Display Consequential Due Dates for Two Year Assessments
                     * **** •	Fail/Ind/Inc/Utt (Consequence) exam = no changes to Due Date
                    //Not required to pass 2 in a row
                    //PBI 150794 Update the display due date to the maximum of adminYear+2, mocExamDueDate
                    //Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate
                    //Revised again to be greatest of adminYear+2 or current DisplayDueDate           
                    cred.DisplayExamDueDate = new List<DateTime?> { new DateTime(adminYear + 2, 12, 31), cred.DisplayExamDueDate }.Max();
                    cred.SetModified("ExamResult");
                    cred.HasChanged = true;
                    */

                    Log.Info($"Registration {registration.Id} is KCI and {effectiveExamResult} (consecutive KCI pass not required)");
                }
            }
            else if (registration.IsCmp)
            {
                //PBI 150806
                //Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate
                var dueDate = new List<DateTime?> { new DateTime(adminYear + 1, 12, 31), cred.ExamDueDate }.Max();
                //Revised again for DisplayDueDate to be greatest of adminYear+2 or current DisplayDueDate
                var displayDueDate = new List<DateTime?> { new DateTime(adminYear + 1, 12, 31), cred.DisplayExamDueDate }.Max();

                if (effectiveExamResult == ExamResultType.Pass)
                {
                    cred.ExamDueDate = dueDate;
                    cred.DisplayExamDueDate = displayDueDate;
                    SetAssessmentMet(cred, examDate);
                    cred.HasChanged = true;
                }

                //PBI 150806 had logic to update the display due date if the result was Fail or Unable to Test (IND/INC 
                //aren't supported for CMP), but this was removed per the subsequent PBI 158548.

                if (cred.HasChanged)
                {
                    cred.SetModified("ExamResult");
                    Log.Info($"Registration {registration.Id} is CMP and {effectiveExamResult}");
                }
            }

            Log.Info($"Dates for credential {cred.ExternalId}: ExamDueDate = {cred.ExamDueDate}, DisplayExamDueDate = {cred.DisplayExamDueDate}, MOCExamDueDate = {cred.MOCExamDueDate}, and KCIExamDueDate = {cred.KCIExamDueDate}.");
        }

        private void SetAssessmentMet(Credential cred, DateTime examDate)
        {
            if (cred.ConsecutiveKCIPassRequired)
            {
                //Mark the credential as not requiring 2 consecutive kci passes
                cred.ConsecutiveKCIPassRequired = false;
                cred.HasChanged = true;
            }

            if (!cred.AssessmentMet)
            {
                //Mark the crediential as having met the assessment requirement
                cred.AssessmentMet = true;
                cred.AssessmentMetDate = examDate.Date;
                cred.HasChanged = true;
            }

            //If in the grace period(Credential.GracePeriodStartDate <> null or Credential.GracePeriodEndDate <> null)
            if (cred.GracePeriodStartDate.HasValue || cred.GracePeriodEndDate.HasValue)
            {
                //exit the grace period
                cred.GracePeriodStartDate = null;
                cred.GracePeriodEndDate = null;
                cred.HasChanged = true;
            }

            Log.Debug($"Credential {cred.ExternalId} has ConsecutiveKCIPassRequired = {cred.ConsecutiveKCIPassRequired}, AssessmentMet = {cred.AssessmentMet}, AssessmentMetDate = {cred.AssessmentMetDate}, GracePeriodStartDate = {cred.GracePeriodStartDate} and GracePeriodEndDate = {cred.GracePeriodEndDate}.");
        }

        #endregion Update Credential On Loading Exam Result


        #endregion

        #region Run Functions (from Hanfire Jobs)
        /// <summary>
        ///  creates an FPHM credential for the member if it does not already exist.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        public Task RunCreateNewCredentials(Guid memberId, DateTime processingDate)
        {
            try
            {
                var credential = CredentialService.GetCredentialByMemberAndCode(memberId, "HOSP");
                if (credential != null)
                {
                    Log.Warn($"A credential by {memberId} and code FPHM already exists.");
                    return Task.FromResult<object>(null);
                }

                var IMcredential = CredentialService.GetCredentialByMemberAndCode(memberId, "IM");
                if (IMcredential == null)
                {
                    Log.Error($"An IM credential by {memberId} does not exists.");
                    return Task.FromResult<object>(null);
                }
                MemberId = memberId;
                ProcessingDate = processingDate;
                var command = new CreateCredentialCommand
                {
                    CertificationId = FPHMCertificateGuid,
                    MemberId = memberId,
                    Type = CredentialType.General,
                    IsActive = false,
                    Pathway = PathwayType.MOC,

                    //These values set per PBI 135988
                    ExamDueDate = DateTime.MaxValue,
                    MOCExamDueDate = null,
                    KCIExamDueDate = null,
                    DisplayExamDueDate = null,
                    ConsecutiveKCIPassRequired = false,

                    ReAttestationDueDate = new DateTime(processingDate.Year + 3, 12, 31),
                    UserInfo = new UserInfo
                    {
                        Username = "NewFPAttest"
                    },
                    ProcessingDate = processingDate
                };

                var result = (CreateCredentialCommandResult)(CredentialService.Handle(command));

                if (result.Succeeded)
                    Logger.Debug($"[PBI:92099] Added New Credential To DB: {command.Dump()}");
                else if (result.Status == CommandStatus.Rejected)
                    Log.Error($"RunCreateNewCredentials's call to Handle(CreateCredentialCommand) failed with the following error: {result.Message}");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationId"></param>
        /// <param name="processingDate"></param>
        /// <param name="examRegistration"></param>
        /// <returns></returns>
        public async Task RunProcessesOnExamResultEvent(Guid registrationId,
                                                  DateTime processingDate,
                                                  ExamRegistrationType examRegistration)
        {

            #region declarations
            Log.Debug($"Started 'RunProcessesOnExamResultEvent' with parameters registrationId='{registrationId}', processingDate='{processingDate.ToShortDateString()}'");
            #endregion

            try
            {

                RegistrationResource registration;
                RegistrationData regData = GetRegistrationData(registrationId, examRegistration, out registration);


                if (regData == null)
                {
                    Log.Error($"Registration Id {registrationId} is not found");
                    return;
                }

                MemberId = regData.MemberId;
                ProcessingDate = processingDate;
                TriggeringEvent triggeringEvent = regData.GetExamType() == "CERT" ? TriggeringEvent.ExamResultInitial : TriggeringEvent.ExamResultMocKci;

                if (ShouldWeIssueNewCredential(registration))
                {
                    //PBI:94844 create credential/issuance on passing initial cert
                    await IssueNewCredentialOnPassingInitialCert_(registration, processingDate);
                }

                await RunCorrectiveActionForMember(MemberId, regData.ExamTestDate, processingDate, triggeringEvent, regData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

            return;
        }

        /// <summary>
        /// Program Rules Processor for RunRulesForMustBeMaintainedCertificateCommand
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public void HandleJob(RunRulesForMustBeMaintainedCertificateCommand command)
        {
            Logger.Debug("*Start HandleJob:RunRulesForMustBeMaintainedCertificateCommand: {0}", command.Dump());
            try
            {
                var credential = CredentialService.Load(command.CredentialId);

                MemberId = credential.MemberId;
                ProcessingDate = command.ProcessingDate;
                DateTime evaluationDate = command.ProcessingDate;

                IssueNewCredentialForTLPC_(new List<Credential>() { credential },
                                            credential.MemberId,
                                            command.EventDate,
                                            command.ProcessingDate)
                   .UpdateResult((c, p) => UpdateTLPCCredential(c, p), ProcessingDate)
                   .LogCorrectiveActionResult(async (a) => await CorrectiveActionRunService.Add(a));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// HandleEarlyYearEndLookbackChildJob
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lookbackWindowType"></param>
        /// <param name="processingDate"></param>
        public async Task HandleEarlyYearEndLookbackChildJob(Guid memberId,
                                                        WindowsIntervalType? lookbackWindowType,
                                                        DateTime processingDate)
        {
            try
            {
                MemberId = memberId;
                ProcessingDate = processingDate;
                EventDate = new DateTime(processingDate.Year - 1, 12, 31);
                ExecutingProcess = ExecutingProcessType.EarlyYearEndLookBack;

                Tuple<DateTime, DateTime> calculatedTwoYearLookBackWindow = null;
                Tuple<DateTime, DateTime> calculatedFiveYearLookBackWindow = null;

                // if no value then calc both
                if (!lookbackWindowType.HasValue)
                {
                    calculatedTwoYearLookBackWindow = CalculateLookBackWindow(WindowsIntervalType.TwoYearLookBack);
                    calculatedFiveYearLookBackWindow = CalculateLookBackWindow(WindowsIntervalType.FiveYearLookBack);
                }
                else if (lookbackWindowType == WindowsIntervalType.TwoYearLookBack)
                    calculatedTwoYearLookBackWindow = CalculateLookBackWindow(lookbackWindowType.Value);
                else if (lookbackWindowType == WindowsIntervalType.FiveYearLookBack)
                    calculatedFiveYearLookBackWindow = CalculateLookBackWindow(lookbackWindowType.Value);


                var calculatedMemberValues = LookBackDatesInfo.Create(
                                MemberId,
                                calculatedTwoYearLookBackWindow?.Item1,
                                calculatedTwoYearLookBackWindow?.Item2,
                                calculatedFiveYearLookBackWindow?.Item1,
                                calculatedFiveYearLookBackWindow?.Item2,
                                 "EarlyYearEnd_LookbackWindows");

                await TryUpdateLookBackDatesInfo(calculatedMemberValues, "EarlyYearEnd_LookbackWindows");

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Update DB
        /// <summary>
        /// PBI: 94842: Back - end process: update credential on loading moc / kci exam results
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="expireActiveIssuance"></param>
        /// <param name="administrationDate"></param>
        private void UpdateCredentialOnExamResult(ref Credential credential,
                                                    bool expireActiveIssuance,
                                                    DateTime? administrationDate)
        {
            var command = new UpdateCredentialOnExamResultCommand()
            {
                CredentialId = credential.ExternalId,
                ExamDueDate = credential.ExamDueDate,
                ForcedPathway = credential.ForcedPathway,
                Pathway = credential.Pathway,
                ExamFailCount = credential.ExamFailCount,
                AssessmentMet = credential.AssessmentMet,
                AssessmentMetDate = credential.AssessmentMetDate,
                GracePeriodEndDate = credential.GracePeriodEndDate,
                GracePeriodStartDate = credential.GracePeriodStartDate,
                ExpireActiveIssuances = expireActiveIssuance,
                ModifiedBy = credential.AuditData?.ModifiedBy ?? "ExamResults",
                AdministrationDate = administrationDate,
                ProcessingDate = ProcessingDate,
                DisplayExamDueDate = credential.DisplayExamDueDate,
                MOCExamDueDate = credential.MOCExamDueDate,
                KCIExamDueDate = credential.KCIExamDueDate,
                IsInCMP=credential.IsInCMP
            };

            var result = (UpdateCredentialOnExamResultCommandResult)(CredentialService.Handle(command));

            if (result.Succeeded)
                Logger.Debug($"Updated credential with Id:'{credential.ExternalId}' on loading moc/kci exam result command: '{command.Dump()}'");
            else if (result.Status == CommandStatus.Rejected)
                Log.Error($"UpdateCertStatusCredentials's call to Handle(UpdateCredentialOnExamResultCommand) failed with the following error:'{result.Message}'");
        }
        #endregion    

        private RegistrationData GetRegistrationData(
            Guid registrationId,
            ExamRegistrationType examRegistration,
            out RegistrationResource registration)
        {
            RegistrationData output = null;

            switch (examRegistration)
            {
                case ExamRegistrationType.Registration:
                    registration = GetRegistrationById(registrationId).Result;
                    if (registration != null)
                        output = new RegistrationData(registration);
                    break;

                case ExamRegistrationType.CMPRegistration:
                    var cmpReg = GetCMPRegistrationById(registrationId).Result;
                    registration = null;
                    if (cmpReg != null)
                        output = new RegistrationData(cmpReg);
                    break;

                default:
                    throw new ApplicationException($"Unknown ExamRegistrationType found in GetRegistrationData(): {examRegistration}.");
            }

            return output;
        }

        private bool ShouldWeExpireActiveIssuances(RegistrationData registration, Credential credential, DateTime processingDate)
        {
            //For Bug 160589
            if (credential.LookbackDate == null)
                return false;

            bool assessmentReqMet; //Needed as output param for MeetsExamRequirements(), but not used here

            bool examAdminPriorToLastLookback = ExamAdminWasPriorToMostRecentLookback(registration, credential);
            bool meetsExamRequirements = MeetsExamRequirements(credential, registration.MemberId, credential.LookbackDate.Value, out assessmentReqMet);
            bool meetsNonExamRequirements = NonExamRequirement(credential, credential.LookbackDate.Value, processingDate, false);
            bool returnValue = examAdminPriorToLastLookback && (!meetsExamRequirements || !meetsNonExamRequirements);

            Logger.Info($"ProgramRulesService.ShouldWeExpireActiveIssuances returning {returnValue} for member {registration.MemberId}, credential {credential.ExternalId}: examAdminPriorToLastLookback = {examAdminPriorToLastLookback}, meetsExamRequirements = {meetsExamRequirements}, meetsNonExamRequirements = {meetsNonExamRequirements}");

            return returnValue;
        }

        private bool ExamAdminWasPriorToMostRecentLookback(RegistrationData registration, Credential credential)
        {
            //For Bug 160589
            if (credential.LookbackDate == null)
                return false;
            else
            {
                //AdministrationYear should always have a non-zero value, but just to be safe, let's check
                int adminYear = (registration.AdministrationYear > 0 ? registration.AdministrationYear : registration.AdministrationDate.Year);

                return adminYear <= credential.LookbackDate.Value.Year;
            }
        }

        private string GetOnBehalfBoardName(string onBehalfBoardCode) 
        {
            if (String.IsNullOrWhiteSpace(onBehalfBoardCode))
                throw new ArgumentException("onBehalfBoardCode is required.", nameof(onBehalfBoardCode));

            var board = SourceService.Search().FirstOrDefault(x => x.Code == onBehalfBoardCode);
            if (board == null)
                throw new ApplicationException($"Board {onBehalfBoardCode} was not found in Sources.");

            return board.Name;
        }

        private bool ShouldWeIssueNewCredential(RegistrationResource registration)
        {
            //Variable for readability purposes
            bool examIsCosponsored = !String.IsNullOrWhiteSpace(registration?.OnBehalfOf);

            //PhysicianIsAbim will be true when the user has an ABIM ID and OnBehalfOf is null
            return (registration != null 
                && registration.Result == ExamResultType.Pass.ToString() 
                && registration.ExamType.ToEnum() == ExamType.Cert 
                && (registration.PhysicianIsAbim || examIsCosponsored)); 
        }
    }
}