using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// ReissueCommandValidator Class.
    /// </summary>
    public class ReissueCommandValidator : AbstractValidator<ReissueCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReissueCommandValidator"/> class.
        /// </summary>
        public ReissueCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.IssuanceDate).Must(NotTooOldDate)
                .WithMessage("IssuanceDate may not be prior to 1936");
            RuleFor(o => o.IssuanceDate).LessThanOrEqualTo(f=>f.ProcessingDate.AddYears(1))
                .WithMessage("IssuanceDate may not be more than a year from ProcessingDate:'{0}'",e=>e.ProcessingDate.ToShortDateString());
            RuleFor(o => o.ScheduledUpdate).Must(NotTooOldDate)
                .WithMessage("ScheduledUpdate may not be prior to 1936");
            RuleFor(o => o.ScheduledUpdate).LessThanOrEqualTo(f => f.ProcessingDate.AddMonths(13))
                .WithMessage("ScheduledUpdate may not be more than 13 months from ProcessingDate:'{0}'", e => e.ProcessingDate.ToShortDateString());
            RuleFor(o => o.CreatedBy).NotEmpty()
                .WithMessage("CreatedBy is required");
        }

        /// <summary>
        /// Makes sure a date is not too old
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooOldDate(DateTime date)
        {
            return !(date.Date < new DateTime(1936, 1, 1).Date);
        }
    }
}
