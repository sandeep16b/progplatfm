using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// AddPrimaryCertificationCommandValidator Class.
    /// </summary>
    public class AddPrimaryCertificationCommandValidator : AbstractValidator<AddPrimaryCertificationCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddPrimaryCertificationCommandValidator"/> class.
        /// </summary>
        public AddPrimaryCertificationCommandValidator()
        {
            RuleFor(o => o.Name).NotEmpty()
                .WithMessage("Name is required");
            RuleFor(o => o.Code).NotEmpty()
                .WithMessage("Code is required");
        }
    }
}
