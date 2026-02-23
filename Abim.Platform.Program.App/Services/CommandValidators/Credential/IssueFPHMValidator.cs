using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// IssueFPHMCommandValidator Class.
    /// </summary>
    public class IssueFPHMCommandValidator : AbstractValidator<IssueFPHMCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssueFPHMCommandValidator"/> class.
        /// </summary>
        public IssueFPHMCommandValidator()
        {
            RuleFor(o => o.IssuanceDate).Must(NotTooOldDate)
                .WithMessage("IssuanceDate may not be prior to 1936");
            RuleFor(o => o.IssuanceDate).LessThanOrEqualTo(f => f.ProcessingDate.AddYears(1))
                .WithMessage("IssuanceDate may not be more than a year from ProcessingDate:'{0}'", e => e.ProcessingDate.ToShortDateString());
            RuleFor(o => o.ScheduledUpdate).Must(NotTooOldDate)
                .WithMessage("ScheduledUpdate may not be prior to 1936");
            RuleFor(o => o.ScheduledUpdate).LessThanOrEqualTo(f => f.ProcessingDate.AddMonths(13))
                .WithMessage("ScheduledUpdate may not be more than 13 months from ProcessingDate:'{0}'", e => e.ProcessingDate.ToShortDateString());
            RuleFor(o => o.CreatedBy).NotEmpty()
                .WithMessage("CreatedBy is required");
            RuleFor(o => o.MaintenanceStatus).NotEqual(default(MaintenanceStatusType))
                .WithMessage("MaintenanceStatus is required");
        }

        /// <summary>
        /// Makes sure a date is not too old
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooOldDate(DateTime date)
        {
            return !(date < new DateTime(1936, 1, 1));
        }

    }
}
