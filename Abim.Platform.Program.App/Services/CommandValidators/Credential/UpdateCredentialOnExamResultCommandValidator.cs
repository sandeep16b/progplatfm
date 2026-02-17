using System;
using FluentValidation;
using Abim.Platform.Program.App.Services.Commands;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// UpdateCredentialGracePeriodForTLandMBMValidator Class.
    /// </summary>
    public class UpdateCredentialOnExamResultCommandValidator : AbstractValidator<UpdateCredentialOnExamResultCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCredentialOnExamResultCommandValidator"/> class.
        /// </summary>
        public UpdateCredentialOnExamResultCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.ModifiedBy).NotEmpty()
                .WithMessage("ModifiedBy is required");
        }
    }
}
