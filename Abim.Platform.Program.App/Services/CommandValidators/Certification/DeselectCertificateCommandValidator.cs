using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// The Validation class for DeselectCertificateCommandValidator
    /// </summary>
    public class DeselectCertificateCommandValidator : AbstractValidator<DeselectCertificateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeselectCertificateCommandValidator"/> class.
        /// </summary>
        public DeselectCertificateCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required.");

            RuleFor(o => o.ExpiredDate).NotEqual(DateTime.MinValue)
                .WithMessage("ExpiredDate is required.");

            RuleFor(o => o.Username).NotEmpty()
                .WithMessage("Username is required.");
        }
    }
}
