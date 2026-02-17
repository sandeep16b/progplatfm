using Abim.Platform.Program.Relational.Services;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Update LNG Assessment Due Date Command
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateLngAssessmentDueDateCommand : ICommand
    {

        #region Properties

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// IsSummativeDecisionYear
        /// </summary>
        public bool IsSummativeDecisionYear { get; set; }

        /// <summary>
        /// MetParticipationStatus
        /// </summary>
        public bool? MetParticipationStatus { get; set; }

        /// <summary>
        /// PassSummativeDecision
        /// </summary>
        public bool? PassSummativeDecision { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        public string UserName { get; set; }

        #endregion

    }
}