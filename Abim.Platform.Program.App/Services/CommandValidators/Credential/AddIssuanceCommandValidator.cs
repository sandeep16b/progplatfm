using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Issuance
{
    /// <summary>
    /// The Validation class for the AddIssuanceCommand
    /// </summary>
    public class AddIssuanceCommandValidator : AbstractValidator<AddIssuanceCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddIssuanceCommandValidator"/> class.
        /// </summary>
        public AddIssuanceCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");

            RuleFor(o => o.IssuanceDate).NotEqual(DateTime.MinValue)
               .WithMessage("IssuanceDate is required");

            RuleFor(o => o.EffectiveDate).NotEqual(DateTime.MinValue)
               .WithMessage("EffectiveDate is required");

            RuleFor(o => o.SourceId).NotEqual(Guid.Empty)
                    .WithMessage("SourceId is required");

            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");

            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
        }

    }

}
