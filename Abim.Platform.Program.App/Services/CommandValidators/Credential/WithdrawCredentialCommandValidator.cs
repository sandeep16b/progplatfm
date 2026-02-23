using Abim.Platform.Program.Resources;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Extensions;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the WithdrawCredentialCommand
    /// </summary>
    public class WithdrawCredentialCommandValidator : AbstractValidator<WithdrawCredentialCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WithdrawCredentialCommandValidator"/> class.
        /// </summary>
        public WithdrawCredentialCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(l => l.WithdrawnStatus).In(IssuanceStatusType.Revoked, IssuanceStatusType.Surrendered, IssuanceStatusType.Suspended);
            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");
            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
        }

    }

}
