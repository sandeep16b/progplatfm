using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Class AddSubspecialtyCertificationCommand
    /// </summary>
    public class AddSubspecialtyCertificationCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the base identifier.
        /// </summary>
        /// <value>
        /// The base identifier.
        /// </value>
        public Guid BaseId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the consecutive attempt.
        /// </summary>
        /// <value>
        /// The consecutive attempt.
        /// </value>
        public int? ConsecutiveAttempt { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        #endregion
    }
}
