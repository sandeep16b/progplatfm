using System;
using System.ComponentModel.DataAnnotations;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using FluentValidation;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// A class representing a record in the CredentialDateLog table
    /// </summary>
    public class CredentialDateLog : Entity
    {
        #region Properties
        /// <summary>
        /// Credential
        /// </summary>
        public virtual Credential Credential        { get; protected internal set; }
        /// <summary>
        /// DateType
        /// </summary>
        public virtual CredentialDateType DateType  { get; protected internal set; }
        /// <summary>
        /// OldValue
        /// </summary>
        public virtual DateTime? OldValue           { get; protected internal set; }
        /// <summary>
        /// NewValue
        /// </summary>
        public virtual DateTime? NewValue           { get; protected internal set; }
        /// <summary>
        /// ChangedDate
        /// </summary>
        public virtual DateTime ChangedDate         { get; protected internal set; }

        #endregion Properties

        #region Impl methods
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as CredentialDateLog;
            if (other == null)
                return false;

            return (Id == other.Id);
        }

        #endregion Impl methods

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="cred"></param>
        /// <param name="dateType"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="changedDate"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public static CredentialDateLog Create(
            Credential cred, 
            CredentialDateType dateType, 
            DateTime? oldValue, 
            DateTime? newValue, 
            DateTime changedDate, 
            string createdBy)
        {
            return new CredentialDateLog
            {
                Credential = cred,
                DateType = dateType,
                OldValue = oldValue,
                NewValue = newValue,
                ChangedDate = changedDate,
                AuditData = AuditData.Create(createdBy)
            };
        }
    }

    /// <summary>
    /// CredentialDateType
    /// </summary>
    public enum CredentialDateType
    {
        /// <summary>
        /// AssessmentMetDate
        /// </summary>
        [Display(Name = "AssessmentMetDate", ShortName = "A", Description = "Assessment Met Date")]
        AssessmentMetDate = 'A',

        /// <summary>
        /// ExamDueDate
        /// </summary>
        [Display(Name = "ExamDueDate", ShortName = "E", Description = "Exam Due Date")]
        ExamDueDate = 'E',

        /// <summary>
        /// DisplayExamDueDate
        /// </summary>
        [Display(Name = "DisplayExamDueDate", ShortName = "D", Description = "Display Exam Due Date")]
        DisplayExamDueDate = 'D',

        /// <summary>
        /// KCIExamDueDate
        /// </summary>
        [Display(Name = "KCIExamDueDate", ShortName = "K", Description = "KCI Exam Due Date")]
        KCIExamDueDate = 'K',

        /// <summary>
        /// MOCExamDueDate
        /// </summary>
        [Display(Name = "MOCExamDueDate", ShortName = "M", Description = "MOC Exam Due Date")]
        MOCExamDueDate = 'M',

        /// <summary>
        /// ExpirationDate
        /// </summary>
        [Display(Name = "ExpirationDate", ShortName = "X", Description = "Expiration Date")]
        ExpirationDate = 'X',

        /// <summary>
        /// GracePeriodStartDate
        /// </summary>
        [Display(Name = "GracePeriodStartDate", ShortName = "G", Description = "Grace Period Start Date")]
        GracePeriodStartDate = 'G',

        /// <summary>
        /// GracePeriodEndDate
        /// </summary>
        [Display(Name = "GracePeriodEndDate", ShortName = "Z", Description = "Grace Period End Date")]
        GracePeriodEndDate = 'Z'

    }

    /// <summary>
    /// CredentialDateLogValidator
    /// </summary>
    public class CredentialDateLogValidator : AbstractValidator<CredentialDateLog>
    {
        /// <summary>
        /// CredentialDateLogValidator
        /// </summary>
        public CredentialDateLogValidator()
        {
            RuleFor(x => x.Credential).NotNull().WithMessage("Credential is required");
            RuleFor(x => x.ChangedDate).NotEqual(DateTime.MinValue).WithMessage("ChangedDate is required");
            RuleFor(x => x.AuditData).NotNull().WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(new AuditData.AuditDataValidator());
        }
    }
}
