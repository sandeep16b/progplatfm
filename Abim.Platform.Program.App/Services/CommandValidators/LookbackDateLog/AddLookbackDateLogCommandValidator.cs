using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Lookback
{
    /// <summary>
    /// Command Validator class for LookbackDateLog
    /// </summary>
    public class AddLookbackDateLogCommandValidator: AbstractValidator<AddLookbackDateLogEntry>
    {
        /// <summary>
        /// AddLookbackDateLogCommandValidator
        /// </summary>
        public AddLookbackDateLogCommandValidator()
        {
            RuleFor(o => o.ChangedDate).NotEqual(DateTime.MinValue).WithMessage("ChangedDate is required");

            RuleFor(o => o.MemberGuid).NotEqual(Guid.Empty).WithMessage("MemberGuid is required");

            RuleFor(o => o.LookbackDate).NotEqual(default(LookbackDateType)).WithMessage("LookbackDate is required");

            RuleFor(o => o.NewValue).NotEqual(DateTime.MinValue).When(o => o.NewValue != null).WithMessage("NewValue is required");

            RuleFor(o => o.OldValue).NotEqual(DateTime.MinValue).When(o => o.OldValue != null).WithMessage("OldValue is required");

            RuleFor(o => o.UserName).NotEmpty().WithMessage("UserName is required");
        }

    }
}
