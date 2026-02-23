using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.App.Extensions.Registration;
using System;
using System.Linq;

namespace Abim.Platform.Program.App.Util
{
    public class RegistrationData
    {
        #region Properties

        /// <summary>
        /// The ID of the registration
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The date of the exam administration.
        /// </summary>
        public DateTime AdministrationDate { get; }

        /// <summary>
        /// The year of the exam administration. This is a separate property because it can be sourced differently in the resource object.
        /// </summary>
        public int AdministrationYear { get; }

        /// <summary>
        /// The GUID ID of the certification.
        /// </summary>
        public Guid CertificationId { get; }

        /// <summary>
        /// The (actual) exam result.
        /// </summary>
        public ExamResultResource ExamResult { get; }

        /// <summary>
        /// Specifies whether or not the "registration" is CMP.
        /// </summary>
        public bool IsCmp { get; }

        /// <summary>
        /// Specifies whether or not the registration is KCI.
        /// </summary>
        public bool IsKci { get; }

        /// <summary>
        /// Specifies whether or not the registration is MOC.
        /// </summary>
        public bool IsMoc { get; }

        /// <summary>
        /// The Member GUID of the member.
        /// </summary>
        public Guid MemberId { get; }

        /// <summary>
        /// For RegistrationResource, this is the minimum seat date (if there are seats) or the delivery start date.
        /// If neither are present, returns the administration date.
        /// For CMPRegistrationResource, this is the exam date.
        /// </summary>
        public DateTime MinSeatOrDeliveryDate { get; }

        /// <summary>
        /// Specifies whether or not the exam was No Consequence.
        /// </summary>
        public bool NoConsequence { get; }

        /// <summary>
        /// Specifies whether or not the exam results are on hold due to lack of payment. Applies to CMP only.
        /// </summary>
        public bool OnHold { get; }

        /// <summary>
        /// Specifies whether or not the physician is an ABIM physician
        /// </summary>
        public bool PhysicianIsAbim { get; }

        /// <summary>
        /// The date of the exam
        /// </summary>
        public DateTime ExamTestDate { get; }

        /// <summary>
        /// The board the registration was administered on behalf of
        /// </summary>
        public string OnBehalfOf { get; }

        /// <summary>
        /// Indicates whether or not the registration was administered on behalf of another board
        /// </summary>
        public bool IsCosponsored { get { return !String.IsNullOrWhiteSpace(OnBehalfOf); } }

        #endregion Properties

        #region Fields

        private string[] _effectivePassingResultTypes =
            new string[] { "FAIL", "INDETERMINATE", "INCOMPLETE", "UNABLETOTEST" };

        private Enterprise.Core.Util.EnumResource.EnumValueResource<ExamType> _examType; //Only for our registrations. This doesn't get set for ACC b/c we don't have true registrations for them.

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor to instantiate an instance of the object using a RegistrationResource as the source of it's data
        /// </summary>
        /// <param name="reg">The RegistrationResource to use as the source</param>
        public RegistrationData(RegistrationResource reg)
        {
            Id = reg.Id;

            AdministrationDate = reg.AdministrationDate;
            AdministrationYear = reg.AdministrationYear;
            CertificationId = reg.CertificationId;
            ExamTestDate = reg.ExamTestDate();
            ExamResult = reg.ExamResult;
            _examType = reg.ExamType;
            IsCmp = false;
            IsKci = reg.IsKci();
            IsMoc = reg.IsMoc();
            IsCmp = false;
            MemberId = reg.MemberId;
            MinSeatOrDeliveryDate = reg.MinSeatOrDeliveryDate();
            NoConsequence = reg.NoConsequence;
            PhysicianIsAbim = reg.PhysicianIsAbim;
            OnBehalfOf = reg.OnBehalfOf;
        }

        /// <summary>
        /// Constructor to instantiate an instance of the object using a CMPRegistrationResource as the source of it's data
        /// </summary>
        /// <param name="reg">The CMPRegistrationResource to use as the source</param>
        public RegistrationData(CMPRegistrationResource reg)
        {
            Id = reg.Id;

            AdministrationDate = reg.TestDate;
            AdministrationYear = reg.TestDate.Year;

            CertificationId = reg.CMPExam.CertificationId;

            ExamTestDate = reg.TestDate;
            ExamResult = GetExamResultFromCMPExamResult(reg.ExamResult.ToEnum());

            IsCmp = true;
            IsKci = false;
            IsMoc = false;
            MemberId = reg.MemberId;

            MinSeatOrDeliveryDate = reg.TestDate;
            //NoConsequence = reg.CMPExam.NoConsequence;
            NoConsequence = reg.CMPExam.NoConsequenceYears.Any(x => x == reg.TestDate.Year);
            OnHold = reg.OnHold;
            PhysicianIsAbim = reg.PhysicianIsAbim;
        }

        #endregion Constructors

        /// <summary>
        /// Returns the "effective" exam result. When an exam is a no-consequences exam, 
        /// FAIL, INDETERMINATE, INCOMPLETE, and UNABLETOTEST are treated as PASS.
        /// </summary>
        /// <param name="credentialExamDueDate">The exam due date of the credential this exam registration applies to</param>
        /// <param name="consecutiveKCIPassRequired">Specifies whether or not the diplomate is required to pass 2 consecutive KCI exams</param>
        /// <returns>The "effective" exam result as determined by the business criteria</returns>
        /// <remarks>
        /// See PBI 134151 for more information. 
        /// Note that the PBI does not explicitly mention that the exam was taken before it was due, but it is implied.
        /// </remarks>
        public ExamResultType GetEffectiveExamResult(
            DateTime credentialExamDueDate,
            bool consecutiveKCIPassRequired)
        {
            if (NoConsequence
                && _effectivePassingResultTypes.Contains(ExamResult.Result.Value.ToUpper())
                && AdministrationYear <= credentialExamDueDate.Year
                && !consecutiveKCIPassRequired)
                return ExamResultType.Pass;
            else
                return ExamResult.Result.ToEnum();
        }

        public string GetExamType()
        {
            //CMPRegistrations don't have a corresponding exam type in our system, so here's our workaround
            if (IsCmp)
                return "ACC"; //Suggested by Dan
            else
                return _examType.Value.ToUpper();
        }

        private ExamResultResource GetExamResultFromCMPExamResult(ExamResultType cmpResult)
        {
            var output = new ExamResultResource();

            if (cmpResult == ExamResultType.Fail)
                output.Result = new Extensions.ExternalResponses.EnumValueResponseResource<ExamResultType>(ExamResultType.Fail);
            else if (cmpResult == ExamResultType.Pass)
                output.Result = new Extensions.ExternalResponses.EnumValueResponseResource<ExamResultType>(ExamResultType.Pass);
            else if (cmpResult == ExamResultType.Pending)
                output.Result = new Extensions.ExternalResponses.EnumValueResponseResource<ExamResultType>(ExamResultType.Pending);
            else if (cmpResult == ExamResultType.UnableToTest)
                output.Result = new Extensions.ExternalResponses.EnumValueResponseResource<ExamResultType>(ExamResultType.UnableToTest);
            else if (cmpResult == ExamResultType.NoShow)
                output.Result = new Extensions.ExternalResponses.EnumValueResponseResource<ExamResultType>(ExamResultType.NoShow);
            else
                throw new ApplicationException($"Unknown ExamResultType found in RegistrationData.GetExamResultFromCMPExamResult(): {cmpResult}");

            return output;
        }
    }

}
