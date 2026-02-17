using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the UpdatePathwayCommand
    /// </summary>
    public class UpdatePathwayCommandValidator : AbstractValidator<UpdatePathwayCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePathwayCommandValidator"/> class.
        /// </summary>
        public UpdatePathwayCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");
            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
            RuleFor(x => x.Pathway).NotEmpty()
                .WithMessage("Pathway must be specified");
        }
    }
}
