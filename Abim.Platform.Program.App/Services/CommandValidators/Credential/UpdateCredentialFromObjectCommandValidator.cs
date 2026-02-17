using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the UpdateCredentialFromObjectCommand
    /// </summary>
    public class UpdateCredentialFromObjectCommandValidator : AbstractValidator<UpdateCredentialFromObjectCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCredentialFromObjectCommandValidator"/> class.
        /// </summary>
        public UpdateCredentialFromObjectCommandValidator()
        {
            RuleFor(o => o.Credential.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            RuleFor(x => x.Credential.AuditData).NotNull()
                .WithMessage("AuditData must not be null");

            RuleFor(x => x.Credential.AuditData.ModifiedBy).NotEmpty().When(x => x.Credential.AuditData != null)
                .WithMessage("ModifiedBy cannot be empty");
        }

    }

}
