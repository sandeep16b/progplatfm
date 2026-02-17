using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// The Validation class for MarkCertificateForSelectCommand
    /// </summary>
    public class MarkCertificateForSelectCommandValidator : AbstractValidator<MarkCertificateForSelectCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkCertificateForSelectCommandValidator"/> class.
        /// </summary>
        public MarkCertificateForSelectCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required.");

            RuleFor(o => o.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null.");

            RuleFor(o => o.UserInfo.Username).NotEmpty().When(o => o.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty.");
        }
    }
}
