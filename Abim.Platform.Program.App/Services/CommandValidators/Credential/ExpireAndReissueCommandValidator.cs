using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators
{
    /// <summary>
    /// ExpireAndReissueCommandValidator Class.
    /// </summary>
    public class ExpireAndReissueCommandValidator : AbstractValidator<ExpireAndReissueCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpireAndReissueCommandValidator"/> class.
        /// </summary>
        public ExpireAndReissueCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEqual(Guid.Empty)
                .WithMessage("CredentialId is required");
            RuleFor(o => o.IssuanceDate).Must(NotTooOldDate)
                .WithMessage("IssuanceDate may not be prior to 1936");
            RuleFor(o => o.IssuanceDate).Must(NotTooFutureDate)
                .WithMessage("IssuanceDate may not be more than a year in the future");
            RuleFor(o => o.ScheduledUpdate).Must(NotTooOldDate)
                .WithMessage("ScheduledUpdate may not be prior to 1936");
            //ar@8/24/2017 commented below: if run process with current date as 3/1/2018 (per Don's test cases) we would end up with 2019-04-01 which would
            // break this conditions
            //RuleFor(o => o.ScheduledUpdate).Must(NotTooFutureDate)
            //    .WithMessage("ScheduledUpdate may not be more than a year in the future");
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

        /// <summary>
        /// Makes sure a date is not too far in the future
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooFutureDate(DateTime date)
        {
            if(date < DateTime.Now) return true;
            return (date - DateTime.Now).TotalDays < 365;
        }
    }
}
