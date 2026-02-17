using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the UpdateLngAssessmentDueDateCommand
    /// </summary>
    public class UpdateLngAssessmentDueDateCommandValidator : AbstractValidator<UpdateLngAssessmentDueDateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLngAssessmentDueDateCommandValidator"/> class.
        /// </summary>
        public UpdateLngAssessmentDueDateCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");

            RuleFor(o => o.Year)
                .GreaterThan(2021)
                .WithMessage("Year value should be greater than 2021");

            RuleFor(o => o.Year)
                .LessThan(3000)
                .WithMessage("Year value should be less than 3000");

            RuleFor(o => o.MetParticipationStatus).Empty().When(o => o.PassSummativeDecision.HasValue)
                .WithMessage("MetParticipationStatus and PassSummativeDecision CANNOT be set for both, only one is allowed");

            RuleFor(o => o.MetParticipationStatus).NotEqual(true).When(o => o.PassSummativeDecision.HasValue && o.PassSummativeDecision.Value)
                .WithMessage("MetParticipationStatus and PassSummativeDecision CANNOT have 'true' value for both");
        }
    }
}
