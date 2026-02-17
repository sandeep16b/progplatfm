using Abim.Platform.Program.App.Services.Commands;
using FluentNHibernate.Conventions;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// The Validation class for MarkCertificatesForSelectOrDeselectCommand
    /// </summary>
    public class MarkCertificatesForSelectOrDeselectCommandValidator : AbstractValidator<MarkCertificatesForSelectOrDeselectCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkCertificatesForSelectOrDeselectCommandValidator"/> class.
        /// </summary>
        public MarkCertificatesForSelectOrDeselectCommandValidator()
        {
            RuleFor(o => o.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            RuleFor(o => o.CredentialIdsForDeselect).NotEmpty()
                .When(o => o.CredentialIdsForSelect == null || o.CredentialIdsForSelect.IsEmpty())
                .WithMessage("CredentialIdsForSelect or CredentialIdsForDeselect are required.");

            RuleFor(o => o.CredentialIdsForSelect).NotEmpty()
                .When(o => o.CredentialIdsForDeselect == null || o.CredentialIdsForDeselect.IsEmpty())
                .WithMessage("CredentialIdsForSelect or CredentialIdsForDeselect are required.");

            RuleForEach(o => o.CredentialIdsForDeselect).NotEqual(Guid.Empty)
                .WithMessage("All CredentialIdsForDeselect must be valid, non-default GUIDs.");

            RuleForEach(o => o.CredentialIdsForSelect).NotEqual(Guid.Empty)
                .WithMessage("All CredentialIdsForSelect must be valid, non-default GUIDs.");
        }
    }
}
