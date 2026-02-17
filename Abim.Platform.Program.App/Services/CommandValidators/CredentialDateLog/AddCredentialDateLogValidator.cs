using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.CredentialDateLog
{
    /// <summary>
    /// Validates an AddCredentialDateLog command object
    /// </summary>
    public class AddCredentialDateLogValidator : AbstractValidator<AddCredentialDateLog>
    {
        /// <summary>
        /// Creates a new instance of an AddCredentialDateLogValidator
        /// </summary>
        public AddCredentialDateLogValidator()
        {
            RuleFor(x => x.CredentialId).NotEmpty().WithMessage("CredentialId is required");
            RuleFor(x => x.ChangedDate).NotEqual(DateTime.MinValue).WithMessage("ChangedDate is required");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }
}
