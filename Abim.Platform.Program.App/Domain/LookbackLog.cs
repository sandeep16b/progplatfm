using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;


namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// LookbackLog Class.
    /// </summary>
    public class LookbackLog : AggregateRoot<LookbackLog>,
        IDomainValidationHandler<LookbackLog>
    {
        #region Propeties
        /// <summary>
        /// Credential
        /// </summary>
        public virtual Credential Credential     { get; protected internal set; }
        /// <summary>
        /// Action
        /// </summary>
        public virtual LookbackActionType Action { get; protected internal set; }
        /// <summary>
        /// Reason
        /// </summary>
        public virtual LookbackReasonType Reason { get; protected internal set; }
        /// <summary>
        /// Status
        /// </summary>
        public virtual LookbackStatusType Status { get; protected internal set; }
        /// <summary>
        /// IsPendingAction
        /// </summary>
        public virtual bool IsPendingAction      { get; protected internal set; }
        /// <summary>
        /// LogDate
        /// </summary>
        public virtual DateTime LogDate          { get; protected internal set; }

        #endregion

        #region Impl methods

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as LookbackLog;
            if (other == null)
                return false;

            return (ExternalId == other.ExternalId);
        }

        #endregion

        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="crendtial"></param>
        /// <param name="reason"></param>
        /// <param name="action"></param>
        /// <param name="status"></param>
        /// <param name="username"></param>
        /// <param name="isPending"></param>
        /// <param name="lookbackLogDate"></param>
        /// <returns></returns>
        public static LookbackLog Create(Credential crendtial, LookbackReasonType reason, LookbackActionType action, LookbackStatusType status, string username, bool isPending, DateTime lookbackLogDate)
        {
            return new LookbackLog
            {
                Credential      = crendtial,
                Action          = action,
                ExternalId      = Guid.NewGuid(),
                AuditData       = AuditData.Create(username),
                IsPendingAction = isPending,
                LogDate         = lookbackLogDate,
                Reason          = reason,
                Status          = status
            };
        }
    }

    #region Validation Classes

    /// <summary>
    /// LookbackLogValidator Class.
    /// </summary>
    public class LookbackLogValidator : 
        AbstractValidator<LookbackLog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookbackLogValidator"/> class.
        /// </summary>
        public LookbackLogValidator()
        {
            RuleFor(x => x.Credential).NotNull().WithMessage("Credential is required");
            RuleFor(x => x.LogDate).NotEqual(DateTime.MinValue).WithMessage("LogDate is required");
            RuleFor(x => x.AuditData).NotNull().WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(new AuditData.AuditDataValidator());
        }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public enum LookbackReasonType 
    {
        /// <summary>
        /// TwoYear
        /// </summary>
        [Display(Description = "Two Year", Name = "TwoYear", ShortName = "T")]
        TwoYear     = 84,
        /// <summary>
        /// FiveYear
        /// </summary>
        [Display(Description = "Five Year", Name = "FiveYear", ShortName = "F")]
        FiveYear    = 70,
        /// <summary>
        /// Attestation
        /// </summary>
        [Display(Description = "Attestation", Name = "Attestation", ShortName = "A")]
        Attestation = 65,
        /// <summary>
        /// Assessment
        /// </summary>
        [Display(Description = "Assessment", Name = "Assessment", ShortName = "S")]
        Assessment  = 83,
        /// <summary>
        /// Certification
        /// </summary>
        [Display(Description = "Certification", Name = "Certification", ShortName = "C")]
        Certification = 67 
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LookbackActionType
    {
        /// <summary>
        /// FailurePoint
        /// </summary>
        [Display(Description = "Failure Point", Name = "FailurePoint", ShortName = "F")]
        FailurePoint     = 70,
        /// <summary>
        /// RestorationPoint
        /// </summary>
        [Display(Description = "Restoration Point", Name = "RestorationPoint", ShortName = "R")]
        RestorationPoint = 82
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LookbackStatusType
    {
        /// <summary>
        /// CredentialStatus
        /// </summary>
        [Display(Description = "Credential Status", Name = "CredentialStatus", ShortName = "C")]
        CredentialStatus    = 67,
        /// <summary>
        /// ParticipationStatus
        /// </summary>
        [Display(Description = "Participation Status", Name = "ParticipationStatus", ShortName = "P")]
        ParticipationStatus = 80
    }
}
