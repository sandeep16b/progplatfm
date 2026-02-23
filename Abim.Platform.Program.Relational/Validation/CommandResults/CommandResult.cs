using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Validation.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Relational.Validation.Impl
{
    /// <summary>
    /// CommandResult Class.
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.ICommandResultWithErrors"/>
    public class CommandResult : ICommandResultWithErrors
    {
        /// <summary>
        /// Gets or sets the status. This can be independent of the Validation's status; the Succeeded property will look at both
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public CommandStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>
        /// The validation.
        /// </value>
        public AbimValidationResult Validation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult"/> class.
        /// </summary>
        public CommandResult()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status"></param>
        /// <param name="validation"></param>
        protected CommandResult(CommandStatus status, AbimValidationResult validation)
        {
            Status = status;
            Validation = validation;
        }

        #region Error Messages

        /// <summary>
        /// Gets the custom error messages.
        /// </summary>
        /// <value>
        /// The custom error messages.
        /// </value>
        protected List<string> CustomErrorMessages { get; private set; } = new List<string>();

        /// <summary>
        /// Sets the custom error messages.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void SetCustomErrorMessages(List<string> list)
        {
            if (list == null) throw new ArgumentNullException("Cannot set CustomErrorMessages to null");
            CustomErrorMessages = list;
        }

        /// <summary>
        /// Remove the custom error messages.
        /// </summary>
        public void RemoveCustomErrorMessages()
        {
            CustomErrorMessages.Clear();
        }

        /// <summary>
        /// The full list of error messages, including custom ones and automatically-read-in validation ones.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<string> Messages
        {
            get
            {
                //sometimes we specifically don't want to show or log the original validation message because it's either misleading or a security concern if
                //... presented to the user
                if (CustomErrorMessages.Any() && (!PersistValidationMessages || !ValidationMessages.Any()))
                    return CustomErrorMessages;
                else if (!CustomErrorMessages.Any())
                    return ValidationMessages;
                else return CustomErrorMessages.Union(ValidationMessages).ToList();
            }
        }

        /// <summary>
        /// Gets just the validation messages.
        /// </summary>
        /// <value>
        /// The validation messages.
        /// </value>
        public List<string> ValidationMessages
        {
            get
            {
                if (Validation == null || Validation.Results == null || Validation.Results.All(o => o == null))
                    return new List<string>();
                return Validation.Results.Where(o => o != null).Select(e => e.Message).ToList();
            }
        }

        /// <summary>
        /// Adds a custom error message.
        /// </summary>
        /// <param name="text">The text.</param>
        public void AddErrorMessage(string text)
        {
            if (text == null) return;
            if (text.Trim().Length == 0)
                text = "(empty error)";
            if (!CustomErrorMessages.Contains(text))
                CustomErrorMessages.Add(text);
        }

        /// <summary>
        /// internal backing field
        /// </summary>
        internal bool _persistValidationMessages = false;

        /// <summary>
        /// Keeps the validation messages as custom error messages (unions them, effectually), instead of replacing them with the custom error messages
        /// </summary>
        /// <returns></returns>
        public bool PersistValidationMessages
        {
            get
            {
                return _persistValidationMessages;
            }
            set
            {
                _persistValidationMessages = value;
            }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether this instance has additional information.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has additional information; otherwise, <c>false</c>.
        /// </value>
        public bool HasAdditionalInfo
        {
            get
            {
                return InfoMessages.Any();
            }
        }

        /// <summary>
        /// Gets the information messages.
        /// </summary>
        /// <value>
        /// The information messages.
        /// </value>
        protected List<string> InfoMessages { get; private set; } = new List<string>();

        /// <summary>
        /// Sets the info messages.
        /// </summary>
        /// <param name="list">The list.</param>
        internal void SetInfoMessages(List<string> list)
        {
            if (list == null) throw new ArgumentNullException("Cannot set InfoMessages to null");
            InfoMessages = list;
        }

        /// <summary>
        /// Adds the information message.
        /// </summary>
        /// <param name="text">The text.</param>
        public void AddInfoMessage(string text)
        {
            InfoMessages.Add(text);
        }

        /// <summary>
        /// Gets a single info message string
        /// </summary>
        /// <value>
        /// The get information message.
        /// </value>
        public string InfoMessage
        {
            get
            {
                return string.Join("; ", InfoMessages.ToArray());
            }
        }

        /// <summary>
        /// If you want your CommandResult child class to return Succeeded = false on some other condition besides the ones coded in the "Succeeded"
        /// property here, you can override this method
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has other failure condition]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool HasOtherFailureCondition()
        {
            return false;
        }

        /// <summary>
        /// Returns whether the validation failed, if the Validation was set. If it wasn't set, returns false
        /// </summary>
        /// <returns></returns>
        public bool ValidationFailed
        {
            get
            {
                if (Validation != null && (!Validation.Succeeded || ValidationMessages.Any()))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Determines whether the CommandResult has an error condition other than its CommandStatus.
        /// </summary>
        /// <returns>
        ///   <c>true</c> or <c>false</c>.
        /// </returns>
        public bool HasErrorConditionOtherThanCommandStatus
        {
            get
            {
                if (CustomErrorMessages.Any()) return true;
                if (ValidationFailed) return true;
                if (HasOtherFailureCondition()) return true;
                return false;
            }
        }

        /// <summary>
        /// The Succeeded property looks at multiple things in order to ensure that this CommandResult is truly successful. If Succeeded is false
        /// and the caller wishes to know why it is false, the FailureReason will indicate that
        /// </summary>
        /// <value>
        /// The failure reason.
        /// </value>
        public CommandResultFailureReasonType FailureReason
        {
            get
            {
                if (Status != CommandStatus.Accepted) return CommandResultFailureReasonType.StatusNotAccepted;
                if (CustomErrorMessages.Any()) return CommandResultFailureReasonType.ErrorMessagePresent;
                if (ValidationFailed) return CommandResultFailureReasonType.ValidationFailure;
                if (HasOtherFailureCondition()) return CommandResultFailureReasonType.HasOtherFailureCondition;
                return CommandResultFailureReasonType.DidNotFail;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CommandResult"/> has succeeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if succeeded; otherwise, <c>false</c>.
        /// </value>
        public bool Succeeded
        {
            get
            {
                if (Status != CommandStatus.Accepted) return false;
                if (HasErrorConditionOtherThanCommandStatus) return false;
                return true;
            }
        }

        /// <summary>
        /// for chaining error messages
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public CommandResult WithErrorMessage(string text)
        {
            CustomErrorMessages.Add(text);
            return this;
        }

        /// <summary>
        /// Gets a single message string.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get
            {
                return string.Join("; ", Messages.ToArray());
            }
        }

        /// <summary>
        /// Gets a single validation message string.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string ValidationMessage
        {
            get
            {
                if (Validation == null) return null;
                return Validation.Message;
            }
        }
    }

    /// <summary>
    /// CommandResult Class.
    /// </summary>
    public class CommandResult<TData> : CommandResult, ICommandResult<TData>
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public TData Data { get; set; }

        /// <summary>
        /// Gets or sets an already-known correlation identifier.
        /// </summary>
        /// <value>
        /// The known correlation identifier.
        /// </value>
        protected internal Guid _knownCorrelationId { get; set; }

        /// <summary>
        /// Gets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        public Guid CorrelationId
        {
            get
            {
                if (_knownCorrelationId != Guid.Empty) return _knownCorrelationId;
                if (Data == null) return Guid.Empty;
                if (Data is IAggregateRoot) return ((IAggregateRoot)Data).ExternalId;
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult{TData}"/> class.
        /// </summary>
        public CommandResult()
        {
        }

        /// <summary>
        /// Gets or sets the object resulting from the Command.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        TData ICommandResult<TData>.Object
        {
            get { return Data; }
            set { Data = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResult{TData}"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="validation">The validation.</param>
        /// <param name="data">The data.</param>
        public CommandResult(CommandStatus status, AbimValidationResult validation, TData data) : base(status, validation)
        {
            Data = data;
        }

        /// <summary>
        /// Copies this instance. Not a deep copy (shares lists, etc)
        /// </summary>
        /// <returns></returns>
        public CommandResult<TData> Copy()
        {
            var copy = new CommandResult<TData>()
            {
                Status = Status,
                Validation = Validation,
                _persistValidationMessages = _persistValidationMessages,
                Data = Data,
                _knownCorrelationId = CorrelationId
            };
            copy.SetCustomErrorMessages(CustomErrorMessages);
            copy.SetInfoMessages(InfoMessages);
            return copy;
        }

        /// <summary>
        /// Generic overload for Copy()
        /// </summary>
        /// <returns></returns>
        public TCommandResult CopyResult<TCommandResult>()
            where TCommandResult : CommandResult<TData>, new()
        {
            var copy = new TCommandResult()
            {
                Status = Status,
                Validation = Validation,
                _persistValidationMessages = _persistValidationMessages,
                Data = Data,
                _knownCorrelationId = CorrelationId
            };
            copy.SetCustomErrorMessages(CustomErrorMessages);
            copy.SetInfoMessages(InfoMessages);
            return copy;
        }

        /// <summary>
        /// Removes the Data and returns the current instance. For use in serialization because the Data is often an NHibernate object which may
        /// encounter JsonSerialization errors over RabbitMQ
        /// </summary>
        /// <returns></returns>
        public CommandResult<TData> WithoutData()
        {
            if (_knownCorrelationId == Guid.Empty && CorrelationId != Guid.Empty)
                _knownCorrelationId = CorrelationId;
            Data = default(TData);
            return this;
        }
    }
}
