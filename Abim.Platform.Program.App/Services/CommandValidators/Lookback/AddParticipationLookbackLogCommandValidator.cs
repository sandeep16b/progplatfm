using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Lookback
{
    /// <summary>
    /// 
    /// </summary>
    public class AddParticipationLookbackLogCommandValidator : 
        AbstractValidator<AddParticipationLookbackLog>
    {
        /// <summary>
        /// 
        /// </summary>
        public AddParticipationLookbackLogCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty).WithMessage("CredentialId is required");
            RuleFor(o => o.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
}
