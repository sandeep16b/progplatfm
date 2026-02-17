using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// RunRulesForMustBeMaintainedCertificateCommandValidator Class.
    /// </summary>
    public class RunRulesForMustBeMaintainedCertificateCommandValidator : AbstractValidator<RunRulesForMustBeMaintainedCertificateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunRulesForMustBeMaintainedCertificateCommandValidator"/> class.
        /// </summary>
        public RunRulesForMustBeMaintainedCertificateCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.IssuanceId).NotEqual(0)
                .WithMessage("IssuanceId is required");
            RuleFor(o => o.EventDate).NotEqual(DateTime.MinValue)
                .WithMessage("EventDate is required");
            RuleFor(o => o.CreatedBy).NotEmpty()
                .WithMessage("CreatedBy is required");
        }
    }
}
