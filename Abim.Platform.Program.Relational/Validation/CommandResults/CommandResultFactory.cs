namespace Abim.Platform.Program.Relational.Validation.Impl
{
    /// <summary>
    /// ResultExtensions Class.
    /// </summary>
    public static class CommandResultFactory
    {
        /// <summary>
        /// Maps onto a new command result and returns it.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static TCommandResult ToCommandResult<TCommandResult>(this AbimValidationResult result)
            where TCommandResult : CommandResult, new()
        {
            return new TCommandResult
            {
                Status = result.Succeeded ? CommandStatus.Accepted : CommandStatus.Rejected,
                Validation = result
            };
        }

        /// <summary>
        /// Maps onto a new command result and returns it.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static TCommandResult ToCommandResult<TCommandResult, TData>(this AbimValidationResult result, TData data)
            where TCommandResult : CommandResult<TData>, new()
        {
            return new TCommandResult
            {
                Status = result.Succeeded ? CommandStatus.Accepted : CommandStatus.Rejected,
                Validation = result,
                Data = data
            };
        }

        /// <summary>
        /// Maps onto an existing command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static TCommandResult ToCommandResult<TCommandResult>(this AbimValidationResult result, TCommandResult value)
            where TCommandResult : CommandResult, new()
        {
           if(value == null)
                return null;
            
           if(value.Status == CommandStatus.Accepted)
                value.Status = result.Succeeded ? CommandStatus.Accepted : CommandStatus.Rejected;
           value.Validation = result;
            
           return value;
        }

        /// <summary>
        /// Maps onto an existing command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <typeparam name="TData">The type of the data.</typeparam>
        /// <param name="result">The result.</param>
        /// <param name="value">The value.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static TCommandResult ToCommandResult<TCommandResult, TData>(this AbimValidationResult result, TCommandResult value, TData data)
            where TCommandResult : CommandResult<TData>, new()
        {
           if(value == null)
                return null;
            
            if(value.Status == CommandStatus.Accepted)
                value.Status = result.Succeeded ? CommandStatus.Accepted : CommandStatus.Rejected;
            value.Validation = result;
            value.Data = data;
            
            return value;
        }
    }
}
