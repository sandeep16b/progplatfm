using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using Embarr.WebAPI.AntiXss;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// UpdateCredentialFromLookbackCommand
    /// </summary>
    public class UpdateCredentialFromLookbackCommand : ICommand
    {
        //This command just takes the credential, which has already been modified during the lookback process, 
        //to persist.
        /// <summary>
        /// The Credential
        /// </summary>
        public Credential Credential { get; set; }

        /// <summary>
        /// The audit username
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string ModifiedBy { get; set; }
    }
}
