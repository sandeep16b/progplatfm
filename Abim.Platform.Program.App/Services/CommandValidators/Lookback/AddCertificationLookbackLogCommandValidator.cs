using Abim.Platform.Program.App.Services.Commands;
using FluentValidation;

namespace Abim.Platform.Program.App.Services.CommandValidators.Lookback
{
    /// <summary>
    /// 
    /// </summary>
    public class AddCertificationLookbackLogCommandValidator :
        AbstractValidator<AddCertificationLookbackLog>
    {
        /// <summary>
        /// 
        /// </summary>
        public AddCertificationLookbackLogCommandValidator()
        {
            RuleFor(o => o.CredentialId).NotEmpty().WithMessage("CredentialId is required");
            RuleFor(o => o.UserName).NotNull().NotEmpty().WithMessage("UserName is required");
        }
    }
}
