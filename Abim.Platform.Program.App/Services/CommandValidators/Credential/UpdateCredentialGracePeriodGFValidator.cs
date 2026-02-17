using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// UpdateCredentialGracePeriodForGFValidator Class.
    /// </summary>
    public class UpdateCredentialGracePeriodGFValidator : AbstractValidator<UpdateCredentialGracePeriodGFCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCredentialGracePeriodGFValidator"/> class.
        /// </summary>
        public UpdateCredentialGracePeriodGFValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.ModifiedBy).NotEmpty()
                .WithMessage("ModifiedBy is required");
        }
    }
}
