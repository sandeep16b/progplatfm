using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions;
using Abim.Platform.Program.App.Services.RuleValidators;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService
    {
        /// <summary>
        /// PBI 73194 [P035] Backend Process: Issue New Credentials for Time Limited Physicians Credentials
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        private IList<RuleResults> IssueNewCredentialForTLPC_(IEnumerable<Credential> credentials,
                                                                   Guid memberId,
                                                                   DateTime eventDate,
                                                                   DateTime processingDate)
        {
            #region declarations
            DateTime? BatchProcessingEvent;
            RuleResults rulesResult;
            IList<RuleResults> rulesResults = new List<RuleResults>();

            Log.Info($"IssueNewCredentialForTLPC_ CredenitalIds:{string.Join(", ", credentials.Select(r => r.Id).ToArray())} on Thread:{Thread.CurrentThread.ManagedThreadId}");
            #endregion

            var eventDateList = ComputeEventDateList(eventDate, includeBatchProcessingList:true).ToList();

            foreach (var credential in credentials.Where(cred => cred.CanHaveMBMForTLIssuance))
            {

                rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "73194", "MBMforTL");

                rulesResults.Add(rulesResult);

                DateTime checkDate = credential.ExpirationDate.HasValue && credential.ExpirationDate.Value.Date > processingDate ? credential.ExpirationDate.Value : processingDate;

                // credential has expiration date and it is end of the year (typical)
                if (credential.ExpirationDate.HasValue && credential.ExpirationDate.Value.Month == 12 && credential.ExpirationDate.Value.Day == 31)
                {
                    BatchProcessingEvent = new DateTime(credential.ExpirationDate.Value.Year, 9, 1);
                    if (eventDateList.Any(p => p.Date < BatchProcessingEvent))
                    {
                        // drop events that before batch processing date
                        var eventDateListReduced = eventDateList
                                        .Where(p => p.Date > BatchProcessingEvent)
                                        .ToList();
                        // add Batch processing event
                        eventDateListReduced.Add(BatchProcessingEvent.Value);
                        // sort and assign back to eventDateList
                        eventDateList = eventDateListReduced.OrderBy(o => o.Date).ToList();
                    }
                }

                foreach (var evaluationDate in eventDateList)
                {
                    // add validation to this rule:  Validate(() => (new CorrectiveActionxxxxxValidator()).Validate(credential), typeof(CorrectiveActionxxxxxValidator).ToString())
                    rulesResult.BeginStep(() => Attestation(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => FiveYearsLookBackTL(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => ExamTL(credential, evaluationDate, credential.NewestIssuance.IssuanceDate))
                                .OnMeetStep(() => MaintenanceStatus(credential, evaluationDate)) // [P031] 2 years lookback Requirements
                                .MapStepsToCorrectiveActionResults(isLastIteration: evaluationDate == eventDateList.Last());

                    if (rulesResult.MeetRuleRequirement)
                        break;
                }

                

            } //foreach (var credential in credentials)

            return rulesResults;
        }


        /// <summary>
        /// PBI 160256  Backend Process: Issue New MBM Credentials for Epired Lifetime certs
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        private IList<RuleResults> IssueMBMIssuanceForExpiredGF (IEnumerable<Credential> credentials,
                                                                   Guid memberId,
                                                                   DateTime eventDate,
                                                                   DateTime processingDate)
        {
            #region declarations
            RuleResults rulesResult;
            IList<RuleResults> rulesResults = new List<RuleResults>();
            Log.Info($"IssueMBMIssuanceForExpiredGF CredenitalIds:{string.Join(", ", credentials.Select(r => r.Id).ToArray())} on Thread:{Thread.CurrentThread.ManagedThreadId}");
            #endregion

            var eventDateList = ComputeEventDateList(eventDate, includeBatchProcessingList: false).ToList();
            DateTime checkDate = processingDate;

            foreach (var credential in credentials)
            {
                rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "73194(GF)-160256", "MBMforXGF");

                rulesResults.Add(rulesResult);

                foreach (var evaluationDate in eventDateList)
                {
                    rulesResult.BeginStep(() => Attestation(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => FiveYearsLookBackTL(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => ExamTL(credential, evaluationDate, credential.NewestIssuance.IssuanceDate))
                                .OnMeetStep(() => MaintenanceStatus(credential, evaluationDate)) // [P031] 2 years lookback Requirements
                                .MapStepsToCorrectiveActionResults(isLastIteration: evaluationDate == eventDateList.Last());

                    if (rulesResult.MeetRuleRequirement)
                        break;
                }
            } //foreach (var credential in credentials)

            return rulesResults;
        }
      
        private IList<RuleResults> CorrectiveActionParticipationStatus_(IEnumerable<Credential> credentials,
                                            Guid memberId,
                                            DateTime eventDate,
                                            DateTime evaluationDate,
                                            DateTime processingDate)
        {
            #region declarations
            DateTime checkDate = processingDate; 
            IList<RuleResults> rulesResults = new List<RuleResults>();
            Log.Info($"CorrectiveActionParticipationStatus_ CredenitalIds:{string.Join(", ", credentials.Select(r => r.Id).ToArray())}");
            #endregion

            foreach (var credential in credentials)
            {              
                // PBI: 86853 ***  Corrective action: Grandfather participation status ***
                if (credential.IsGrandfather)
                {
                    RuleResults rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "86853", "CorrectiveGF");

                    rulesResult.Validate(() => (new CorrectiveActionGrandfatherParticipationStatusValidator()).Validate(credential), typeof(CorrectiveActionGrandfatherParticipationStatusValidator).ToString())
                                .BeginStep(() => Exam(credential, checkDate)) // [C033] Met the non - exam requirements as of the check date and evaluation date
                                .OnMeetStep(() => FiveYearsLookBack(credential, checkDate, evaluationDate)) // [P034] 5 years lookback Requirements
                                .OnMeetStep(() => MaintenanceStatus(credential, processingDate, true, true)) // [P031] 2 years lookback Requirements
                                .Update(() => UpdateRuleResults(rulesResult, ref rulesResults));

                }
                // PBI: 86855 *** Corrective action: Time limited participation status ***
                else if (credential.IsTimelimited)
                {
                    RuleResults rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "86855", "CorrectiveTL");

                    rulesResult.Validate(() => (new CorrectiveActionTLParticipationStatusValidator()).Validate(credential), typeof(CorrectiveActionTLParticipationStatusValidator).ToString())
                                .BeginStep(() => Attestation(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => FiveYearsLookBack(credential, checkDate, evaluationDate))
                                .OnMeetStep(() => MaintenanceStatus(credential, processingDate, true, true)) // [P031] 2 years lookback Requirements
                                .Update(() => UpdateRuleResults(rulesResult, ref rulesResults));

                }
                // PBI: 86857 *** Corrective Action: MBM participation status ***
                else if (credential.IsMBM)
                {
                    RuleResults rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "86857", "CorrectiveMBM");

                    rulesResult.Validate(() => (new CorrectiveActionMBMParticipationStatusValidator()).Validate(credential), typeof(CorrectiveActionMBMParticipationStatusValidator).ToString())
                                .BeginStep(() => MaintenanceStatus(credential, processingDate, true, true)) // [P031] 2 years lookback Requirements Requirement: [P032] any MOC points at last 2 year lookback OR in reciprocity at last 2 year lookback or since then OR Initially certified recently
                                .Update(() => UpdateRuleResults(rulesResult, ref rulesResults));
                }

            } // end foreach

            return rulesResults;
        }

        private IList<RuleResults> CorrectiveActionCertStatus_(IEnumerable<Credential> credentials,
                                            Guid memberId,
                                            DateTime eventDate,
                                            DateTime evaluationDate)
        {

            #region declarations
            IList<RuleResults> rulesResults = new List<RuleResults>();
            DateTime checkDate = evaluationDate;
            Log.Info($"CorrectiveActionCertStatus_ CredenitalIds:{string.Join(", ", credentials.Select(r => r.Id).ToArray())}");
            #endregion

            foreach (var credential in credentials)
            {

                RuleResults rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "86846", "CorrectiveMBMCert");

                // PBI: 86846 ***  Corrective action: Evaluate Certification Status for a MBM cert ***
                rulesResult.Validate(() => (new CorrectiveActionMBMCertStatusValidator()).Validate(credential), typeof(CorrectiveActionMBMCertStatusValidator).ToString())
                    .BeginStep(() => Exam(credential, checkDate)) // [C033] Met the non - exam requirements as of the check date and evaluation date
                    .OnMeetStep(() => Attestation(credential, checkDate, evaluationDate))
                    .OnMeetStep(() => FiveYearsLookBack(credential, checkDate, evaluationDate)) // [P034] 5 years lookback Requirements
                    .OnMeetStep(() => MaintenanceStatus(credential, evaluationDate)) // [P031] 2 years lookback Requirements
                    .Update(() => UpdateRuleResults(rulesResult, ref rulesResults));
            }

            return rulesResults;
        }


        /// <summary>
        /// TL ******
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="memberId"></param>
        /// <param name="eventDate"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        private IList<RuleResults> CorrectiveActionReinstateTLCert(IEnumerable<Credential> credentials,
                                    Guid memberId,
                                    DateTime eventDate,
                                    DateTime evaluationDate)
        {

            #region declarations
            IList<RuleResults> rulesResults = new List<RuleResults>();
            DateTime checkDate = evaluationDate;
            #endregion

            foreach (var credential in credentials)
            {
                RuleResults rulesResult = new RuleResults(credential.ExternalId, MemberId, eventDate, credential.CredentialCategory, "142201", "ReinstateTL");

                // Meet the non-exam requirements (pbi 135009)
                // Meet the Exam requirements (pbi 135089)

                // PBI: 86846 ***  Corrective action: Evaluate Certification Status for a MBM cert ***
                rulesResult.BeginStep(() => FiveYearsLookBack(credential, checkDate, evaluationDate,false)) // [C033] Met the non - exam requirements as of the check date and evaluation date
                            .OnMeetStep(() => Attestation(credential, checkDate, evaluationDate))
                            .OnMeetStep(() => ExamRequirements(credential, checkDate, evaluationDate)) // [P034] 5 years lookback Requirements
                            .OnMeetStep(() => MaintenanceStatus(credential, evaluationDate)) // [P031] 2 years lookback Requirements
                            .Update(() => UpdateRuleResults(rulesResult, ref rulesResults));
            }

            return rulesResults;
        }

       

        #region  Two Five Year LookBack Evaluation

        /// <summary>
        /// LookBackDatesEvaluation
        /// Two/Five year lookbacks evaluations (PBI 133037 / 133038 )
        /// </summary>
        /// <returns></returns>
        internal async Task LookBackDatesEvaluation()
        {
            // find current LookBackDatesInfo (can be null if not found)
            var currentLookBackDatesInfo = await LookBackDatesInfoService.GetLookBackDatesInfo(MemberId);

            // none of the windows are expired
            if (ProcessingDate.Date < currentLookBackDatesInfo?.Lookback2YearEndDate?.Date && 
                ProcessingDate.Date < currentLookBackDatesInfo?.Lookback5YearEndDate?.Date ) 
            {
                Log.Debug($"There are NO expired look back windows (2 & 5 years) were found for MemberId:'{MemberId}'");
                return;
            }
                
            Tuple<DateTime, DateTime> calculatedTwoYearLookBackWindow = null;
            Tuple<DateTime, DateTime> calculatedFiveYearLookBackWindow = null;

            // 2 year look back: process if current value is empty or expired
            if (currentLookBackDatesInfo == null || currentLookBackDatesInfo.Lookback2YearEndDate == null || ProcessingDate.Date >= currentLookBackDatesInfo?.Lookback2YearEndDate?.Date)
                calculatedTwoYearLookBackWindow = CalculateLookBackWindow(WindowsIntervalType.TwoYearLookBack);

            // 5 year look back: process if current value is empty or expired
            if (currentLookBackDatesInfo == null || currentLookBackDatesInfo.Lookback5YearEndDate == null || ProcessingDate.Date >= currentLookBackDatesInfo?.Lookback5YearEndDate?.Date)
                calculatedFiveYearLookBackWindow = CalculateLookBackWindow(WindowsIntervalType.FiveYearLookBack);

            // cannot calculate windows then log warning
            if (calculatedTwoYearLookBackWindow == null && calculatedFiveYearLookBackWindow == null)
                Logger.Warn($"No Look Back Windows were calculate for MemberId:'{MemberId}'");


            var calculatedLookBackDatesInfo = LookBackDatesInfo.Create(MemberId, 
                                            calculatedTwoYearLookBackWindow?.Item1,
                                            calculatedTwoYearLookBackWindow?.Item2,
                                            calculatedFiveYearLookBackWindow?.Item1,
                                            calculatedFiveYearLookBackWindow?.Item2,
                                            "");

            await TryUpdateLookBackDatesInfo(calculatedLookBackDatesInfo, "CA_LookBackWindows", currentLookBackDatesInfo);
        }

        private Tuple<DateTime, DateTime> CalculateLookBackWindow(WindowsIntervalType lookbackWindowType)
        {
            DateTime evalDate = new DateTime(ProcessingDate.Year - 1, 12, 31);

            int cycle = (int) Math.Ceiling( ((double)evalDate.Year + 1 - Math.Max(FirstIssuanceDate.Year+ 1, 2014)) / (int)lookbackWindowType );

            if (cycle < 1)
                cycle = 1;
           
            bool meetReq;
            Tuple<DateTime, DateTime> backEndlookBackWindow;

            do
            {
                backEndlookBackWindow = ComputeLookBackWindowDuringLookBack(FirstIssuanceDate,
                                                                                evalDate,
                                                                                lookbackWindowType);

                meetReq = LookBackRequirements(backEndlookBackWindow,
                                    lookbackWindowType,
                                    ProcessingDate);

                if (meetReq)
                    break;

                //step back to previous window
                evalDate = evalDate.AddYears(-(int)lookbackWindowType);

                //moving backwards
                cycle--;
            }
            while (cycle >= 1);

            Logger.Debug($"LookBackDatesEvaluation# meetReq:{meetReq} lookbackWindowType:{lookbackWindowType.ToString()} LookBackStartDate:'{backEndlookBackWindow.Item1.ToShortDateString()}' LookBackEndDate:'{backEndlookBackWindow.Item2.ToShortDateString()}'");

            // if requirement was met and after expired look back windows than set eval date to the next period ....
            if (meetReq && ProcessingDate.Date >= backEndlookBackWindow.Item2.Date)
                return new Tuple<DateTime, DateTime>(backEndlookBackWindow.Item2.AddDays(1), backEndlookBackWindow.Item2.AddYears((int)lookbackWindowType));
            else
                return backEndlookBackWindow;
        }

        /// <summary>
        /// LookBackRequirements
        /// </summary>
        /// <param name="loopBackWindow"></param>
        /// <param name="lookbackWindowType"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        private bool LookBackRequirements(  Tuple<DateTime, DateTime> loopBackWindow,
                                            WindowsIntervalType lookbackWindowType,
                                            DateTime processingDate)
        {
            // -- FiveYearLookBack requirements
            if (lookbackWindowType == WindowsIntervalType.FiveYearLookBack)
            {
                // 100 Total points or more
                if (UserActivities.totalMOCpoints(loopBackWindow.Item1, processingDate) >= 100 ||
                    // OR in MOC Reciprocity attestation | Pbi 134025 reciprocity requirement (ar@2/16/2019)
                    UserActivities.IsEnrolledInReciprocity_5yearLookBack(ExecutingProcess, processingDate, loopBackWindow.Item2) || // pbi 274137 : (2.50) Update Program Rule 35 - 5-year Lookback Requirements
                // OR  earned an initial subspecialty Certificate
                    Credentials.isEarnedNewSubspecialtyInitialCertOk(loopBackWindow.Item1, processingDate))
                        return true;

            }
            // -- TwoYearLookBack
            else if (lookbackWindowType == WindowsIntervalType.TwoYearLookBack)
            {
                // any points OR in MOC Reciprocity attestation 
                if (UserActivities.totalMOCpoints(loopBackWindow.Item1, processingDate) > 0 ||
                    // OR in MOC Reciprocity attestation | Pbi 134025 reciprocity requirement (ar@2/16/2019)
                    UserActivities.IsEnrolledInReciprocity_2yearLookBack( ExecutingProcess, processingDate, startOfTheWindowDate: loopBackWindow.Item1, endOfTheWindowDate: loopBackWindow.Item2)) // pbi 274136 : Update Program Rule 32 - 2-year Lookback Requirement

                    return true;
            }
            return false;
        }

        /// <summary>
        /// GetFirstLookbackWindow
        /// </summary>
        /// <param name="earliestCertDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        private Tuple<DateTime, DateTime> GetFirstLookbackWindow(DateTime earliestCertDate, WindowsIntervalType windowsInterval)
        {
            bool before2014 = earliestCertDate.Date <= DateOf2014;

            DateTime startDate = before2014 ? DateOf2014 : earliestCertDate.Date;
            DateTime endDate = before2014 ? startDate.AddYears((int)windowsInterval).AddDays(-1) :
                                        new DateTime(startDate.Year + (int)windowsInterval, 12, 31);

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date)  based on Earliest Cert issuance by ABIM date and
        /// certificate check date
        /// </summary>
        /// <param name="earliestCertDate"></param>
        /// <param name="checkDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        internal Tuple<DateTime, DateTime> ComputeLookBackWindowDuringLookBack(DateTime earliestCertDate,
                                                                           DateTime checkDate,
                                                                           WindowsIntervalType windowsInterval)
        {
            int startDateYear;
            int endDateYear;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            // ----- 5 years window ----------
            if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 11 && earliestCertDate.Year >= 2014)
                {
                    startDate = earliestCertDate;
                    endDate = new DateTime(startDate.Year + 5, 12, 31);
                }
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ((trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 5)-1) *5)) ),'mm/dd/yyyy');
                    startDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + (((int)Math.Ceiling((double)(checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) - 1) * 5));

                    startDate = new DateTime(startDateYear, 1, 1);
                    endDate = new DateTime(startDate.Year + 4, 12, 31);
                }
            }
            // ----- 2 years window ----------
            else if (windowsInterval == WindowsIntervalType.TwoYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 5 && earliestCertDate.Year >= 2014)
                {
                    startDate = earliestCertDate;
                    endDate = new DateTime(startDate.Year + 2, 12, 31);
                }
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char( greatest(earlyyear+1, 2014) + ( (trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 2)-1) *2 ) ) ,'mm/dd/yyyy');
                    startDateYear = Math.Max(earliestCertDate.Year + 1, 2014) + (((int)Math.Ceiling((double)(checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) - 1) * 2);

                    startDate = new DateTime(startDateYear, 1, 1);
                    endDate = new DateTime(startDate.Year + 1, 12, 31);
                }
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }
        #endregion
    }
}
