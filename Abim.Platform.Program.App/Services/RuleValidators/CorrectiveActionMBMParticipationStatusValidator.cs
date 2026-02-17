using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using FluentValidation;

namespace Abim.Platform.Program.App.Services.RuleValidators
{
    /// <summary>
    /// Validates a credential
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Credential}" />
    public class CorrectiveActionMBMParticipationStatusValidator : AbstractValidator<Credential>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectiveActionMBMParticipationStatusValidator"/> class.
        /// </summary>
        public CorrectiveActionMBMParticipationStatusValidator()
        {
            //Requirement: The credential is issued by ABIM as the source (you can check the source of the most recent issuance)
            RuleFor(o => o.NewestIssuance.Source.Code).Equal("ABIM")
               .WithMessage("Credential '{0}' is NOT issued by ABIM", e => e.Id);
            // Requirement: The credential is not clinical laboratory immunology, diagnostic immunology or allergy and immunology.
            RuleFor(o => o.Certification.Code == "CLI" // Clinical and Laboratory Immunology
                       || o.Certification.Code == "DLI"   //Diagnostic and Laboratory Immunology
                       || o.Certification.Code == "ALLG") //Allergy and Immunology
                        .Equal(false)
                        .WithMessage("Credential '{0}' IS clinical laboratory immunology, diagnostic immunology or allergy and immunology", e => e.Id);
            //Requirement: [P004] [C026] The credential is must-be-maintained (eg there is at least one historic issuance which has the maintenancerequired=1, 
            // most recent issued should have it if any exist)
            RuleFor(o => o.IsMBM).Equal(true)
                .WithMessage("Credential '{0}' is NOT must-be-maintained", e => e.Id);
            RuleFor(o => o.NewestIssuance.IssuanceStatus == IssuanceStatusType.Active).Equal(true)
                .WithMessage("Most recent issuance is NOT active for Credential '{0}'", e => e.Id);

        }

    }
}
