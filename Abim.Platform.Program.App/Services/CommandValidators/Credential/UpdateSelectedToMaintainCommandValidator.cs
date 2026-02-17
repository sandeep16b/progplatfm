using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the UpdateSelectedToMaintainCommand
    /// </summary>
    public class UpdateSelectedToMaintainCommandValidator : AbstractValidator<UpdateSelectedToMaintainCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSelectedToMaintainCommandValidator"/> class.
        /// </summary>
        public UpdateSelectedToMaintainCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");
            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
        }
    }
}
