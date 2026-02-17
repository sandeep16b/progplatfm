using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using FluentValidation;


namespace Abim.Platform.Program.App.Services.RuleValidators
{
    /// <summary>
    /// Validators a credential
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Credential}" />
    public class CorrectiveActionMBMCertStatusValidator : AbstractValidator<Credential>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectiveActionMBMCertStatusValidator"/> class.
        /// </summary>
        public CorrectiveActionMBMCertStatusValidator()
        {
            //Requirement: The credential is issued by ABIM as the source (you can check the source of the most recent issuance)
            RuleFor(o => o.NewestIssuance.Source.Code).Equal("ABIM")
               .WithMessage("Credential '{0}' is NOT issued by ABIM", e => e.Id);
            //Requirement: [P004] [C026] The credential is must-be-maintained (eg there is at least one historic issuance which has the maintenancerequired=1, 
            // most recent issued should have it if any exist)
            RuleFor(o => o.IsMBM).Equal(true)
                .WithMessage("Credential '{0}' is NOT must-be-maintained", e => e.Id);
            //Requirement: The certificate status is Inactive or Expired (eg. no issuance which is active/ revoked/surrendered/suspended)
            RuleFor(o => o.NewestIssuance.IssuanceStatus == IssuanceStatusType.Expired).Equal(true)
                .WithMessage("Most recent issuance is NOT Expired for Credential '{0}'", e => e.Id);

        }

    }
}
