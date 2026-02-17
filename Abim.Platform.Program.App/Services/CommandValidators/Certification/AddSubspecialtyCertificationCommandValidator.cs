using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// AddSubspecialtyCertificationCommandValidator Class.
    /// </summary>
    public class AddSubspecialtyCertificationCommandValidator : AbstractValidator<AddSubspecialtyCertificationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddSubspecialtyCertificationCommandValidator"/> class.
        /// </summary>
        public AddSubspecialtyCertificationCommandValidator()
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
