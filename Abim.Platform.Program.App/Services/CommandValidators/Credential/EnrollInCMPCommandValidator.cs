using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;
using System.Text.RegularExpressions;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// The Validation class for the EnrollInCMPCommand
    /// </summary>
    public class EnrollInCMPCommandValidator : AbstractValidator<EnrollInCMPCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnrollInCMPCommand"/> class.
        /// </summary>
        public EnrollInCMPCommandValidator()
        {
            // MemberId / AbimId
            RuleFor(o => o.MemberId).NotEqual(Guid.Empty).When(x => string.IsNullOrEmpty(x.AbimId))
                .WithMessage("MemberId or AbimId is required");

            // EnrollmentDate
            RuleFor(o => o.EnrollmentDate).Must(NotTooOldDate)
                .WithMessage("EnrollmentDate may not be prior to 1936");

            RuleFor(o => o.EnrollmentDate).Must(NotTooFutureDate)
                .WithMessage("EnrollmentDate may not be more than 3 years in the future");

            //RequestingUserName
            RuleFor(x => x.RequestingUserName).NotEmpty();

            // SubspecialtyCertCode
            RuleFor(x => x.SubspecialtyCertCode)
                .NotEmpty().WithMessage("Subspecialty Code is required.")
                .Must(IsValidSubspecialtyCertCode)
                .WithMessage("Subspecialty Code must be between 4 and 5 characters.");

            // UserInfo
            RuleFor(x => x.UserInfo).NotNull()
                .WithMessage("UserInfo must not be null");

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

        /// <summary>
        /// Makes sure a date is not too far in the future
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        private bool NotTooFutureDate(DateTime date)
        {
            if (date < DateTime.Now) return true;
            return (date - DateTime.Now).Days < 365*3;
        }

        /// <summary>
        /// IsValidSpecialtyCode
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsValidSubspecialtyCertCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;
            else
            {
                var pattern = new Regex("^[a-zA-Z0-9]{4,5}$");
                return pattern.IsMatch(code);
            }
        }
    }

}
