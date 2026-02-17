using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// SetIssuanceToMaintainedCommandValidator Class.
    /// </summary>
    public class SetIssuanceToMaintainedCommandValidator : AbstractValidator<SetIssuanceToMaintainedCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetIssuanceToMaintainedCommandValidator"/> class.
        /// </summary>
        public SetIssuanceToMaintainedCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.ModifiedBy).NotEmpty()
                .WithMessage("ModifiedBy is required");
        }
    }
}
