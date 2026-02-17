using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Certification
{
    /// <summary>
    /// The Validation class for the AddCertificationCommand
    /// </summary>
    public class AddCertificationCommandValidator : AbstractValidator<AddCertificationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCertificationCommandValidator"/> class.
        /// </summary>
        public AddCertificationCommandValidator()
        {
            RuleFor(o => o.SourceId).NotEqual(Guid.Empty)
                .WithMessage("SourceId is required");

            RuleFor(o => o.Code).NotEmpty()
                .WithMessage("Code is required");

            RuleFor(o => o.Name).NotEmpty()
                .WithMessage("Name is required");

            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");

            RuleFor(x => x.UserInfo.Username).NotEmpty().When(x => x.UserInfo != null)
                .WithMessage("UserInfo's UserName cannot be empty");
        }

    }

}
