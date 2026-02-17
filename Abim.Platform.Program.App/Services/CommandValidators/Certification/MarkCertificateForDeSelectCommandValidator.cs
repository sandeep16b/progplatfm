using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// The Validation class for MarkCertificateForDeSelectCommand
    /// </summary>
    public class MarkCertificateForDeselectCommandValidator : AbstractValidator<MarkCertificateForDeselectCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkCertificateForDeselectCommandValidator"/> class.
        /// </summary>
        public MarkCertificateForDeselectCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required.");

            RuleFor(o => o.SubmittedDate).NotEqual(DateTime.MinValue)
                .WithMessage("SubmittedDate is required.");

            RuleFor(o => o.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null.");

            RuleFor(o => o.UserInfo.Username).NotEmpty().When(o => o.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty.");
        }
    }
}
