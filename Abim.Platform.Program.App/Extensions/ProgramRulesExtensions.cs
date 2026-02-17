using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using static Abim.Platform.Program.Resources.ProgramResourceConstants;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// Extensions class specifically for use within the ProgramRulesService
    /// </summary>
    internal static class ProgramRulesExtensions
    {

        #region Certification

        /// <summary>
        /// Determines whether this instance is icard.
        /// </summary>
        /// <param name="certification">The certification.</param>
        /// <returns>
        ///   <c>true</c> if the specified certification is icard; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsICARD(this Certification certification)
        {
            return certification.Code == "ICARD";
        }

        /// <summary>
        /// Determines whether this instance is FPHM.
        /// </summary>
        /// <param name="certification">The certification.</param>
        /// <returns>
        ///   <c>true</c> if the specified certification is FPHM; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsFPHM(this Certification certification)
        {
            return certification.Code == CertificationCode.FocusedPracticeHospitalMedicine;
        }

        #endregion

        #region CredentialList

        internal static IEnumerable<Credential> NewestIssuanceABIM(this IEnumerable<Credential> credentials)
        {
            return credentials.Where(i => i.HasIssuances && i.NewestIssuance.Source.Code == "ABIM");
        }

        internal static IEnumerable<Credential> ExamDueDateIsPreviousYear(this IEnumerable<Credential> credentials, DateTime processingDate)
        {
            return credentials.Where(i => i.ExamDueDate.HasValue && i.ExamDueDate.Value.Year == processingDate.Year - 1);
        }

        internal static IEnumerable<Credential> IsExpired(this IEnumerable<Credential> credentials)
        {
            return credentials.Where(i => i.HasIssuances && i.NewestIssuance.IssuanceStatus == IssuanceStatusType.Expired);
        }

        internal static IEnumerable<Credential> RequiresGracePeriodCorrectiveActionCheck(this IEnumerable<Credential> credentials, IList<RegistrationResource> registrations, DateTime processingDate)
        {
            IList<Credential> results = new List<Credential>();

            foreach (var credential in credentials)
            {
                //--------- Time limited =========
                if (credential.IsTimelimited &&
                    credential.IssuanceExpirationIsPreviousYear(processingDate))
                {
                    results.Add(credential);
                }
                //--------- Must-Be-Maintained =========
                else if (credential.IsMBM &&
                         credential.IssuanceExpirationIsPreviousYear(processingDate) &&
                         credential.LastExamPass10yearsAgo(registrations))
                {
                    results.Add(credential);
                }
                //--------- Grand Father =========
                else if (credential.IsGrandfather &&
                        credential.IfGrandFatherNotMaintained &&
                        credential.LastExamPass10yearsAgo(registrations))
                {
                    results.Add(credential);
                }
            }

            return results;
        }

        internal static IEnumerable<Credential> AbimCredentials(this IEnumerable<Credential> credentials)
        {
          return  credentials.Where(x => x.Certification.Source.Code == "ABIM");
        }

        #endregion

        #region Credential

        internal static bool IsKci(this Credential credential)
        {
            return credential.Pathway == Resources.PathwayType.KCI;
        }

        internal static bool IsMoc(this Credential credential)
        {
            return credential.Pathway == Resources.PathwayType.MOC;
        }

        internal static bool IssuanceExpirationIsPreviousYear(this Credential credential, DateTime processingDate)
        {
            return (credential.HasIssuances &&
                    credential.Issuances.Where(p => p.ExpirationDate.HasValue && p.ExpirationDate.Value.Year == processingDate.Year - 1).Any());
        }

        //Requirement:  [C031] 
        //The year of the last pass of the 10 year exam (initial or moc) for this credential was 10 years prior to the expiration date.
        internal static bool LastExamPass10yearsAgo(this Credential credential, IList<RegistrationResource> registrations)
        {
            return registrations.Where(e => credential.ExpirationDate.HasValue 
                                        && e.ExamTestDate().AddYears(10).Year >= credential.ExpirationDate.Value.Year
                                        && (e.IsMoc() || e.IsInitial())
                                        && e.Result == ExamResultType.Pass.ToString())
                                .Any();
        }

        /// <summary>
        /// Requirement: Assessment is met 
        ///  [C034] This is determined by looking at the credential.asssmentmet=1 and credential.assessmentmetdate less or equal evaluation date
        ///  [C033] Met the non - exam requirements as of the check date and evaluation date
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        internal static bool IsMeetExamRequirements(this Credential credential, DateTime evaluationDate)
        {
            return credential.AssessmentMet = true &&
                   credential.AssessmentMetDate.HasValue &&
                   DateTime.Compare(credential.AssessmentMetDate.Value.Date, evaluationDate.Date) <= 0;
        }


        internal static bool isEarnedNewSubspecialtyInitialCertOk(this IEnumerable<Credential> credentialList, DateTime startFiveYearLookBackDate, DateTime evaluationDate)
        {
            // [C028] Determine if any issuance exists that is the first issuance for the credential and the source is ABIM and 
            // the certificate is not Internal Medicine and the issuance date >= starting date and the issuance date <= evaluation date
            return credentialList.Where(a => a.Certification.Code != "IM" && a.HasIssuances && !a.IsCosponsored) //pbi 282145: only should consider none-cosponsored certs
                                  .Where(t=> t.Certification.Code != "HOSP") // TEMP FIX ar@2/17/2019
                                        .Select(a => a.OldestIssuance)
                                        .Where(k => k.Source.Code == "ABIM"
                                             && k.IssuanceDate.Date >= startFiveYearLookBackDate.Date
                                             && evaluationDate.Date >= k.IssuanceDate.Date)
                                        .Any();
        }

        internal static IEnumerable<DateTime> EarnedNewSubspecialtyInitialCertEvents(this IEnumerable<Credential> credentialList, DateTime evaluationDate)
        {
            // [C028] Determine if any issuance exists that is the first issuance for the credential and the source is ABIM and 
            // the certificate is not Internal Medicine and the issuance date >= starting date and the issuance date <= evaluation date
            return credentialList.Where(a => a.Certification.Code != "IM" && a.HasIssuances)
                                        .Select(a => a.OldestIssuance)
                                        .Where(k => k.Source.Code == "ABIM"
                                             && k.IssuanceDate.Date > evaluationDate.Date)
                                        .Select(a=>a.IssuanceDate);
        }
        #endregion

        #region IEnumerable<ActivityResource>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public static DateTime? FindCompletedDateForProductCodeSinceDate(this IEnumerable<ActivityResource> userActivities, DateTime startDate, string productCode)
        {
            return userActivities.Where(p => p.Product.Code == productCode)
                  .Where(p => p.ActivityResult.Value == ActivityResultType.Pass.ToString())
                  .Where(d => d.CompletedDate.HasValue && d.CompletedDate.Value.Date >= startDate.Date)
                  .OrderByDescending(d=>d.CompletedDate)
                  .Select(a=>a.CompletedDate)
                  .FirstOrDefault();
        }

        /// <summary>
        /// IsEnrolledInReciprocity_5yearLookBack
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="executingProcess"></param>
        /// <param name="endOfTheWindowDate"></param>
        /// <param name="evaluationDate"></param>
        /// <returns></returns>
        public static bool IsEnrolledInReciprocity_5yearLookBack(this IEnumerable<ActivityResource> userActivities, ExecutingProcessType executingProcess, DateTime evaluationDate, DateTime endOfTheWindowDate)
        {
            var theDate = executingProcess == ExecutingProcessType.YearEndLookBack ||
                                executingProcess == ExecutingProcessType.EarlyYearEndLookBack ? endOfTheWindowDate.Date : evaluationDate.Date;

            return userActivities.Where(p => p.Product.Code == ProductResourceConstants.ProductCode.ReciprocityAttest) //  "ReciprocityAttest"
                                                                                                                       // if Pass (completed) then it should be only for 2 years 
                                .Where(p => (p.ActivityResult.Value == ActivityResultType.Pass.ToString()
                                                && p.CompletedDate.HasValue
                                                // Modified Program Rule 034: a) currently in the MOC reciprocity program OR b) in the MOC reciprocity program at the end of the 5-year lookback cycle.
                                                && ( p.CompletedDate.Value.Date.AddYears(2) > evaluationDate.Date || p.CompletedDate.Value.Date.AddYears(2) > endOfTheWindowDate.Date)) 
                                            // if Canceled then we need to check if the date is before cancalation date 
                                            || (p.ActivityResult.Value == ActivityResultType.Cancelled.ToString()
                                                && p.CancelledDate.HasValue && p.CancelledDate.Value.Date > theDate))
                                .Any();
        }

        /// <summary>
        /// IsEnrolledInReciprocity_2yearLookBack
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="executingProcess"></param>
        /// <param name="evaluationDate"></param>
        /// <param name="startOfTheWindowDate"></param>
        /// <param name="endOfTheWindowDate"></param>
        /// <returns></returns>
        public static bool IsEnrolledInReciprocity_2yearLookBack(this IEnumerable<ActivityResource> userActivities, ExecutingProcessType executingProcess, DateTime evaluationDate, DateTime startOfTheWindowDate, DateTime endOfTheWindowDate)
        {
            var theDate = executingProcess == ExecutingProcessType.YearEndLookBack ||
                                executingProcess == ExecutingProcessType.EarlyYearEndLookBack ? endOfTheWindowDate.Date : evaluationDate.Date;

            return userActivities.Where(p => p.Product.Code == ProductResourceConstants.ProductCode.ReciprocityAttest) //  "ReciprocityAttest"
                                .Where(p => (p.ActivityResult.Value == ActivityResultType.Pass.ToString()
                                                && p.CompletedDate.HasValue
                                                && p.CompletedDate.Value.Date.AddYears(2) > theDate || p.CompletedDate.Value.Date.AddYears(2) >= startOfTheWindowDate.Date) // pbi 274136 : Update Program Rule 32 - 2-year Lookback Requirement
                                                                                                                                                                            // if Canceled then we need to check if the date is before cancalation date 
                                            || (p.ActivityResult.Value == ActivityResultType.Cancelled.ToString()
                                                && p.CancelledDate.HasValue && p.CancelledDate.Value.Date > theDate))
                                .Any();
        }

        #endregion

        #region Temp (keep it 

        public static DateTime? GetRecentExamPassDate1(this IEnumerable<RegistrationResource> registrations,
                                      ExamType examType,
                                      Guid CertificationId)
        {
            if (registrations.Count() == 0) return null;

            DateTime? recentExamDate = null;

            var mostRecentRegistration = registrations.Where(p => p.Result == ExamResultType.Pass.ToString())
                                                  .Where(e => e.CertificationId == CertificationId)
                                                  .Where(reg => reg.ExamType != null && reg.ExamType.ToEnum() == examType)
                                                  .OrderByDescending(s => s.AdministrationDate)
                                                  .FirstOrDefault();

            if (mostRecentRegistration != null)
                recentExamDate = mostRecentRegistration.ExamTestDate();
            // there are no passes per spesified examType, try to find pass date for initial cert exam
            else if (examType== ExamType.Moc)
            {
                var recentCertRegistration = registrations.Where(e => e.CertificationId == CertificationId
                                                                  && e.ExamType != null
                                                                  && e.ExamType.ToEnum() == ExamType.Cert
                                                                  && e.Result == ExamResultType.Pass.ToString())
                                               .FirstOrDefault();

                if (recentCertRegistration != null)
                    recentExamDate = recentCertRegistration.ExamTestDate();
            }

            return recentExamDate;
        }

        #endregion
    }


}
