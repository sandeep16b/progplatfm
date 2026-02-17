using System;
using FluentValidation;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// CreateCredentialIssuanceCommandValidator Class.
    /// </summary>
    public class CreateCredentialIssuanceCommandValidator : AbstractValidator<CreateCredentialIssuanceCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCredentialIssuanceCommandValidator"/> class.
        /// </summary>
        public CreateCredentialIssuanceCommandValidator()
        {
            RuleFor(o => o.CertificationId).NotEqual(Guid.Empty)
                .WithMessage("CertificationId is required");
            RuleFor(o => o.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            //TODO: these are sample lines; need to confirm with business what the real date ranges should be
            //RuleFor(o => o.GracePeriodStartDate).Must(NotTooOldDate)
            //    .WithMessage("GracePeriodStartDate may not be more than 4 years ago");
            //RuleFor(o => o.GracePeriodStartDate).Must(NotTooFutureDate)
            //    .WithMessage("GracePeriodStartDate may not be more than a year in the future");
            //RuleFor(o => o.GracePeriodEndDate).Must(NotTooOldDate)
            //    .WithMessage("GracePeriodEndDate may not be more than 4 years ago");
            //RuleFor(o => o.GracePeriodEndDate).Must(NotTooFutureDate)
            //    .WithMessage("GracePeriodEndDate may not be more than a year in the future");
            //RuleFor(o => o.ExamDueDate).Must(NotTooOldDate)
            //    .WithMessage("ExamDueDate may not be more than 4 years ago");
            //RuleFor(o => o.ExamDueDate).Must(NotTooFutureDate)
            //    .WithMessage("ExamDueDate may not be more than a year in the future");

            //the default value, 0, is not a valid value for these enums
            RuleFor(o => o.Type).NotEqual(default(CredentialType))
                .WithMessage("Type is required");
            RuleFor(o => o.Pathway).NotEqual(default(PathwayType))
                .WithMessage("Pathway is required");
        }

        /// <summary>
        /// Makes sure a date is not too old
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooOldDate(DateTime? date)
        {
            if (date == null) return true;
            if (date.Value >= DateTime.Now) return true;
            return (DateTime.Now - date.Value).TotalDays < (365 * 4);
        }

        /// <summary>
        /// Makes sure a date is not too far in the future
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooFutureDate(DateTime? date)
        {
            if (date == null) return true;
            if (date.Value < DateTime.Now) return true;
            return (date.Value - DateTime.Now).TotalDays < 365;
        }
    }
}
