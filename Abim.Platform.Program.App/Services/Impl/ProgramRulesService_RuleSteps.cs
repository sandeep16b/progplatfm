using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService
    {
        #region DetermineMaintenanceStatus
        /// <summary>
        /// 1) if we use as Step then it is PBI 135236 Met 2 year activity requirement (technical - common)
        /// 2) not as step then remains old rules
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="processingDate"></param>
        /// <param name="checkTwoYearReciprocityRequirement"></param>
        /// <param name="runAsStepRequirement"></param>
        /// <returns></returns>
        private IStep MaintenanceStatus(Credential credential,
                                         DateTime processingDate,
                                         Boolean checkTwoYearReciprocityRequirement = true,
                                         Boolean runAsStepRequirement = false)
        {
            var step = new MaintenanceStatusStep();

            step.MeetStepRule = step.MeetMaintenanceStatus = true;

            return step;

            /*
            // Pbi 296003 : (Proj 1530) Update Program Rule 28 – Participation Status Evaluation Due Date
            // Pbi 296004 : (Proj 1530) Retire Program Rule 31 – 2-Year Lookback Start and End Dates
            // Pbi 296005 : (Proj 1530) Retire Program Rule 32 – 2-Year Lookback
            // Pbi 296006 : (Proj 1530) Update Program Rule 37 – All Certificates Participation Requirements

            //to-do after 1/1/2025 remove MaintenanceStatus function and all references (12 total)

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [P031] 2 years lookback Requirements
            // Requirement: [P032] 
            //          any MOC points at last 2 year lookback 
            //      OR in reciprocity at last 2 year lookback or since then 
            //      OR Initially certified recently
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            try
            {
                // a) Any MOC points ( 2 year window )
                //----------------------------------------
                step.MaintenanceTwoYearLookBackDates = ProgramRulesHelpers.ComputeLookBackWindow(FirstIssuanceDate, processingDate, WindowsIntervalType.TwoYearLookBack); // [C009] [P031][C004]

                // when executing the YearEndLookBack then use as it was before this change processingDate as an endDate
                // check for this test Should_UseTheCurrentDate_InMeetFiveYearLookbackRequirement_For100MOCPoints_And_Not_MeetRequirement
                var maxUserActivityDate = ExecutingProcess == ExecutingProcessType.YearEndLookBack ? processingDate : UserActivities.Max(a => a.CompletedDate);

                step.MaintenanceAnyMOCPoints = maxUserActivityDate != null ? UserActivities.anyMOCpoints(step.MaintenanceTwoYearLookBackDates.Item1, maxUserActivityDate.Value) : 0; // [C008]

                if (step.MaintenanceAnyMOCPoints > 0)
                    step.MeetMaintenanceStatus = true;

                // b) Reciprocity (2 years lookback)
                //----------------------------------------
                if (!step.MeetMaintenanceStatus && checkTwoYearReciprocityRequirement)
                {
                    // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                    step.Reciprocity = step.MeetMaintenanceStatus = UserActivities.IsEnrolledInReciprocity_2yearLookBack(ExecutingProcess, processingDate, startOfTheWindowDate: step.MaintenanceTwoYearLookBackDates.Item1, endOfTheWindowDate: step.MaintenanceTwoYearLookBackDates.Item2); // pbi 274136 : Update Program Rule 32 - 2-year Lookback Requirement
                }

                // c) Recently initially certified (2 years window)
                //--------------------------------------------------
                if (!step.MeetMaintenanceStatus)
                {
                    //[C037] True If the year of earliest issuance + 2 years >= year of current date+1day
                    step.RecentlyInitiallyCertified = step.MeetMaintenanceStatus = FirstIssuanceDate.AddYears(2).Date >= processingDate.AddDays(1).Date; //(ar@2/15/2019) Bug 143394
                }

            }
            catch (UnsuccessfulStatusException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            step.MeetStepRule = runAsStepRequirement ? step.MeetMaintenanceStatus : true;

            return step;
            */
        }

        #endregion

        #region FiveYearsLookBack
        /// <summary>
        /// Marking public to test from unit tests
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="checkDate"></param>
        /// <param name="evaluationDate"></param>
        /// <param name="checkCheckDateRequirement"></param>
        /// <returns></returns>
        public IStep FiveYearsLookBack(Credential credential,
                                          DateTime checkDate,
                                          DateTime evaluationDate,
                                          bool checkCheckDateRequirement = true)
        {

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [P034] 5 years lookback Requirements | 
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /*   100 Total points and   20 Medical knowledge points
                    OR in reciprocity 
                    OR check_date < 12/31/2018 
                    OR recently initially certified 
                    OR earned new subspecialty initial certification during window                   
             */

            var step = new FiveYearLookBackStep();

            step.EvaluationDate = evaluationDate;

            if (checkCheckDateRequirement)
            {
                // a) Check Date < 12/31/2018
                step.MeetStepRule = DateTime.Compare(checkDate.Date, EndOf2018.Date) < 0;
            }

            if (!step.MeetStepRule)
            {
                step.FiveYearLookBackDates = ProgramRulesHelpers.ComputeLookBackWindow(FirstIssuanceDate,
                                                            checkDate,
                                                            WindowsIntervalType.FiveYearLookBack);

                // when executing the YearEndLookBack then use as it was before this change evaluationDate as an endDate for points to calculate
                // check for this test Should_UseTheCurrentDate_InMeetFiveYearLookbackRequirement_For100MOCPoints_And_Not_MeetRequirement
                var maxUserActivityDate = ExecutingProcess == ExecutingProcessType.YearEndLookBack ? evaluationDate : UserActivities.Max(a => a.CompletedDate);

                // ** [C005] 100 MOC points ( 5 year window)
                step.TotalMOCPoints = maxUserActivityDate != null ? UserActivities.totalMOCpoints(step.FiveYearLookBackDates.Item1, maxUserActivityDate.Value) : 0;

                //20 Medical knowledge points requirement is being removed as part of project 1380. 
                // *** [C006] 20 medical Knowledge Points (5 year window)
                //step.MKPoints = UserActivities.medicalKnowledgePoints(step.FiveYearLookBackDates.Item1, checkDate);
                //if (step.TotalMOCPoints >= 100 && step.MKPoints >= 20)
                //    step.MeetStepRule = true;
                if (step.TotalMOCPoints >= 100)
                    step.MeetStepRule = true;
            }


            // c) Reciprocity (5 year window) [P033][C003]
            if (!step.MeetStepRule)
            {
                // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReciprocity_5yearLookBack(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
            }

            // [C028] e) Eearned new Subspecialty initial Certification during window
            if (!step.MeetStepRule)
            {
                step.NewSubspecialtyInitialCert = step.MeetStepRule = Credentials.isEarnedNewSubspecialtyInitialCertOk(step.FiveYearLookBackDates.Item1, checkDate);
            }

            // [C038] d) Recently initially certified (5 years window) (ar@2/15/2019) Bug 143394
            if (!step.MeetStepRule)
            {
                step.RecentlyInitiallyCertified = step.MeetStepRule = FirstIssuanceDate.AddYears(5).Date >= checkDate.AddDays(1).Date;
            }

            return step;
        }


        /// <summary>
        /// This is only used for Five Year Lookback TLPC which is inside events looping
        /// Marking public for unit testing
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="checkDate"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        public IStep FiveYearsLookBackTL(Credential credential,
                                          DateTime checkDate,
                                          DateTime evaluationDate)
        {

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [P034] 5 years lookback Requirements | 
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /*   100 Total points and   20 Medical knowledge points
                    OR in reciprocity 
                    OR recently initially certified 
                    OR earned new subspecialty initial certification during window                   
            */
            var step = new FiveYearLookBackStep();
            step.EvaluationDate = evaluationDate;

            // b) [P033] [C002] (5-Year Lookback Start and End Dates)	
            step.FiveYearLookBackDates = ProgramRulesHelpers.ComputeLookBackWindow(FirstIssuanceDate,
                                                                            checkDate,
                                                                            WindowsIntervalType.FiveYearLookBack);

            // ** [C005] 100 MOC points ( 5 year window)
            step.TotalMOCPoints = UserActivities.totalMOCpoints(step.FiveYearLookBackDates.Item1, evaluationDate);

            // *** [C006] 20 medical Knowledge Points (5 year window) => removed in project 1380
            //step.MKPoints = UserActivities.medicalKnowledgePoints(step.FiveYearLookBackDates.Item1, evaluationDate);
            //if (step.TotalMOCPoints >= 100 && step.MKPoints >= 20)
            if (step.TotalMOCPoints >= 100)
                step.MeetStepRule = true;

            // c) Reciprocity (5 year window) [P033][C003]
            if (!step.MeetStepRule)
            {
                // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReciprocity_5yearLookBack(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
            }

            // [C028] e) Eearned new Subspecialty initial Certification during window
            if (!step.MeetStepRule)
            {
                // [C028] Determine if any issuance exists that is the first issuance for the credential and the source is ABIM and 
                // the certificate is not Internal Medicine and the issuance date >= starting date and the issuance date <= evaluation date
                step.NewSubspecialtyInitialCert = step.MeetStepRule = Credentials.isEarnedNewSubspecialtyInitialCertOk(step.FiveYearLookBackDates.Item1, evaluationDate);

            }

            // [C038] d) Recently initially certified (5 years window)
            if (!step.MeetStepRule)
            {
                step.RecentlyInitiallyCertified = step.MeetStepRule = FirstIssuanceDate.AddYears(5).Date >= evaluationDate.AddDays(1).Date; //(ar@2/15/2019) Bug 143394
            }

            return step;
        }

 

        #endregion

        #region ExamRequirement
        private IStep Exam(Credential credential,
                           DateTime evaluationDate)
        {

            var step = new ExamRequirementStep();
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Exam Requirements [C034] [C033] 
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Requirement: [C033] Met the non - exam requirements as of the check date and evaluation date
            step.ExamAssessmentMet = step.MeetStepRule = credential.AssessmentMet &&
                                                        credential.AssessmentMetDate.HasValue &&
                                                        credential.AssessmentMetDate.Value.Date <= evaluationDate.Date;

            return step;
        }

        private IStep ExamTL(Credential credential,
                                 DateTime evaluationDate,
                                 DateTime issuanceDateOfTLCP)
        {
            //PBI 137190, replaces code originally implemented as part of PBI 73194

            var step = new ExamRequirementStep();

            step.ExamAssessmentMet = step.MeetStepRule = credential.AssessmentMet &&
                                                         credential.AssessmentMetDate.HasValue &&
                                                         credential.AssessmentMetDate.Value.Date <= evaluationDate.Date;

            if (!step.MeetStepRule)
            {
                Log.Debug($"ExamTL() - assessment not met for credential {credential.Id}, requirement not met");
                return step;
            }

            //Determine if there was a fail in 0a no-consequences administration, if so verify there is at least 
            //one actual Pass in a administration that is later then the most recent fail no-consequences administration

            //Query registration to determine if there is an administration of type MOC for the exam where year of 
            //admin year+10 > year of (evaluation date+1 day) and result is pass. 
            step.PassMOCExam = step.MeetStepRule =
                Registrations.Any(x => x.CertificationId == credential.Certification.ExternalId
                    && x.IsMoc()
                    && (x.AdministrationYear + 10 > evaluationDate.AddDays(1).Year)
                    && x.IsPassExam());

            //If found, then this subrequirement is good (doesn’t matter if they took a noconcequences since there 
            //is a 10 year pass)
            if (step.MeetStepRule)
            {
                Log.Debug($"ExamTL() - found eligible passing MOC exam for credential {credential.Id}, requirement met");
                return step;
            }

            // to retire the program rule for no consequence assessments as ABIM is no longer offering no consequences assessments.
            // PBI 284269 : 2.53 Retire Program Rule 40 – No Consequence KCI Assessments

            // 180679 : (Proj 1492) - Update Program Rule 35 Time Limited Certificate Certification Requirements to Include LNG
            if (!step.MeetStepRule)
            {
                step.MeetStepRule = LongitudinalEnrollments.Any(e =>e.EnrollmentStatus.ToEnum() == EnrollmentStatusType.Active &&
                                                                    e.Assessment.CertificationId == credential.Certification.ExternalId &&
                                                                    e.LongitudinalParticipations.Any(x => x.Status.ToEnum() == ParticipationStatusType.Met));
            }

            Log.Debug($"ExamTL() - returning for credential {credential.Id}, step.MeetStepRule = {step.MeetStepRule}");
            return step;
        }
        /// <summary>
        /// ExamRequirements per 135089
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="evaluationDate"></param>
        /// <param name="issuanceDateOfTLCP"></param>
        /// <returns></returns>
        private IStep ExamRequirements(Credential credential,
                          DateTime evaluationDate,
                          DateTime issuanceDateOfTLCP)
        {
            //PBI 135089
            /*
             * The exam requirements are met if any of the following are true:
             * - Credential.AssessmentMet = true and Credential.AssessmentMetDate <= lookback date
             * - Credential.GracePeriodEndDate is not null and is >= current lookback date and Credential.GracePeriodStartDate <= lookback date and not null
             * - Has pending exam results
            */

            var step = new ExamRequirementStep();

            step.ExamAssessmentMet = step.MeetStepRule = credential.AssessmentMet &&
                                                        credential.AssessmentMetDate.HasValue &&
                                                        credential.AssessmentMetDate.Value.Date <= evaluationDate.Date;

            if (step.MeetStepRule)
            {
                Log.Debug($"ExamRequirements() - assessment met for credential {credential.Id}, requirement met");
                return step;
            }

            // if in a grace period
            step.InGracePeriod = step.MeetStepRule =
                    (credential.GracePeriodStartDate.HasValue && credential.GracePeriodEndDate.HasValue
                    && credential.GracePeriodStartDate.Value.AddDays(-1).Date <= evaluationDate.Date
                    && credential.GracePeriodEndDate.Value.Date >= evaluationDate.Date);

            if (step.MeetStepRule)
            {
                Log.Debug($"ExamRequirements() - credential {credential.Id} in a grace period, requirement met");
                return step;
            }
            // just to check .....
            if (credential.LookbackDate.HasValue)
            {
                // check if exam exists in MOC/KCI and status is Pending/Hold/PendingAdditionalTake
                step.PendingExamResultsExist = step.MeetStepRule = DoPendingExamResultsExistForCertification(credential.Certification.ExternalId, credential.LookbackDate.Value);

                if (step.MeetStepRule)
                {
                    Log.Debug($"ExamRequirements() - Pending ExamResults Exist for credential {credential.Id}, requirement met");
                    return step;
                }
            }

            Log.Debug($"ExamRequirements() - returning for credential {credential.Id}, step.MeetStepRule = {step.MeetStepRule}");
            return step;
        }
       
  

        #endregion

        #region Attestation

        //---------------- DELETE CODE LATER (I left it for reference, but would be remove later…. ) 

        /*
        private IStep Attestation(Credential credential,
                          DateTime checkDate,
                          DateTime evaluationDate)
        {
            var step = new AttestationStep();
            step.MeetStepRule = true;

            if (credential.Certification.IsICARD())
            {
                DateTime ReattestationDueDate = new DateTime();
                step.Attestation = true;

                DateTime? PreviousAttestationDate = UserActivities.FindMostRecentCompletedDateForProductCode(ProductResourceConstants.ProductCode.ICARDAttestMOC);

                if (credential.IsTimelimited)
                {
                    bool CertExpires2018orLater = credential.ExpirationDate.Value.Year <= 2018;
                    //*** Programs 006.1 : initial && 2018 or before ~~~ ReattestationDueDate == ExpirationDate
                    if (!PreviousAttestationDate.HasValue && CertExpires2018orLater)
                        ReattestationDueDate = credential.ExpirationDate.Value;
                    //*** Programs 006.2 : PreviousAttestationDate && 2019 or later ~~~ ReattestationDueDate == 12/31/2018
                    else if (!PreviousAttestationDate.HasValue && !CertExpires2018orLater)
                        ReattestationDueDate = new DateTime(2018, 12, 31);
                    //*** Programs 006.1a : PreviousAttestationDate && 2018 or before ~~~ ReattestationDueDate == PreviousAttestationDate + 5
                    //*** Programs 006.3  : PreviousAttestationDate && 2019 or later ~~~ ReattestationDueDate == PreviousAttestationDate + 5
                    else if (PreviousAttestationDate.HasValue)
                        ReattestationDueDate = new DateTime(PreviousAttestationDate.Value.Year + 5, 12, 31);

                }
                else if (credential.IsMBM)
                {
                    //*** Programs 006.4  : initial then ReattestationDueDate == initial certification year plus 5 
                    if (!PreviousAttestationDate.HasValue)
                        ReattestationDueDate = new DateTime(credential.OldestIssuance.IssuanceDate.Year + 5, 12, 31);
                    //*** Programs 006.5  : PreviousAttestationDate && 2019 or later ~~~ ReattestationDueDate == PreviousAttestationDate + 5
                    else
                        ReattestationDueDate = new DateTime(PreviousAttestationDate.Value.Year + 5, 12, 31);
                }
                else
                {
                    Log.Warn($"Improper use of 'Icard Reattestation Due Date'(Programs 006) rule for credentialId:'{credential.ExternalId}'");
                    return step;
                }

                step.ReattestationDueDate = ReattestationDueDate.Date;

                // if it is after ReattestationDueDate then it is over due
                if (evaluationDate.Date >= ReattestationDueDate.Date)
                    step.Attestation = step.MeetStepRule = false;
                
            }
            else if (credential.Certification.IsFPHM())
            {
                //[P006*][C010] This can be computed by taking the check date, add 1 to the date, extract the year, subtract 5 from the year to obtain a start year,
                DateTime attestationStartDate = ProgramRulesHelpers.ComputeReattestationDueDate(checkDate, WindowsIntervalType.FiveYearLookBack);

                //[P007][C011] verify there is an icard attestation “ICARDAttestMOC” activity completed between 1/1/start year and the evaluation date.
                step.Attestation = step.MeetStepRule = UserActivities.isAttestationOK(attestationStartDate, evaluationDate, ProductResourceConstants.ProductCode.FPHMAttestMOC);

                step.ReattestationDueDate = attestationStartDate.Date;

            }

            return step;
        }

        */

        /// <summary>
        /// PBI 135010, 135011. Method to determine if Attestation Requirement has been met during YearEnd Lookback and corrective actions
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="checkDate"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        public IStep Attestation(Credential credential,
            DateTime checkDate,
            DateTime evaluationDate)
        {
            var step = new AttestationStep();
            step.MeetStepRule = true;

            // pbi 257166 : Proj 1493 - Release 2.0 : Update Program Rule 12
            // – The FPHM attestation requirement is not past due (Program Rule: Focused Practice in Hospital Medicine Reattestation Due Date)
            // Release 2 : we would need to remove all reference to FPHM and corresponding attestations since we retire this cert 

            if (credential.Certification.IsICARD())
            {

                //[P006*][C010] This can be computed by taking the check date, add 1 to the date, extract the year, subtract 5 from the year to obtain a start year,
                DateTime attestationStartDate = ProgramRulesHelpers.ComputeReattestationDueDate(checkDate, WindowsIntervalType.FiveYearLookBack);

                //[P007][C011] verify there is an attestation [ICARDAttestMOC] activity completed between 1/1/start year and the evaluation date.
                // OR
                //[C052] the year of the earliest issuance date is > start year
                step.Attestation =
                    step.MeetStepRule = (UserActivities.isAttestationOK(attestationStartDate, evaluationDate, ProductResourceConstants.ProductCode.ICARDAttestMOC)
                                            || credential.OldestIssuance?.IssuanceDate.Date.Year >= attestationStartDate.Year);

                step.ReattestationDueDate = new DateTime(attestationStartDate.Year + 5, 12, 31);
            }

            return step;
        }

        #endregion

        #region FailedOrPendingExamInIssuanceExpiredYear
        //Requirement:  [C029] Failed, ind, inc, utt or pending result for the moc exam associated with the credential in the year the Issuance Expired. 
        private IStep FailedOrPendingExamInIssuanceExpiredYearNew(Credential credential)
        {
            var step = new FailedOrPendingExamInIssuanceExpiredYearStep();

            step.MeetStepRule = Registrations
                                .Where(e => credential.NewestIssuance.ExpirationDate.HasValue && e.ExamTestDate().Year == credential.NewestIssuance.ExpirationDate.Value.Year &&
                                        e.CertificationId == credential.Certification.ExternalId &&
                                        e.ExamType.Value != null && (e.ExamType.Value == ExamType.Moc.ToString()))
                                    .Where(r => r.Result == ExamResultType.Fail.ToString()
                                            || r.Result == ExamResultType.Indeterminate.ToString()
                                            || r.Result == ExamResultType.Incomplete.ToString()
                                            || r.Result == ExamResultType.UnableToTest.ToString()
                                            || r.Result == ExamResultType.Pending.ToString()).Any();

            return step;
        }
        #endregion

        #region DetermineMaintenanceStatusOnPassingInitialCert
        private MaintenanceStatusType DetermineMaintenanceStatusOnPassingInitialCert_(string CertificationCode,
                                                                         DateTime processingDate)
        {

            bool meetMaintainedRequirement = false;

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [P031] 2 years lookback Requirements
            // Requirement: [P032] 
            //          any MOC points at last 2 year lookback 
            //      OR in reciprocity at last 2 year lookback or since then 
            //      OR exam = "Internal Medicine"
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

            try
            {
                // a) exam = "Internal Medicine" or ACHD (Cert exam) or HPM (Cert Exam)
                //------------------------------------------------------------------------------------------
                // SR548818 : Doctors who passed HPM cert exam not publicly listed as certified
                if (CertificationCode == "IM" || CertificationCode == "ACHD" || CertificationCode == "HPM")
                    meetMaintainedRequirement = true;

                Tuple<DateTime, DateTime> twoYearLookBackDates = null;

                // b) Any MOC points ( 2 year window )
                //----------------------------------------
                if (!meetMaintainedRequirement)
                {
                    twoYearLookBackDates = ProgramRulesHelpers.ComputeLookBackWindow(FirstIssuanceDate, processingDate, WindowsIntervalType.TwoYearLookBack); // [C009] [P031][C004]

                    decimal anyMOCpoints = UserActivities.anyMOCpoints(twoYearLookBackDates.Item1, processingDate); // [C008]

                    if (anyMOCpoints > 0)
                        meetMaintainedRequirement = true;
                }

                // c) Reciprocity (2 years lookback)
                //----------------------------------------
                if (!meetMaintainedRequirement)
                {
                    // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                    bool twoYearReciprocityOK = UserActivities.IsEnrolledInReciprocity_2yearLookBack(ExecutingProcess, processingDate, startOfTheWindowDate: twoYearLookBackDates.Item1 , endOfTheWindowDate: twoYearLookBackDates.Item2 ); // pbi 274136 : Update Program Rule 32 - 2-year Lookback Requirement

                    if (twoYearReciprocityOK)
                        meetMaintainedRequirement = true;
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            return meetMaintainedRequirement ? MaintenanceStatusType.Maintained : MaintenanceStatusType.NotMaintained;
        }
        #endregion

        #region UpdateRuleResults 
        /// <summary>
        /// This function should normally run last in the chain of rules and should update move collected data 
        /// from invidual steps to RuleResult that would be return back to the caller
        /// It has dependency on step name and date that should be retrieved. Might be better to update Data during individual step execution.
        /// </summary>
        /// <param name="currentRuleResult"></param>
        /// <param name="ruleResults"></param>
        /// <returns></returns>
        public IList<RuleResults> UpdateRuleResults(RuleResults currentRuleResult, ref IList<RuleResults> ruleResults)
        {
            currentRuleResult.MapStepsToCorrectiveActionResults(isLastIteration: true); // since it is not looping through events

            ruleResults.Add(currentRuleResult);

            return ruleResults;
        }

        /// <summary>
        /// This function should normally run last in the chain of rules and should update move collected data 
        /// from invidual steps to RuleResult that would be return back to the caller
        /// It has dependency on step name and date that should be retrieved. Might be better to update Data during individual step execution.
        /// </summary>
        /// <param name="currentRuleResult"></param>
        /// <param name="eventDate"></param>
        /// <returns></returns>
        public RuleResults UpdateRuleResults(RuleResults currentRuleResult, DateTime eventDate)
        {
            currentRuleResult.MapStepsToCorrectiveActionResults();
            return currentRuleResult;
        }
        #endregion
    }
}
