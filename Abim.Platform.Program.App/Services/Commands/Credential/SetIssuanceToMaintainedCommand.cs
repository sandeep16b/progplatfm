using Abim.Platform.Program.Relational.Services;
using Embarr.WebAPI.AntiXss;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to set an Issuance to be 'Maintained'
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class SetIssuanceToMaintainedCommand : ICommand
    {
        /// <summary>
        /// Id of the Credential
        /// </summary>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The audit username
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string ModifiedBy { get; set; }

        ///// <summary>
        ///// Gets or sets the user information.
        ///// </summary>
        ///// <value>
        ///// The user information.
        ///// </value>
        //[JsonIgnore]
        //public UserInfo UserInfo { get; set; }
    }
}
