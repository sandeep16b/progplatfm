using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Services.CommandValidators.Credential
{
    /// <summary>
    /// UpdateCredentialFromLookbackCommandValidator
    /// </summary>
    public class UpdateCredentialFromLookbackCommandValidator : AbstractValidator<UpdateCredentialFromLookbackCommand>
    {
        /// <summary>
        /// UpdateCredentialFromLookbackCommandValidator
        /// </summary>
        public UpdateCredentialFromLookbackCommandValidator()
        {
            RuleFor(o => o.Credential).NotNull()
               .WithMessage("Credential is required.");

            RuleFor(o => o.ModifiedBy).NotEmpty()
                .WithMessage("ModifiedBy is required.");

            //Are these more suited to being on the CredentialValidator?
            RuleFor(o => o.Credential.GracePeriodStartDate).NotNull().When(o => o.Credential != null && o.Credential.GracePeriodEndDate.HasValue)
                .WithMessage("GracePeriodStart is required when GracePeriodEnd is present.");

            RuleFor(o => o.Credential.GracePeriodEndDate).NotNull().When(o => o.Credential != null && o.Credential.GracePeriodStartDate.HasValue)
                .WithMessage("GracePeriodEnd is required when GracePeriodStart is present.");

            RuleFor(o => o.Credential.GracePeriodStartDate).LessThan(o => o.Credential.GracePeriodEndDate).When(o => o.Credential != null && o.Credential.GracePeriodEndDate.HasValue)
                .WithMessage("GracePeriodStart must be less than GracePeriodEnd.");

            RuleFor(o => o.Credential.NewestIssuance.IssuanceStatus).NotEmpty().When(o => o.Credential?.NewestIssuance != null)
                .WithMessage("Credential.NewestIssuance.IssuanceStatus is required.");

            RuleFor(o => o.Credential.NewestIssuance.MaintenanceStatus).NotEmpty().When(o => o.Credential?.NewestIssuance != null)
                .WithMessage("Credential.NewestIssuance.MaintenanceStatus is required.");

            //RuleFor(o => o.Credential.LookbackDate).NotEmpty().When(o => o.Credential != null)
            //The line above should have been fine, but the validation didn't fail when expected, probably 
            //because LookbackDate is nullable. Commented out line is left to illustrate why the 
            //approach below was used.
            RuleFor(o => o.Credential.LookbackDate).NotEqual(new DateTime()).When(o => o.Credential != null)
                .WithMessage("LookbackDate must be specified.");
        }
    }
}
