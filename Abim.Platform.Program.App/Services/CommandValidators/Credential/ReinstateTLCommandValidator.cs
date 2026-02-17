using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the ReinstateTLCommand
    /// </summary>
    public class ReinstateTLCommandValidator : AbstractValidator<ReinstateTLCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReinstateTLCommandValidator"/> class.
        /// </summary>
        public ReinstateTLCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(x => x.ModifiedBy).NotNull()
                .WithMessage("ModifiedBy must not be null");
        }

    }

}
