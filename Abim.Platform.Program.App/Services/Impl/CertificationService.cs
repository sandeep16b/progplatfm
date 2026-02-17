using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System.Linq;
using System.Threading.Tasks;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class CertificationService
    /// </summary>
    public class CertificationService : ServiceBase<Certification, ICertificationRepository>, ICertificationService
    {
        #region Properties

        /// <summary>
        /// The logger is inherited from the base class. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }

        /// <summary>
        /// The Credential service property-backer
        /// </summary>
        protected ICredentialService _credentialService;
        /// <summary>
        /// The Credential service
        /// </summary>
        /// <remarks>
        /// Cannot be dependency-injected because that would cause a circular reference
        /// </remarks>
        protected ICredentialService CredentialService
        {
            get
            {
                if (_credentialService == null)
                    _credentialService = DependencyResolver.Container.GetInstance<ICredentialService>();
                return _credentialService;
            }
        }

        /// <summary>
        /// The Source service
        /// </summary>
        protected ISourceService SourceService { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationService"/> class.
        /// </summary>
        /// <param name="certificationRepository">The certification repository.</param>
        /// <param name="sourceService">The source service.</param>
        /// <param name="bus">The bus.</param>
        /// <param name="jobClient">The job client.</param>
        /// <param name="validationFactory">The validation factory.</param>
        public CertificationService(ICertificationRepository certificationRepository,
                                    ISourceService sourceService,
                                    IBusControl bus,
                                    IBackgroundJobClient jobClient,
                                    IValidationFactory validationFactory)
            : base(bus, certificationRepository, jobClient, validationFactory)
        {
            SourceService = sourceService;
        }



        #region Custom Get Methods

        /// <summary>
        /// 
        /// </summary>
        public Certification GetByCode(string Code)
        {
            return Repository.Query(o => o.Code == Code).SingleOrDefault();
        }

        #endregion

        #region Command Handlers

        #region AddPrimaryCertificationCommand

        /// <summary>
        /// Handles an AddPrimaryCertificationCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(AddPrimaryCertificationCommand command)
        {
            Log.Trace("Started Handle for AddPrimaryCertificationCommand");
            Log.Debug("Command Args for AddPrimaryCertificationCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddPrimaryCertificationCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddPrimaryCertificationCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //fetch the default Source
                var defaultSource = SourceService.GetAbimSource();

                if (defaultSource == null) return Warning<AddPrimaryCertificationCommandResult>($"Default {typeof(Source).Name} of code 'ABIM' not found", cmdValidation);

                //create the model
                string createdBy = command.UserInfo.Username;
                var certification = Certification.Create(null, defaultSource, CertificationType.Primary, command.Name, command.Code, createdBy);

                //apply field settings
                certification.SetConsecutiveAttempt(command.ConsecutiveAttempt);

                //add to the database
                return Add<AddPrimaryCertificationCommandResult>(certification, cmdValidation, createdBy);

            }
            finally
            {
                Log.Trace("Returning from Handle for AddPrimaryCertificationCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<AddPrimaryCertificationCommand>.Validate(AddPrimaryCertificationCommand command)
        {
            return base.Validate<AddPrimaryCertificationCommand>(command);
        }

        #endregion

        #region AddAddedQualificationCertificationCommand

        /// <summary>
        /// Handles a AddAddedQualificationCertificationCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(AddAddedQualificationCertificationCommand command)
        {
            Log.Trace("Started Handle for AddAddedQualificationCertificationCommand");
            Log.Debug("Command Args for AddAddedQualificationCertificationCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddAddedQualificationCertificationCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddAddedQualificationCertificationCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //fetch the default Source
                var defaultSource = SourceService.GetAbimSource();

                if (defaultSource == null) return Warning<AddPrimaryCertificationCommandResult>($"Default {typeof(Source).Name} of code 'ABIM' not found", cmdValidation);

                //fetch the base Certification
                var baseCert = Repository.Load(command.BaseId);

                if (baseCert == null) return Warning<AddPrimaryCertificationCommandResult>(ErrorMessages.NotFound("Certification", command.BaseId), cmdValidation);

                //create the model
                var certification = Certification.Create(baseCert, defaultSource, CertificationType.Subspecialty, RandomString.Build(), RandomString.Build(), command.UserInfo.Username);

                //apply field settings
                certification.SetConsecutiveAttempt(command.ConsecutiveAttempt);

                //add to the database
                return Add<AddAddedQualificationCertificationCommandResult>(certification, cmdValidation, command.UserInfo.Username);

            }
            finally
            {
                Log.Trace("Returning from Handle for AddAddedQualificationCertificationCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<AddAddedQualificationCertificationCommand>.Validate(AddAddedQualificationCertificationCommand command)
        {
            return base.Validate<AddAddedQualificationCertificationCommand>(command);
        }

        #endregion

        #region AddSubspecialtyCertificationCommand

        /// <summary>
        /// Handles a AddSubspecialtyCertificationCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(AddSubspecialtyCertificationCommand command)
        {
            Log.Trace("Started Handle for AddSubspecialtyCertificationCommand");
            Log.Debug("Command Args for AddSubspecialtyCertificationCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddSubspecialtyCertificationCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddSubspecialtyCertificationCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //fetch the default Source
                var defaultSource = SourceService.GetAbimSource();

                if (defaultSource == null) return Warning<AddSubspecialtyCertificationCommandResult>($"Default {typeof(Source).Name} of code 'ABIM' not found", cmdValidation);

                //fetch the base Certification
                var baseCert = Repository.Load(command.BaseId);

                if (baseCert == null) return Warning<AddSubspecialtyCertificationCommandResult>(ErrorMessages.NotFound("Certification", command.BaseId), cmdValidation);

                //create the model
                var certification = Certification.Create(baseCert, defaultSource, CertificationType.Subspecialty, command.Name, command.Code, command.UserInfo.Username);

                //apply field settings
                certification.SetConsecutiveAttempt(command.ConsecutiveAttempt);

                //add to the database
                return Add<AddSubspecialtyCertificationCommandResult>(certification, cmdValidation, command.UserInfo.Username);          
            }
            finally
            {
                Log.Trace("Returning from Handle for AddSubspecialtyCertificationCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<AddSubspecialtyCertificationCommand>.Validate(AddSubspecialtyCertificationCommand command)
        {
            return base.Validate<AddSubspecialtyCertificationCommand>(command);
        }

        #endregion

        #region ETL processing endpoints

        #region AddCertification

        /// <summary>
        /// Handles an AddCertificationCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<AddCertificationCommandResult> Handle(AddCertificationCommand command)
        {
            Log.Debug("Command Args for AddCertificationCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<AddCertificationCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded)
                return Warning<AddCertificationCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the Source
            var source = SourceService.Load(command.SourceId);

            if (source == null)
                return Warning<AddCertificationCommandResult>(ErrorMessages.NotFound("Source", command.SourceId), cmdValidation);

            // Create the model
            var certification = Certification.Create(null, source, command.Type, command.Name, command.Code, command.UserInfo.Username);

            certification.SetConsecutiveAttempt(command.ConsecutiveAttempt);

            //add to the database
            return Add<AddCertificationCommandResult>(certification, cmdValidation, command.UserInfo.Username);

        }

        /// <summary>
        /// Validate a AddCertificationCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(AddCertificationCommand command)
        {
            return Validate<AddCertificationCommand>(command);
        }

        #endregion

        #region UpdateCertification

        /// <summary>
        /// Handles an UpdateCertificationCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdateCertificationCommandResult> Handle(UpdateCertificationCommand command)
        {
            Log.Debug("Command Args for UpdateCertificationCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<UpdateCertificationCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded) return Warning<UpdateCertificationCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the Source
            var source = SourceService.Load(command.SourceId);

            if (source == null)
                return Warning<UpdateCertificationCommandResult>(ErrorMessages.NotFound("Source", command.SourceId), cmdValidation);

            // Fetch the Certification
            var certification = Repository.GetBySourceIdAndCode(command.SourceId, command.Code);

            if (certification == null)
                return Warning<UpdateCertificationCommandResult>($"No Certification found for SourceId:'{command.SourceId}' and Code:'{command.Code}'", cmdValidation);

            certification.ApplyUpdateCertification(command.Name, command.Type, command.ConsecutiveAttempt);

            //save to the database
            return Update<UpdateCertificationCommandResult>(certification, cmdValidation, command.UserInfo.Username);

        }

        /// <summary>
        /// Validate a UpdateCertificationCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdateCertificationCommand command)
        {
            return Validate<UpdateCertificationCommand>(command);
        }

        #endregion

        #endregion

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the child services.
        /// </summary>
        protected override void DisposeChildServices()
        {
            if (_credentialService != null)
                _credentialService.Dispose();
        }

        #endregion
    }
}
