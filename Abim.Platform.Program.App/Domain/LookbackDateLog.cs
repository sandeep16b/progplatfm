using System;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// LookbackDateLog
    /// </summary>
    public class LookbackDateLog : AggregateRoot<LookbackDateLog>
    {
        #region Propeties
        /// <summary>
        /// ChangedDate
        /// </summary>
        public virtual DateTime ChangedDate { get; protected internal set; }
        /// <summary>
        /// MemberGuid
        /// </summary>
        public virtual Guid MemberGuid { get; protected internal set; }
        /// <summary>
        /// LookbackDate
        /// </summary>
        public virtual LookbackDateType LookbackDate { get; protected internal set; }
        /// <summary>
        /// NewValue
        /// </summary>
        public virtual DateTime? NewValue { get; protected internal set; }
        /// <summary>
        /// OldValue
        /// </summary>
        public virtual DateTime? OldValue { get; protected internal set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changedDate"></param>
        /// <param name="memberGuid"></param>
        /// <param name="lookbackDate"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static LookbackDateLog Create(DateTime changedDate, Guid memberGuid, LookbackDateType lookbackDate, DateTime? newValue, DateTime? oldValue, string userName)
        {
            return new LookbackDateLog
            {
                ChangedDate = changedDate,
                MemberGuid = memberGuid,
                LookbackDate = lookbackDate,
                NewValue = newValue,
                OldValue = oldValue,
                AuditData = AuditData.Create(userName)
            };
        }
    }
    /// <summary>
    /// LookbackDateType
    /// </summary>
    public enum LookbackDateType
    {
        /// <summary>
        /// TwoYearStart
        /// </summary>
        [Display(Description = "Two Year Start", Name = "TwoYearStart", ShortName = "S")]
        TwoYearStart = 83,
        /// <summary>
        /// TwoYearEnd
        /// </summary>
        [Display(Description = "Two Year End", Name = "TwoYearEnd", ShortName = "E")]
        TwoYearEnd = 69,
        /// <summary>
        /// FiveYearStart
        /// </summary>
        [Display(Description = "Five Year Start", Name = "FiveYearStart", ShortName = "F")]
        FiveYearStart = 70,
        /// <summary>
        /// FiveYearEnd
        /// </summary>
        [Display(Description = "Five Year End", Name = "FiveYearEnd", ShortName = "Y")]
        FiveYearEnd = 89
    }

    /// <summary>
    /// LookbackDateLogValidator
    /// </summary>
    public class LookbackDateLogValidator : AbstractValidator<LookbackDateLog>
    {
        /// <summary>
        /// LookbackDateLogValidator
        /// </summary>
        public LookbackDateLogValidator(IValidationFactory factory)
        {
            RuleFor(o => o.ChangedDate).NotEqual(DateTime.MinValue).WithMessage("ChangedDate is required");

            RuleFor(o => o.MemberGuid).NotEqual(Guid.Empty).WithMessage("MemberGuid is required");

            RuleFor(o => o.LookbackDate).NotEqual(default(LookbackDateType)).WithMessage("LookbackDate is required");

            RuleFor(o => o.NewValue).NotEqual(DateTime.MinValue).When(o => o.NewValue != null).WithMessage("NewValue is required");

            RuleFor(o => o.OldValue).NotEqual(DateTime.MinValue).When(o => o.OldValue != null).WithMessage("OldValue is required");

            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");

            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }
}
