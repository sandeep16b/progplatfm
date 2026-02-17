using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// Use to set the CorrectiveActionResult of where the Certification data was derived from
    /// </summary>
    public class CorrectiveActionResult : AggregateRoot<CorrectiveActionResult>
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid CredentialId { get; protected internal set; }

        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public virtual Guid MemberId { get; protected internal set; }

        /// <summary>
        /// Gets or sets the EventDate
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public virtual DateTime EventDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the MeetRule
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public virtual bool MeetRule { get; protected internal set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PBI { get; protected internal set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AdditionalResults { get; protected internal set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ValidationResults { get; protected internal set; }

        //----------- a c t u a l    r e s u l t s -----------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? IssuanceDate { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual MaintenanceStatusType? MaintenanceStatus { get; protected internal set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual CredentialCategoryType? CredentialCategory { get; protected internal set; }
        //------------------------------------------------------------------------------------------------------

        //--------------------- Attestation  ------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? Attestation { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? ReattestationDueDate { get; protected internal set; }

        //--------------------- Exam Requirement ------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? ExamRequirement { get; protected internal set; }
        //public virtual string ExamRequirementRange { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? PassMOCExam { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? PassKCIExam { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? PassCMPExam { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? ExamAssessmentMet { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string MOCExamTimeRange { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string KCIExamTimeRange { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string CMPExamTimeRange { get; protected internal set; }

        //-----------------------------------------------------------------------------------
        //--------------------- Five Year LookBack ------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? FiveYearLookBack { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string FiveYearLookBackRange { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual decimal? TotalMOCPoints { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual decimal? MedicalKnowledgePoints { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? Reciprocity { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool? NewSubspecialtyInitialCert { get; protected internal set; }
        //----------------------------------------------------------------------------------
        //--------------------- Maintenance Status  ------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual decimal? MaintenanceAnyMOCPoints { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string MaintenanceTwoYearLookBackRange { get; protected internal set; }

        //----------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime Created { get; protected internal set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string CreatedBy { get; protected internal set; }

        #endregion Properties

        #region Factory

        /// <summary>
        /// Creates the CorrectiveActionResult.
        /// </summary>
        /// <param name="credentialId">The credential Id.</param>
        /// <param name="memberId">The member Id.</param>
        /// <param name="eventDate">The event Date.</param>
        /// <param name="credentialCategory">The credential Category.</param>
        /// <param name="PBI">The PBI.</param>
        /// <param name="createdBy">The createdBy.</param>
        /// <returns></returns>
        public static CorrectiveActionResult Create(Guid credentialId, Guid memberId, DateTime eventDate, CredentialCategoryType credentialCategory,  string PBI, string createdBy)
        {

            var src = new CorrectiveActionResult()
            {
                CredentialId = credentialId,
                MemberId = memberId,
                EventDate = eventDate,
                CredentialCategory= credentialCategory,
                PBI= PBI,
                AuditData = AuditData.Create(createdBy),
                Created = DateTime.Now,
                CreatedBy = createdBy
            };
            return src;
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as CorrectiveActionResult;
            if (other == null)
                return false;

            return (ExternalId == other.ExternalId);
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <returns>
        ///   <c>HashCode</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override int GetHashCode()
        {
            return ExternalId.GetHashCode();
        }
    }

    #region Validator Classes

    /// <summary>
    /// Validator for CorrectiveActionResult
    /// </summary>
    public class CorrectiveActionRunValidator : AbstractValidator<CorrectiveActionResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectiveActionRunValidator"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public CorrectiveActionRunValidator(IValidationFactory factory)
        {
            RuleFor(x => x.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            RuleFor(x => x.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");

            RuleFor(x => x.EventDate).NotEqual(DateTime.MinValue)
                .WithMessage("EventDate is required");

            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }

    #endregion
}
