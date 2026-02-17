using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System.Linq;

namespace Abim.Platform.Program.App.Services.RuleValidators
{
    /// <summary>
    /// 
    /// </summary>
    public class CorrectiveActionGrandfatherParticipationStatusValidator : AbstractValidator<Credential>
    {
        /// <summary>
        /// 
        /// </summary>
        public CorrectiveActionGrandfatherParticipationStatusValidator()
        {
            RuleFor(o => o.NewestIssuance.Source.Code).Equal("ABIM")
               .WithMessage("Credential '{0}' is NOT issued by ABIM",e=>e.Id);
            RuleFor(o => o.IsGrandfather).Equal(true)
                .WithMessage("Credential '{0}' is NOT grandfather", e => e.Id);
            //Requirement: The lifetime issuance is Active
            RuleFor(o => o.Issuances.Where(t=> t.IssuanceStatus== IssuanceStatusType.Active 
                                            && t.Occurrence==OccurrenceType.Initial).Any()).Equal(true)
                .WithMessage("The Lifetime (initial) issuance is NOT Active for Credential '{0}'", e => e.Id);
        }

    }
}
