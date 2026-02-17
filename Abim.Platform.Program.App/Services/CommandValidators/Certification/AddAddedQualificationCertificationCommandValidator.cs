using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// AddAddedQualificationCertificationCommandValidator Class.
    /// </summary>
    public class AddAddedQualificationCertificationCommandValidator : AbstractValidator<AddAddedQualificationCertificationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddAddedQualificationCertificationCommandValidator"/> class.
        /// </summary>
        public AddAddedQualificationCertificationCommandValidator()
        {
            RuleFor(o => o.BaseId).NotEqual(Guid.Empty)
                .WithMessage("BaseId is required");
            RuleFor(o => o.Name).NotEmpty()
                .WithMessage("Name is required");
            RuleFor(o => o.Code).NotEmpty()
                .WithMessage("Code is required");
        }
    }
}
