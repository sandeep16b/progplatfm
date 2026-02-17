namespace Abim.Platform.Program.Relational.Validation.CommandResults
{
    public enum CommandResultFailureReasonType
    {
        /// <summary>
        /// Default value for this enum
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The Status property of the CommandResult was not equal to CommandStatus.Accepted
        /// </summary>
        StatusNotAccepted = 1,

        /// <summary>
        /// The CustomErrorMessages property of the CommandResult was not empty
        /// </summary>
        ErrorMessagePresent = 2,

        /// <summary>
        /// The Validation property of the CommandResult had a false Succeeded property
        /// </summary>
        ValidationFailure = 3,

        /// <summary>
        /// CommandResult.HasOtherFailureCondition() returned true
        /// </summary>
        HasOtherFailureCondition = 4,

        /// <summary>
        /// For sake of completion, this enum value represents a state in which the CommandResult is in a "Succeeded = true" state
        /// </summary>
        DidNotFail = 5
    }
}
