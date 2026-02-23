using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the UpdateGrandfatherMOCPrintDateCommand
    /// </summary>
    public class UpdateGrandfatherMOCPrintDateCommandValidator : AbstractValidator<UpdateGrandfatherMOCPrintDateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateGrandfatherMOCPrintDateCommandValidator"/> class.
        /// </summary>
        public UpdateGrandfatherMOCPrintDateCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.ModifiedBy).NotEmpty()
               .WithMessage("ModifiedBy is required");
        }
    }
}
