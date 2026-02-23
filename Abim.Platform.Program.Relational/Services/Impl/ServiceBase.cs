using System;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using MassTransit;
using NLog;
using System.Collections.Generic;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Repository;
using Hangfire;

namespace Abim.Platform.Program.Relational.Services.Impl
{
    /// <summary>
    /// ServiceBase Class.
    /// </summary>
    public abstract class ServiceBase<TAggregateRoot, TRepository> : IService<TAggregateRoot>, IDisposable
        where TAggregateRoot : class, IAggregateRoot, IEntity<int>
        where TRepository : IRepository<TAggregateRoot, int>
    {
        #region Fields
        
        /// <summary>
        /// The logger
        /// </summary>
        protected ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The validation logger
        /// </summary>
        protected ILogger ValidationLogger = LogManager.GetLogger("ABIM-LogRhythm-Validation");

        /// <summary>
        /// The domain validation failure message
        /// </summary>
        public const string DomainValidationFailureMessage = "Domain validation failed";

        /// <summary>
        /// The failed to update prefix
        /// </summary>
        public const string FailedToUpdatePrefix = "Failed to update";

        /// <summary>
        /// The save failure message
        /// </summary>
        public static string SaveFailureMessage = string.Format("{0} the {1} in the database", FailedToUpdatePrefix, typeof(TAggregateRoot).Name);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bus.
        /// </summary>
        /// <value>
        /// The bus.
        /// </value>
        protected IBusControl Bus { get; private set; }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <value>
        /// The repository.
        /// </value>
        protected TRepository Repository { get; private set; }

        /// <summary>
        /// Gets the job client.
        /// </summary>
        /// <value>
        /// The job client.
        /// </value>
        protected IBackgroundJobClient JobClient { get; private set; }
        
        /// <summary>
        /// Gets the validation factory.
        /// </summary>
        /// <value>
        /// The validation factory.
        /// </value>
        protected IValidationFactory ValidationFactory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TAggregateRoot, TRepository}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected ServiceBase(TRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TAggregateRoot, TRepository}"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="repository">The repository.</param>
        protected ServiceBase(IBusControl bus, TRepository repository)
        {
            Bus = bus;
            Repository = repository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TAggregateRoot, TRepository}"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="repository">The repository.</param>
        /// <param name="validationFactory">The validation factory.</param>
        protected ServiceBase(IBusControl bus, TRepository repository, IValidationFactory validationFactory)
        {
            Bus = bus;
            Repository = repository;
            ValidationFactory = validationFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TAggregateRoot, TRepository}"/> class.
        /// </summary>
        /// <param name="bus">The bus.</param>
        /// <param name="repository">The repo.</param>
        /// <param name="jobClient">The job client.</param>
        /// <param name="validationFactory">The validation factory.</param>
        protected ServiceBase(IBusControl bus, TRepository repository, IBackgroundJobClient jobClient, IValidationFactory validationFactory)
        {
            Bus = bus;
            Repository = repository;
            JobClient = jobClient;
            ValidationFactory = validationFactory;
        }

        #endregion

        /// <summary>
        /// Validate method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        protected AbimValidationResult Validate<T>(ICommand command)
        {
            if(command == null)
                throw new Exception("Cannot validate a null command");
            Logger.Debug(command);
            
            if(!(command is T))
            {
                var msg = string.Format("Cannot call Validate<T>(ICommand command) on a command which is not of type T (T is {0})", typeof(T).Name);
                throw new Exception(msg);
            }
            var instance = ValidationFactory.GetValidatorInstance<T>();
            var validationResult = instance.Validate((T)command);
            var result = validationResult.ToAbimValidationResult();
            bool isValidationValid = validationResult.IsValid;
            if(isValidationValid == false)
            {
                ValidationLogger.Warn(result);
            }
            Logger.Debug(result);
            return result;
        }

        /// <summary>
        /// Check whether an object exists by its Id
        /// </summary>
        /// <param name="id">The Certification identifier</param>
        /// <returns></returns>
        public virtual bool Exists(int id)
        {
            return Repository.Exists(id);
        }

        /// <summary>
        /// Check whether an object exists by its Id
        /// </summary>
        /// <param name="id">The Certification identifier</param>
        /// <returns></returns>
        public virtual bool Exists(Guid id)
        {
            return Repository.Exists(id);
        }

        /// <summary>
        /// Gets a given object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TAggregateRoot Load(Guid id)
        {
            return Repository.Load(id);
        }

        /// <summary>
        /// Gets a given object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TAggregateRoot Load(int id)
        {
            return Repository.Load(id);
        }

        /// <summary>
        /// Gets all objects
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Search()
        {
            return Repository.GetAll();
        }

        /// <summary>
        /// Searches aggregate roots by a query
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Search(ComplexQueryBase query, out int totalCount)
        {
            return Repository.Query(query, out totalCount);
        }
        
        /// <summary>
        /// Searches aggregate roots by a paging query
        /// </summary>
        /// <param name="paging">The paging</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Search(PageDefinition paging, out int totalCount)
        {
            return Repository.Query(paging, out totalCount);
        }

        #region Error Handling

        /// <summary>
        /// Sometimes an error occurs which the code can pick up but which isn't handled by validation. In that case can call this
        /// to get an ICommandResult to return
        /// </summary>
        /// <remarks>
        /// PotentiallyObsolete (would add that attribute, but it's defined in WebApi)
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected ICommandResult ErrorResult<TCommandResult>(string message)
            where TCommandResult : CommandResult, new()
        {
            return new TCommandResult()
            {
                Status = CommandStatus.Rejected,
                Validation = new AbimValidationResult()
                {
                    Succeeded = false,
                    Results = new List<IValidationResult>()
                    {
                        new ValidationErrorResult()
                        {
                            Message = message
                        }
                    }
                }
            };
        }
        
        /// <summary>
        /// Logs an error, and returns a failed command result with the given error message.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="cmdValidation">The validation result.</param>
        /// <param name="setterAction">An action to perform on the CommandResult after creating it.</param>
        /// <returns></returns>
        protected TCommandResult Error<TCommandResult>(string message, AbimValidationResult cmdValidation, Action<TCommandResult> setterAction = null)
            where TCommandResult : CommandResult, new()
        {
            var result = new TCommandResult();
            result.Status = CommandStatus.Rejected;
            result.Validation = cmdValidation;
            result.AddErrorMessage(message);
            Logger.Error(message + ". Stack Trace: " + Environment.StackTrace);
            if(setterAction != null) setterAction(result);
            return result;
        }

        /// <summary>
        /// Logs an error, and returns a failed command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="ex">The exception.</param>
        /// <param name="cmdValidation">The validation result.</param>
        /// <param name="setterAction">An action to perform on the CommandResult after creating it.</param>
        /// <returns></returns>
        protected TCommandResult Error<TCommandResult>(Exception ex, AbimValidationResult cmdValidation, Action<TCommandResult> setterAction = null)
            where TCommandResult : CommandResult, new()
        {
            var result = new TCommandResult();
            result.Status = CommandStatus.Rejected;
            result.Validation = cmdValidation;
            result.AddErrorMessage(ex.Message);
            Logger.Error(ex);
            if(setterAction != null) setterAction(result);
            return result;
        }

        /// <summary>
        /// Logs a warning, and returns a failed command result with the given error message.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="cmdValidation">The validation result.</param>
        /// <param name="setterAction">An action to perform on the CommandResult after creating it.</param>
        /// <returns></returns>
        protected TCommandResult Warning<TCommandResult>(string message, AbimValidationResult cmdValidation, Action<TCommandResult> setterAction = null)
            where TCommandResult : CommandResult, new()
        {
            var result = new TCommandResult();
            result.Status = CommandStatus.Rejected;
            result.Validation = cmdValidation;
            result.AddErrorMessage(message);
            Logger.Warn(message + ". Stack Trace: " + Environment.StackTrace);
            if(setterAction != null) setterAction(result);
            return result;
        }

        /// <summary>
        /// Logs a warning, and returns a failed command result. Usually, since this takes in an Exception, you should
        /// call Error<TCommandResult>() instead. However this method might be useful for ArgumentExceptions, etc
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="ex">The exception.</param>
        /// <param name="cmdValidation">The validation result.</param>
        /// <param name="setterAction">An action to perform on the CommandResult after creating it.</param>
        /// <returns></returns>
        protected TCommandResult Warning<TCommandResult>(Exception ex, AbimValidationResult cmdValidation, Action<TCommandResult> setterAction = null)
            where TCommandResult : CommandResult, new()
        {
            var result = new TCommandResult();
            result.Status = CommandStatus.Rejected;
            result.Validation = cmdValidation;
            result.AddErrorMessage(ex.Message);
            Logger.Warn(ex);
            if(setterAction != null) setterAction(result);
            return result;
        }

        /// <summary>
        /// Logs an error, adds it to an existing command result, and return false.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="ex">The exception.</param>
        /// <param name="failureResult">The failure result.</param>
        /// <returns></returns>
        public bool ErrorAndReturnFalse<TCommandResult>(Exception ex, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(ex.Message);
            Logger.Error(ex);
            return false;
        }

        /// <summary>
        /// Logs a warning, adds it to an existing command result, and return false.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="failureResult">The failure result.</param>
        /// <returns></returns>
        public bool WarnAndReturnFalse<TCommandResult>(string message, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(message);
            Logger.Warn(message + ". Stack Trace: " + Environment.StackTrace);
            return false;
        }

        /// <summary>
        /// Logs an error and adds it to an existing command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="ex">The exception.</param>
        /// <param name="failureResult">The failure result.</param>
        public void ErrorAndContinue<TCommandResult>(Exception ex, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(ex.Message);
            Logger.Error(ex);
        }

        /// <summary>
        /// Logs a warning and adds it to an existing command result. Typically, you wouldn't use this method,
        /// you'd use ErrorAndContinue, since there was an Exception. However it might be useful for
        /// ArgumentExceptions, etc
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="ex">The exception.</param>
        /// <param name="failureResult">The failure result.</param>
        public void WarnAndContinue<TCommandResult>(Exception ex, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(ex.Message);
            Logger.Warn(ex);
        }

        /// <summary>
        /// Logs an error and adds it to an existing command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="failureResult">The failure result.</param>
        public void ErrorAndContinue<TCommandResult>(string message, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(message);
            Logger.Error(message + ". Stack Trace: " + Environment.StackTrace);
        }

        /// <summary>
        /// Logs a warning and adds it to an existing command result.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="failureResult">The failure result.</param>
        public void WarnAndContinue<TCommandResult>(string message, TCommandResult failureResult)
            where TCommandResult : CommandResult, new()
        {
            if(failureResult != null) failureResult.AddErrorMessage(message);
            Logger.Warn(message + ". Stack Trace: " + Environment.StackTrace);
        }

        #endregion

        #region Repository Saves

        /// <summary>
        /// Adds the aggregateRoot. Returns an error command result if an error occurred, otherwise returns null.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <param name="username">The username.</param>
        /// <param name="persistSpecificValidationErrors">A setting on whether to include what object validation errors existed, in the friendly message.</param>
        /// <returns></returns>
        protected TCommandResult TryAdd<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation, string username,
                bool persistSpecificValidationErrors = false)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            AbimValidationResult objValidation;
            try
            {
                if(Repository.CommitEachCallInItsOwnTransaction)
                    objValidation = Repository.Add(aggregateRoot, username);
                else
                {
                    Repository.BeginTransaction();
                    objValidation = Repository.Add(aggregateRoot, username);
                    if(objValidation.Succeeded)
                        Repository.CommitTransaction();
                    else
                        Repository.RollbackTransaction();
                }
            }
            catch(Exception ex)
            {
                var failureResult = new TCommandResult();
                failureResult.Status = CommandStatus.Rejected;
                failureResult.Validation = cmdValidation;
                failureResult.AddErrorMessage(SaveFailureMessage);
                if(ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE KEY"))
                    failureResult.AddErrorMessage("Failed uniqueness check");
                if(ex.Message != null) failureResult.AddErrorMessage(ex.Message);
                Logger.Error(ex);
                Logger.Error(string.Format("Exception during Add {0}, at {1}", ex.Message, Environment.StackTrace));
                return failureResult;
            }
            if(!objValidation.Succeeded)
            {
                var failureResult = objValidation.ToCommandResult<TCommandResult, TAggregateRoot>(aggregateRoot);
                var msg = string.Format("Domain validation failed: {0}", objValidation.Message);
                if(persistSpecificValidationErrors) failureResult.PersistValidationMessages = true;
                Logger.Warn(msg + ". Stack Trace: " + Environment.StackTrace);
                failureResult.AddErrorMessage(msg);
                return failureResult;
            }

            return null;
        }

        /// <summary>
        /// Adds the aggregateRoot in the database and returns a command result (the difference between this and TryAdd is that this one always
        /// returns a command result, while TryAdd only returns an error one if an error occurred)
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <param name="username">The username.</param>
        /// <param name="persistSpecificValidationErrors">A setting on whether to include what object validation errors existed, in the friendly message.</param>
        /// <returns></returns>
        protected TCommandResult Add<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation, string username,
                bool persistSpecificValidationErrors = false)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            var tryAddResult = TryAdd<TCommandResult>(aggregateRoot, cmdValidation, username, persistSpecificValidationErrors);
            if(tryAddResult != null) return tryAddResult;
            
            var successResult = cmdValidation.ToCommandResult<TCommandResult, TAggregateRoot>(aggregateRoot);
            return successResult;
        }

        /// <summary>
        /// Updates the aggregateRoot. Returns an error command result if an error occurred, otherwise returns null.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <param name="username">The username.</param>
        /// <param name="persistSpecificValidationErrors">A setting on whether to include what object validation errors existed, in the friendly message.</param>
        /// <returns></returns>
        protected TCommandResult TryUpdate<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation, string username,
                bool persistSpecificValidationErrors = false)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            AbimValidationResult objValidation;
            try
            {
                if(Repository.CommitEachCallInItsOwnTransaction)
                    objValidation = Repository.Update(aggregateRoot, username);
                else
                {
                    Repository.BeginTransaction();
                    objValidation = Repository.Update(aggregateRoot, username);
                    if(objValidation.Succeeded)
                        Repository.CommitTransaction();
                    else
                        Repository.RollbackTransaction();
                }
            }
            catch(Exception ex)
            {
                var failureResult = new TCommandResult();
                failureResult.Status = CommandStatus.Rejected;
                failureResult.Validation = cmdValidation;
                failureResult.AddErrorMessage(SaveFailureMessage);
                if(ex.InnerException != null && ex.InnerException.Message.Contains("UNIQUE KEY"))
                    failureResult.AddErrorMessage("Failed uniqueness check");
                if(ex.Message != null) failureResult.AddErrorMessage(ex.Message);
                Logger.Error(ex);
                Logger.Error(string.Format("Exception during Update {0}, at {1}", ex.Message, Environment.StackTrace));
                return failureResult;
            }
            if(!objValidation.Succeeded)
            {
                var failureResult = objValidation.ToCommandResult<TCommandResult, TAggregateRoot>(aggregateRoot);
                var msg = string.Format("{0}: {1}", DomainValidationFailureMessage, objValidation.Message);
                if(persistSpecificValidationErrors) failureResult.PersistValidationMessages = true;
                Logger.Warn(msg + ". Stack Trace: " + Environment.StackTrace);
                failureResult.AddErrorMessage(msg);
                return failureResult;
            }

            return null;
        }

        /// <summary>
        /// Updates the aggregateRoot in the database and returns a command result (the difference between this and TryUpdate is that this one always
        /// returns a command result, while TryUpdate only returns an error one if an error occurred)
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <param name="username">The username.</param>
        /// <param name="persistSpecificValidationErrors">A setting on whether to include what object validation errors existed, in the friendly message.</param>
        /// <returns></returns>
        protected TCommandResult Update<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation, string username,
                bool persistSpecificValidationErrors = false)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            var tryUpdateResult = TryUpdate<TCommandResult>(aggregateRoot, cmdValidation, username, persistSpecificValidationErrors);
            if(tryUpdateResult != null) return tryUpdateResult;
            
            var successResult = cmdValidation.ToCommandResult<TCommandResult, TAggregateRoot>(aggregateRoot);
            return successResult;
        }

        /// <summary>
        /// Deletes the aggregateRoot. Returns an error command result if an error occurred, otherwise returns null.
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <returns></returns>
        protected TCommandResult TryDelete<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            try
            {
                if(Repository.CommitEachCallInItsOwnTransaction)
                    Repository.Delete(aggregateRoot);
                else
                {
                    Repository.BeginTransaction();
                    Repository.Delete(aggregateRoot);
                    Repository.CommitTransaction();
                }
            }
            catch(Exception ex)
            {
                var failureResult = new TCommandResult();
                failureResult.Status = CommandStatus.Rejected;
                failureResult.Validation = cmdValidation;
                failureResult.AddErrorMessage(SaveFailureMessage);
                if(ex.Message != null) failureResult.AddErrorMessage(ex.Message);
                Logger.Error(ex);
                return failureResult;
            }

            return null;
        }

        /// <summary>
        /// Deletes the aggregateRoot in the database and returns a command result (the difference between this and TryDelete is that this one always
        /// returns a command result, while TryDelete only returns an error one if an error occurred)
        /// </summary>
        /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
        /// <param name="aggregateRoot">The aggregateRoot.</param>
        /// <param name="cmdValidation">The command validation.</param>
        /// <returns></returns>
        protected TCommandResult Delete<TCommandResult>(TAggregateRoot aggregateRoot, AbimValidationResult cmdValidation)
            where TCommandResult : CommandResult<TAggregateRoot>, new()
        {
            var tryDeleteResult = TryDelete<TCommandResult>(aggregateRoot, cmdValidation);
            if(tryDeleteResult != null) return tryDeleteResult;
            
            var successResult = cmdValidation.ToCommandResult<TCommandResult, TAggregateRoot>(aggregateRoot);
            return successResult;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
        /// </remarks>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
        /// </remarks>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(IsDisposed) return;
            
            //Free managed resources
            if(disposing)
            {
                Repository.Dispose();
                DisposeChildServices();
            }
            
            //Free any unmanaged resources here
            
            IsDisposed = true;
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="ServiceBase{TAggregateRoot, TRepository}"/> class.
        /// </summary>
        ~ServiceBase()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Disposes the child services. Use this to call dispose on any child services your service might have.
        /// </summary>
        protected virtual void DisposeChildServices()
        {
        }

        #endregion
    }
}
