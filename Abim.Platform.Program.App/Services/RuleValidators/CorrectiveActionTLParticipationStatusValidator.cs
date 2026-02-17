using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using FluentValidation;


namespace Abim.Platform.Program.App.Services.RuleValidators
{
    /// <summary>
    /// Validates a credential
    /// </summary>
    public class CorrectiveActionTLParticipationStatusValidator : AbstractValidator<Credential>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorrectiveActionTLParticipationStatusValidator"/> class.
        /// </summary>
        public CorrectiveActionTLParticipationStatusValidator()
        {
            //Requirement: The credential is issued by ABIM as the source (you can check the source of the most recent issuance)
            RuleFor(o => o.NewestIssuance.Source.Code).Equal("ABIM")
                .WithMessage("Credential '{0}' is NOT issued by ABIM", e => e.Id);
            //Requirement: [P003] [C001] The credential is time limited 
            RuleFor(o => o.IsTimelimited).Equal(true)
                .WithMessage("Credential '{0}' is NOT time limited", e => e.Id);
            RuleFor(o => o.NewestIssuance.IssuanceStatus == IssuanceStatusType.Active).Equal(true)
                .WithMessage("Most recent issuance is NOT active for Credential '{0}'", e => e.Id);

        }

    }
}
