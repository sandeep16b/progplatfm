using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// AddParticipationLookbackLog
    /// </summary>
    public class AddParticipationLookbackLog : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        public Guid CredentialId { get; set; }
        /// <summary>
        /// Reason
        /// </summary>
        public LookbackReasonType Reason { get; set; }
        /// <summary>
        /// Action
        /// </summary>
        public LookbackActionType Action { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// IsPendingAction
        /// </summary>
        public bool IsPendingAction { get; set; }
        /// <summary>
        /// LookbackLogDate
        /// </summary>
        public DateTime LookbackLogDate { get; set; }

        /// <summary>
        /// Returns a string containing the values of all of the command's properties
        /// </summary>
        /// <returns></returns>
        public string Dump()
        {
            return $"CredentialId: {CredentialId}, Reason: {Reason}, Action: {Action}, UserName: {UserName}, IsPendingAction: {IsPendingAction}, LookbackLogDate: {LookbackLogDate}";
        }
    }
}
