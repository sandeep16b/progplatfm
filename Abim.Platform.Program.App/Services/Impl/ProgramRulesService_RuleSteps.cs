using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Exceptions;
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
                    step.Reciprocity = step.MeetMaintenanceStatus = UserActivities.IsEnrolledInReprocity(ExecutingProcess, processingDate, step.MaintenanceTwoYearLookBackDates.Item2);
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
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReprocity(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
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
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReprocity(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
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
        /// <summary>
        /// Marking public for unit tests
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        public IStep FiveYearsLookBackFPHM(Credential credential,
                       DateTime evaluationDate)
        {
            DateTime checkDate = evaluationDate;

            var step = new FiveYearLookBackStep();
            step.EvaluationDate = evaluationDate;

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Requirement: 100 Total points within 5 year [OR earned new subspecialty initial certification during window]
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 100 total moc points 
            //  OR in reciprocity 
            //  or more earned within 5 years of the check date 
            //  OR earned new subspecialty initial certification during window 

            /* [C005] Sum the total points from the activities that were completed between the starting date and evaluation date where the [C015] starting date is 
              computed by getting the check date, add 1 to the date, extract the year, subtract 6 from the year to obtain a start year, and use 1/1/starting year
            */

            DateTime startDate = new DateTime(checkDate.AddDays(1).Year - 5, 1, 1); // changed 6 to 5
            step.FiveYearLookBackDates = new Tuple<DateTime, DateTime>(startDate, evaluationDate);

            step.TotalMOCPoints = UserActivities.totalMOCpoints(startDate, evaluationDate);

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // Requirement: 20 Medical knowledge within 5 year [OR earned new subspecialty initial certification during window]
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 20 part 2 points or more earned in within 5 years of the check date
            /* [C006] Sum part 2 points with claimed=1, including those that are part of a blended activity that includes part 2 with a activity.completeddate between the 
             * [C015] starting date and evaluation date where the starting date is computed by getting the check date, add 1 to the date, extract the year, 
             * subtract 6 from the year to obtain a start year, and use 1/1/starting year
            */
            //MK Points removed in project 1380
            //step.MKPoints = UserActivities.medicalKnowledgePoints(startDate, evaluationDate);
            //if (step.TotalMOCPoints >= 100 && step.MKPoints >= 20)
            if (step.TotalMOCPoints >= 100)
                step.MeetStepRule = true;

            // Reciprocity (5 year window) [P033][C003]
            if (!step.MeetStepRule)
            {
                // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReprocity(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
            }

            if (!step.MeetStepRule)
            {
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // OR earned new subspecialty initial certification during window
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                // Determine if a new ABIM subspecialty initial credential was earned in the applicable 5 year window

                // [C028] Determine if any issuance exists that is the first issuance for the credential and the source is ABIM 
                // and the certificate is not Internal Medicine and the issuance date >= starting date and the issuance date <= evaluation date
                step.NewSubspecialtyInitialCert = step.MeetStepRule = Credentials.isEarnedNewSubspecialtyInitialCertOk(startDate, checkDate);

            }

            return step;
        }
        /// <summary>
        /// Marking public for unit tests
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        public IStep FiveYearsLookBackGFPrinting(Credential credential,
                      DateTime evaluationDate)
        {
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // [P034] 5 years lookback Requirements | 
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            /*   100 Total points and 20 Medical knowledge points
                    OR in reciprocity 
                    OR earned new subspecialty initial certification during window                   
            */
            // Check date – the same as the evaluation date
            DateTime checkDate = evaluationDate;

            var step = new FiveYearLookBackStep();

            // b) [P033] [C002] (5-Year Lookback Start and End Dates)	
            step.FiveYearLookBackDates = ProgramRulesHelpers.ComputeLookBackWindow(FirstIssuanceDate,
                                                                                                    checkDate,
                                                                                                    WindowsIntervalType.FiveYearLookBack);
            // ** [C005] 100 MOC points ( 5 year window)
            step.TotalMOCPoints = UserActivities.totalMOCpoints(step.FiveYearLookBackDates.Item1, evaluationDate);

            // *** [C006] 20 medical Knowledge Points (5 year window)  ==> removed in project 1380
            //step.MKPoints = UserActivities.medicalKnowledgePoints(step.FiveYearLookBackDates.Item1, evaluationDate);
            //if (step.TotalMOCPoints >= 100 && step.MKPoints >= 20)
            if (step.TotalMOCPoints >= 100)
                step.MeetStepRule = true;

            // -- Reciprocity (5 year window) [P033][C003]
            if (!step.MeetStepRule)
            {
                // Pbi 134025 reciprocity requirement (ar@2/16/2019)
                step.Reciprocity = step.MeetStepRule = UserActivities.IsEnrolledInReprocity(ExecutingProcess, evaluationDate, step.FiveYearLookBackDates.Item2);
            }

            // [C028] Eearned new Subspecialty initial Certification during window
            if (!step.MeetStepRule)
            {
                // [C028] Determine if any issuance exists that is the first issuance for the credential and the source is ABIM and 
                // the certificate is not Internal Medicine and the issuance date >= starting date and the issuance date <= evaluation date
                step.NewSubspecialtyInitialCert = step.MeetStepRule = Credentials.isEarnedNewSubspecialtyInitialCertOk(step.FiveYearLookBackDates.Item1, evaluationDate);
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

            //If not found
            if (!step.MeetStepRule)
            {
                //Query registration to determine if there is an administration of type KCI for the exam result 
                //is fail/ind/inc/utt and no consequences=1. 
                var badKCINoConsequenceExams = Registrations.Where(x => x.CertificationId == credential.Certification.ExternalId
                                                                                              && x.IsKci()
                                                                                              && x.IsFailIndIncUtt()
                                                                                              && x.NoConsequence).ToList();

                //PBI 148689: If not found, then check for CMP no consequences.  One more step now until they are good.
                if (badKCINoConsequenceExams == null || !badKCINoConsequenceExams.Any())
                {
                    //PBI 148689: Query CMPRegistration to determine if there is an administration of type CMP for the exam result 
                    //is fail/ind/inc/utt and no consequences=1. 
                    var badCMPNoConsequenceExams =
                    CMPRegistrations.Where(x => x.CMPExam != null && x.CMPExam.CertificationId == credential.Certification.ExternalId
                        && (x.ExamResult.ToEnum() == ExamResultType.Fail || x.ExamResult.ToEnum() == ExamResultType.UnableToTest
                                || x.ExamResult.ToEnum() == ExamResultType.Indeterminate || x.ExamResult.ToEnum() == ExamResultType.Incomplete)
                        && x.CMPExam.NoConsequenceYears.Any(y => y == x.TestDate.Year)).ToList();

                    //PBI 148689: If not found, then this subrequirement is good (never took noconcequences). Note the administration date(B)
                    if (badCMPNoConsequenceExams == null || !badCMPNoConsequenceExams.Any())
                    {
                        step.MeetStepRule = true;
                        Log.Debug($"ExamTL() - no KCI or CMP no-consequences exams with bad result found for credential {credential.Id}, requirement met");
                    }
                    else
                    {
                        //PBI 148689: If found,
                        //Query CMPRegistration to determine if there is an administration of any type for the exam result 
                        //is pass and the administration is after the date of the previous query (B)
                        var latestBadCMPNoConsequencesTestDate = badCMPNoConsequenceExams
                                                                                         .OrderByDescending(x => x.TestDate)
                                                                                         .First()
                                                                                         .TestDate;

                        var passingCMPWithGoodTestDate = CMPRegistrations.Where(x => x.CMPExam.CertificationId == credential.Certification.ExternalId
                                                                && x.ExamResult.ToEnum() == ExamResultType.Pass
                                                                && x.TestDate > latestBadCMPNoConsequencesTestDate);

                        //If found, then this subrequirement is good (passed an exam after noconsequences)
                        if (passingCMPWithGoodTestDate != null && passingCMPWithGoodTestDate.Any())
                        {
                            step.PassCMPExam = true;
                            step.MeetStepRule = true;
                            Log.Debug($"ExamTL() - passing CMP exam with good test date found for credential {credential.Id}, requirement met, PassKCIExam = {step.PassKCIExam}, PassMOCExam = {step.PassMOCExam}, PassCMPExam = {step.PassCMPExam}");
                        }

                        //If not found, then this subrequirement is false and the exam requirement is not met.             
                        //(step.MeetStepRule is already false)
                    }
                }
                else
                {
                    //If found,
                    //Query registration to determine if there is an administration of any type for the exam result 
                    //is pass and the ExamTestDate ( was administration date) is > the ExamTestDate ( was administration date) of the previous query (A)

                    var latestBadKCINoConsequencesExamTestDate = badKCINoConsequenceExams
                                                                                        .OrderByDescending(x => x.ExamTestDate())
                                                                                        .First()
                                                                                        .ExamTestDate();

                    var passingRegistrationsAfterBadKCI =
                                Registrations.Where(x => x.CertificationId == credential.Certification.ExternalId
                                    && x.IsPassExam()
                                    && x.ExamTestDate() > latestBadKCINoConsequencesExamTestDate).ToList();

                    //If kci/moc found, then this subrequirement is good (passed an exam after noconcequences)
                    if (passingRegistrationsAfterBadKCI != null && passingRegistrationsAfterBadKCI.Any())
                    {
                        var latest = passingRegistrationsAfterBadKCI.OrderByDescending(x => x.ExamTestDate()).First();

                        if (latest.IsKci())
                            step.PassKCIExam = true;
                        else
                            step.PassMOCExam = true;

                        Log.Debug($"ExamTL() - passing exam with good admin date found for credential {credential.Id}, requirement met, PassKCIExam = {step.PassKCIExam}, PassMOCExam = {step.PassMOCExam}, PassCMPExam = {step.PassCMPExam}");
                        step.MeetStepRule = true;
                    }
                    else // also need to check CMP to see if they have a pass 
                    {
                        // pbi 227738 : 2020/2021 CMP Pass and New Issuances Issues
                        var passingCMPWithGoodTestDate =
                                CMPRegistrations.Where(x => x.CMPExam.CertificationId == credential.Certification.ExternalId
                                    && x.ExamResult.ToEnum() == ExamResultType.Pass
                                    && x.TestDate > latestBadKCINoConsequencesExamTestDate);

                        //If cmp found, then this subrequirement is good (passed an exam after noconsequences)
                        if (passingCMPWithGoodTestDate != null && passingCMPWithGoodTestDate.Any())
                        {
                            step.PassCMPExam = true;
                            step.MeetStepRule = true;
                            Log.Debug($"ExamTL() - passing CMP exam with good test date found for credential {credential.Id}, requirement met, PassKCIExam = {step.PassKCIExam}, PassMOCExam = {step.PassMOCExam}, PassCMPExam = {step.PassCMPExam}");
                        }
                    }

                    //If not found, then this subreuqirement is false and the exam requirement is not met.             
                    //(step.MeetStepRule is already false)
                }
            }

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

        private IStep ExamFPHM(Credential credential,
                                  DateTime evaluationDate)
        {

            var step = new ExamRequirementStep();

            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // The exam assessment was met
            //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            step.ExamAssessmentMet = step.MeetStepRule = credential.AssessmentMet &&
                                                        credential.AssessmentMetDate.HasValue &&
                                                        credential.AssessmentMetDate.Value.Date <= evaluationDate.Date;

            // don't go any further when ExamAssessmentMet is not met
            if (!step.MeetStepRule)
                return step;

            //      Pathway is 10 year
            //      The MOC exam in the discipline was a pass
            //      The administration date is within 10 years of the evaluation date
            // Bug 119645: (remove one extra year)
            DateTime start10YearLookback = new DateTime(evaluationDate.AddDays(1).Year - 10, 1, 1); // changed 11 to 10

            step.PassMOCExam = step.MeetStepRule = Registrations.IfPassExamInRange(start10YearLookback, evaluationDate, ExamType.Moc, FPHMCertificateGuid);

            step.MOCExamTimeRange = new Tuple<DateTime, DateTime>(start10YearLookback, evaluationDate);

            //doesn’t matter if they took a noconcequences since there is a 10 year pass)
            if (!step.MeetStepRule)
            {

                // to find noconcequences
                DateTime start2YearLookback = new DateTime(evaluationDate.AddDays(1).Year - 2, 1, 1); // changed 3 to 2

                step.KCIExamTimeRange = new Tuple<DateTime, DateTime>(start2YearLookback, evaluationDate);

                var NoConsequenceKCIExam = Registrations
                                .Where(re => re.NoConsequence)
                                // !!! no filtering for date range (per Don)
                                .Where(p => p.IsFailIndIncUtt())
                                .Where(e => e.CertificationId == FPHMCertificateGuid)
                                .Where(reg => reg.IsKci())
                                .FirstOrDefault();

                DateTime? NoConcequenceKCIDateFound = NoConsequenceKCIExam == null ? (DateTime?)null : NoConsequenceKCIExam.AdministrationDate;

                step.NoConsequenceKCIExam = step.MeetStepRule = !NoConcequenceKCIDateFound.HasValue;

                if (NoConcequenceKCIDateFound.HasValue)
                {
                    //make sure user passed an exam after noconcequences
                    var examPassAfterNoconcequences = Registrations
                              .Where(reg => reg.ExamTestDate() > NoConcequenceKCIDateFound.Value.Date)
                              // !!! no filtering for evalution date (upper limit of date range) (per Don)
                              .Where(p => p.IsPassExam())
                              .Where(e => e.CertificationId == FPHMCertificateGuid)
                              .FirstOrDefault();

                    step.ExamPassAfterNoconcequences = step.MeetStepRule = examPassAfterNoconcequences != null;
                }
            }
            return step;
        }

        private IStep ExamGFPrinting(Credential credential,
                                         DateTime evaluationDate)
        {
            var step = new ExamRequirementStep();

            //-------------------------------------------------------------------------------------------------------------------------------------------
            //[P029*][C012] Pathway is 10 year
            //-------------------------------------------------------------------------------------------------------------------------------------------
            DateTime start10YearLookback = new DateTime(evaluationDate.AddDays(1).Year - 10, 1, 1); // changed 11 to 10

            step.MOCExamTimeRange = new Tuple<DateTime, DateTime>(start10YearLookback, evaluationDate);

            step.PassMOCExam = step.MeetStepRule = Registrations.IfPassExamInRange(start10YearLookback, evaluationDate, ExamType.Moc, credential.Certification.ExternalId);

            // ______ OR ________
            if (!step.MeetStepRule)
            {
                //-------------------------------------------------------------------------------------------------------------------------------------------
                //[P030*][C013] Pathway is 2 year
                //-------------------------------------------------------------------------------------------------------------------------------------------
                DateTime start2YearLookback = new DateTime(evaluationDate.AddDays(1).Year - 2, 1, 1);

                step.KCIExamTimeRange = new Tuple<DateTime, DateTime>(start2YearLookback, evaluationDate);

                step.PassKCIExam = step.MeetStepRule = Registrations.IfPassExamInRange(start2YearLookback, evaluationDate, ExamType.Kci, credential.Certification.ExternalId);
            }

            if (!step.MeetStepRule)
            {
                //PBI 150925 - CMP
                DateTime startDate = new DateTime((evaluationDate.AddDays(1).Year - 1), 1, 1);
                step.CMPExamTimeRange = new Tuple<DateTime, DateTime>(startDate, evaluationDate);
                step.PassCMPExam = step.MeetStepRule = CMPRegistrations.IfPassExamInRange(startDate, evaluationDate, credential.Certification.ExternalId);
            }

            // PBI 180686 : Update Program Rule 45 Print Pre-1990 Certs to Include LNG
            // === met the participation requirement on the longitudinal assessment in their first year on the longitudinal assessment 
            if (!step.MeetStepRule)
            {
                step.MetParticipationInTheFirstYear = step.MeetStepRule = LongitudinalEnrollments.IfMetParticipationInTheFirstYear(credential.Certification.ExternalId);
            }

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

            if (credential.Certification.IsICARD() || credential.Certification.IsFPHM())
            {
                var targetAttestationType = credential.Certification.IsICARD()
                    ? ProductResourceConstants.ProductCode.ICARDAttestMOC
                    : ProductResourceConstants.ProductCode.FPHMAttestMOC;

                //[P006*][C010] This can be computed by taking the check date, add 1 to the date, extract the year, subtract 5 from the year to obtain a start year,
                DateTime attestationStartDate = ProgramRulesHelpers.ComputeReattestationDueDate(checkDate, WindowsIntervalType.FiveYearLookBack);

                //[P007][C011] verify there is an attestation [ICARDAttestMOC/FPHMAttestMOC] activity completed between 1/1/start year and the evaluation date.
                // OR
                //[C052] the year of the earliest issuance date is > start year
                step.Attestation =
                    step.MeetStepRule = (UserActivities.isAttestationOK(attestationStartDate, evaluationDate, targetAttestationType)
                                            || credential.OldestIssuance?.IssuanceDate.Date.Year >= attestationStartDate.Year);

                step.ReattestationDueDate = new DateTime(attestationStartDate.Year + 5, 12, 31);
            }

            return step;
        }


        /// <summary>
        /// FPHM Initial attestation [C011] [C016] 
        /// The attestation must be completed within the last 3 years relative to the check date (Check date – the same as the evaluation date)
        /// [C011] Determine if the FPHMAttestInitial activity was completed between the starting date and evaluation date where the [C016] starting date is computed by getting the check date, add 1 to the date, extract the year, subtract 4 from the year to obtain a start year, and use 1/1/starting year
        /// Notes about attestations:
        ///        A new interval begins when an attestation occurs and ends 12/31/3 years later.
        ///        The rules for determining if a FPHM attestation is acceptable should be baked into the attestation process. If the activity is marked as completed, we presume it was acceptable.
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        private IStep AttestationFPHMInitial(Credential credential,
                                   DateTime evaluationDate)
        {

            var step = new AttestationStep();

            //var step = new StepResult("FPHM Initial attestation [C011] [C016]");
            // Bug 119645: (remove one extra year)
            DateTime attestationStartDate = new DateTime(evaluationDate.AddDays(1).Year - 3, 1, 1); // changed 4 to 3

            step.Attestation = step.MeetStepRule = UserActivities.isAttestationOK(attestationStartDate, evaluationDate, ProductResourceConstants.ProductCode.FPHMAttestInitial);

            step.ReattestationDueDate = attestationStartDate;

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

                // a) exam = "Internal Medicine" or ACHD (Cert exam)
                //----------------------------------------
                if (CertificationCode == "IM" || CertificationCode == "ACHD")
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
                    bool twoYearReciprocityOK = UserActivities.IsEnrolledInReprocity(ExecutingProcess, processingDate, twoYearLookBackDates.Item2);

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
