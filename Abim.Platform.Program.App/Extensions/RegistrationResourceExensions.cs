using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Enterprise.Core.Relational.Classes;
using Abim.Enterprise.Core.Util.EnumResource;
using System;
using System.Linq;

namespace Abim.Platform.Program.App.Extensions.Registration
{
    public static class RegistrationResourceExensions
    {
        #region RegistrationResource

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this EnumValueResource<TEnum> resource)
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            return EnumAttributes.ToEnum<TEnum>(resource.Code);
        }

        /// <summary>
        /// Exam Test Date is used to determine the date the person took the exam. 
        /// Not all people have a seat appointment (eg. old data, exams not administered by abim) 
        /// so we have to fall back to admin date when there is no testing date.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static DateTime ExamTestDate(this RegistrationResource registration)
        {
            // based on DB structure AdministrationDate cannot be null in Administration table
            return registration.Seats.Count > 0 ? registration.Seats.Min(a => a.SeatDate) : registration.AdministrationDate;
        }

        /// <summary>
        /// Returns the minimum seat date (if there are seats) or the delivery start date.
        /// If neither are present, returns the administration date.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        public static DateTime MinSeatOrDeliveryDate(this RegistrationResource registration)
        {
            if (registration.Seats != null && registration.Seats.Any())
                return registration.Seats.Min(x => x.SeatDate);
            else
            {
                if (registration.DeliveryStartDate.HasValue)
                    return registration.DeliveryStartDate.Value;
                else
                    return registration.AdministrationDate;
            }
        }

        /// <summary>
        /// Returns the "effective" exam result. When an exam is a no-consequences exam, 
        /// FAIL, INDETERMINATE, INCOMPLETE, and UNABLETOTEST are treated as PASS.
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="credentialExamDueDate">The exam due date of the credential this exam registration applies to</param>
        /// <param name="consecutiveKCIPassRequired">Specifies whether or not the diplomate is required to pass 2 consecutive KCI exams</param>
        /// <returns>The "effective" exam result as determined by the business criteria</returns>
        public static ExamResultType GetEffectiveExamResult(
            this RegistrationResource registration,
            DateTime credentialExamDueDate,
            bool consecutiveKCIPassRequired)
        {
            string[] effectivePassingResultTypes =
                new string[] { "FAIL", "INDETERMINATE", "INCOMPLETE", "UNABLETOTEST" };

            if (registration.NoConsequence
                && effectivePassingResultTypes.Contains(registration.ExamResult.Result.Value.ToUpper())
                && registration.AdministrationYear <= credentialExamDueDate.Year
                && !consecutiveKCIPassRequired)
                return ExamResultType.Pass;
            else
                return registration.ExamResult.Result.ToEnum();
        }


        /// <summary>
        /// Determines whether the specified registration is 2 Year Kci.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        ///   <c>true</c> if the specified registration is 2 Year MOC; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsKci(this RegistrationResource registration)
        {
            return (registration.ExamType != null && registration.ExamType.Value == ExamType.Kci.ToString());
        }

        /// <summary>
        /// Determines whether the specified registration is 10 Year MOC.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        ///   <c>true</c> if the specified registration is 2 Year MOC; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMoc(this RegistrationResource registration)
        {
            return (registration.ExamType != null && registration.ExamType.Value == ExamType.Moc.ToString());
        }

        /// <summary>
        /// Determines whether the specified registration is Initial cert exam.
        /// </summary>
        /// <param name="registration">The registration.</param>
        /// <returns>
        ///   <c>true</c> if the specified registration is 2 Year MOC; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInitial(this RegistrationResource registration)
        {
            return (registration.Result == ExamType.Cert.ToString());
        }

        public static bool IsPassExam(this RegistrationResource registration)
        {
            return (registration.Result == ExamResultType.Pass.ToString());
        }

        public static bool IsFailExam(this RegistrationResource registration)
        {
            return (registration.Result == ExamResultType.Fail.ToString());
        }

        //public static bool IsFailIndIncUtt(this RegistrationResource registration)
        //{
        //    return ExamResultConstants.ExamResultFailIndIncUtt.Contains(registration.ExamResult.Result.ToEnum());
        //}

        //public static bool IsPassFailIndInvIncUtt(this RegistrationResource registration)
        //{
        //    return ExamResultConstants.ExamResultPassFailIndInvIncUtt.Contains(registration.ExamResult.Result.ToEnum());
        //}

        //public static bool IsPassFailIndIncUtt(this RegistrationResource registration)
        //{
        //    return ExamResultConstants.ExamResultPassFailIndIncUtt.Contains(registration.ExamResult.Result.ToEnum());
        //}

        //public static bool IsIndIncUtt(this RegistrationResource registration)
        //{
        //    return ExamResultConstants.ExamResultIndIncUtt.Contains(registration.ExamResult.Result.ToEnum());
        //}

        #endregion
    }
}
