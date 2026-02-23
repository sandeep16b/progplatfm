using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.App.Extensions
{
    public static class ActivityExtensions
    {
        #region IEnumerable<ActivityResource>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public static bool isAttestationOK(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate, string productCode)
        {
            return userActivities.Where(p => p.Product.Code == productCode)
                  .Where(p => p.ActivityResult.Value == ActivityResultType.Pass.ToString())
                  .Where(d => d.CompletedDate.HasValue && d.CompletedDate.Value.Date >= startDate.Date &&
                                d.CompletedDate.Value.Date <= endDate.Date)
                  .Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static decimal totalMOCpoints(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            return userActivities.Where(d => d.CompletedDate.HasValue && d.CompletedDate.Value.Date >= startDate.Date
                                                    && d.CompletedDate.Value.Date <= endDate.Date)
                                    .Where(a => a.ActivityResult.Value == ActivityResultType.Pass.ToString())
                                    .Sum(p => p.TotalMOCPoints);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        public static decimal prior2YearsMOCpoints(this IEnumerable<ActivityResource> userActivities,
                                            DateTime processingDate)
        {
            DateTime startDate = new DateTime(processingDate.Year - 2, 1, 1);
            DateTime endDate = new DateTime(processingDate.Year - 1, 12, 31);
            return userActivities.totalMOCpoints(startDate, endDate);
        }

        /// <summary>
        /// MOC points by credit type
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, decimal> MOCpointsByCreditType(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            Dictionary<string, decimal> returnMOCPoints = new Dictionary<string, decimal>();

            var arrary = userActivities.SelectMany(a => a.ActivityCredits)
                                         .Where(a => a.CreditDate.HasValue && a.CreditDate.Value.Date >= startDate.Date && a.CreditDate.Value.Date <= endDate.Date)
                                         .Where(a => !a.ExpirationDate.HasValue || a.ExpirationDate.Value.Date >= endDate.Date)
                                         .Where(a => a.Claimed && !a.OnHold)
                                         .GroupBy(a => a.CreditType.Value)
                                         .Select(group => new { group.Key, TotalPoints = group.Sum(a => a.CreditEarned) });
            foreach (var item in arrary)
            {
                returnMOCPoints.Add(item.Key, item.TotalPoints);
            }
            return returnMOCPoints;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static decimal medicalKnowledgePoints(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            return userActivities.SelectMany(c => c.ActivityCredits)
                                               .Where(a => a.CreditType.Value == ProductResourceConstants.CreditTypeValue.MedicalKnowledgePoints //( "MK" ) Knowledge Points
                                                    || a.CreditType.Value == ProductResourceConstants.CreditTypeValue.BlendedMedicalKnowledgeActivity)
                                               .Where(a => a.CreditDate.HasValue
                                                     && a.CreditDate.Value.Date >= startDate.Date
                                                     && a.CreditDate.Value.Date <= endDate.Date)
                                               .Where(a => !a.ExpirationDate.HasValue || a.ExpirationDate.Value.Date >= endDate.Date)
                                               .Where(v => v.Claimed && !v.OnHold)
                                               .Sum(c => c.CreditEarned);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static decimal blendedMedicalKnowledgeActivityPoints(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            return userActivities.SelectMany(c => c.ActivityCredits)
                                               .Where(a => a.CreditType.Value == ProductResourceConstants.CreditTypeValue.BlendedMedicalKnowledgeActivity) // Blended MK PA
                                               .Where(a => a.CreditDate.HasValue && a.CreditDate.Value.Date >= startDate.Date && a.CreditDate.Value.Date <= endDate.Date)
                                               .Where(a => !a.ExpirationDate.HasValue || a.ExpirationDate.Value.Date >= endDate.Date)
                                               .Where(v => v.Claimed && !v.OnHold)
                                               .Sum(c => c.CreditEarned);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="earliestCertDate"></param>
        /// <param name="checkDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        public static bool ComputeReciprocityByWindows(this IEnumerable<ActivityResource> userActivities,
                                                    DateTime earliestCertDate,
                                                    DateTime checkDate,
                                                    WindowsIntervalType windowsInterval)
        {

            var window = ProgramRulesHelpers.ComputeLookBackWindow(earliestCertDate,
                                                                        checkDate,
                                                                        windowsInterval);
            return userActivities.EnrolledInReprocity(window.Item1, window.Item2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        // ** reciprocity Program
        public static bool EnrolledInReprocity(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            return userActivities.Where(p => p.Product.Code == ProductResourceConstants.ProductCode.ReciprocityAttest) //  "Reciprocity"
                                    .Where(p => p.CompletedDate.HasValue
                                        && p.CompletedDate.Value.Date >= startDate.Date
                                        && p.CompletedDate.Value.Date <= endDate.Date)
                                    //the reciprocity is good for 3 years from the completion date unless it is cancelled (status changed from pass to cancel) 
                                    //in which case the cancel date is also set and it is then considered good from credit date to cancel date
                                    .Where(p => p.CompletedDate.Value.AddYears(3).AddDays(-1).Date >= endDate.Date)
                                    .Where(p => p.ActivityResult.Value == ActivityResultType.Pass.ToString()
                                             //If the status is cancelled or revoked, verify the cancel date  >= starting date + 2 years.
                                             || ((p.ActivityResult.Value == ActivityResultType.Cancelled.ToString()
                                                    || p.ActivityResult.Value == ActivityResultType.Revoked.ToString())
                                                    && (!p.CancelledDate.HasValue || p.CancelledDate.Value.Date >= startDate.AddYears(2).Date)))
                                    .Any();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="userActivities"></param>
        ///// <param name="startDate"></param>
        ///// <param name="endDate"></param>
        ///// <returns></returns>
        //public static DateTime? attestationDate(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        //{
        //    return userActivities.Where(p => p.Product.Code == ProductResourceConstants.ProductCode.ReciprocityAttest) //"Reciprocity"
        //                                        .Where(p => p.CompletedDate.HasValue 
        //                                            && p.CompletedDate.Value.Date >= startDate.Date
        //                                            && p.CompletedDate.Value.Date <= endDate.Date)
        //                                        .Where(p => p.ActivityResult.Value == ActivityResultType.Pass.ToString()
        //                                            || (p.ActivityResult.Value == ActivityResultType.Cancelled.ToString() && DateTime.Compare(p.CancelledDate.Value.Date, startDate.AddYears(2)) >= 0))
        //                                        .Max(p => p.CompletedDate);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userActivities"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static decimal anyMOCpoints(this IEnumerable<ActivityResource> userActivities, DateTime startDate, DateTime endDate)
        {
            return userActivities.Where(d => d.CompletedDate.HasValue
                                            && d.CompletedDate.Value.Date >= startDate.Date
                                            && d.CompletedDate.Value.Date <= endDate.Date)
                                    .Where(a => a.ActivityResult.Value == ActivityResultType.Pass.ToString())
                                    .Sum(p => p.TotalMOCPoints);
        }

        #endregion
    }
}
