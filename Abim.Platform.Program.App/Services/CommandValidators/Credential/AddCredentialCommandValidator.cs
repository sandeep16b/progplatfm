using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the AddCredentialCommand
    /// </summary>
    public class AddCredentialCommandValidator : AbstractValidator<AddCredentialCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCredentialCommandValidator"/> class.
        /// </summary>
        public AddCredentialCommandValidator()
        {
            RuleFor(o => o.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            RuleFor(o => o.CertificationId).NotEqual(Guid.Empty)
               .WithMessage("CertificationId is required");

            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");

            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
        }

    }

}
