using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// ExpireIssuanceCommandValidator Class.
    /// </summary>
    public class ExpireIssuanceCommandValidator : AbstractValidator<ExpireIssuanceCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpireIssuanceCommandValidator"/> class.
        /// </summary>
        public ExpireIssuanceCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.IssuanceId).NotEqual(0)
                .WithMessage("IssuanceId is required");
            RuleFor(o => o.Status).NotEqual(default(IssuanceStatusType))
                .WithMessage("Status is required");
        }
    }
}
