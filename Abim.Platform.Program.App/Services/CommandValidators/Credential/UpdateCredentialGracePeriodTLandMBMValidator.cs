using System;
using FluentValidation;
using Abim.Platform.Program.App.Services.Commands;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// UpdateCredentialGracePeriodForTLandMBMValidator Class.
    /// </summary>
    public class UpdateCredentialGracePeriodTLandMBMValidator : AbstractValidator<UpdateCredentialGracePeriodTLandMBMCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCredentialGracePeriodTLandMBMValidator"/> class.
        /// </summary>
        public UpdateCredentialGracePeriodTLandMBMValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.ModifiedBy).NotEmpty()
                .WithMessage("ModifiedBy is required");
        }
    }
}
