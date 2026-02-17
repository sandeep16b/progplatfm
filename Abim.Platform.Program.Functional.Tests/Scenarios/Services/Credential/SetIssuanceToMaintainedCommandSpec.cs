using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Registration.Tests.Scenarios.Services.Registration
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the SetIssuanceToMaintainedCommand"
        )]
    [TestFixture]
    public class SetIssuanceToMaintainedCommandSpec
    {
        [TestCase]
        [WorkItem(73199)]
        public void SetIssuanceToMaintainedCommandHandleReturnsAcceptedOnSuccess()
        {
            new SetIssuanceToMaintainedCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73199)]
        
        public void SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73199)]
        public void SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73199)]
        public void SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73199)]
        public void SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException()
        {
            new SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException().BDDfy();
        }
    }
    
    /// <summary>
    /// Command builder for the SetIssuanceToMaintainedCommand class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.App.Services.Commands.SetIssuanceToMaintainedCommand" />
    public class UnitTestSetIssuanceToMaintainedCommandBuilder
    {
        public CommandBuilder<SetIssuanceToMaintainedCommand> ValidCommand()
        {
            var command = CommandBuilder<SetIssuanceToMaintainedCommand>.Valid();
            /*Add any additional setup here for specific property requirements*/
            return command;
        }
        
        public CommandBuilder<SetIssuanceToMaintainedCommand> InvalidCommand()
        {
            var command = CommandBuilder<SetIssuanceToMaintainedCommand>.Invalid();
            return command;
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class SetIssuanceToMaintainedCommandServiceScenario : CredentialServiceScenario
    {
        protected UnitTestSetIssuanceToMaintainedCommandBuilder CommandBuilder = new UnitTestSetIssuanceToMaintainedCommandBuilder();
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ICredentialRepository));
            types.Add(typeof(ISession));
            types.Add(typeof(IQueryFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(ICertificationService));
            types.Add(typeof(ICertificationRepository));
            types.Add(typeof(ISourceService));
            types.Add(typeof(ISourceRepository));
            types.Add(typeof(IBusControl));
            types.Add(typeof(IBackgroundJobClient));
            types.Add(typeof(IValidator<SetIssuanceToMaintainedCommand>));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsAcceptedOnSuccess
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            My<IHelperService>()
               .Setup(o => o.TriggeredCommunication(It.IsAny<Credential>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ICredentialService>()))
               .Returns(new Task(() => { }));

            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.MaintenanceRequirement = MaintenanceRequirementType.Required;
            issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            issuance.IssuanceStatus = IssuanceStatusType.Active;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }
        
        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().NotBeNull();
        }
        
        public void AndThenTheCommandResultStatusShouldBeAccepted()
        {
            CommandResult.Status.Should().Be(CommandStatus.Accepted);
        }
        
        public void AndThenTheCommandResultDotSucceededShouldBeTrue()
        {
            CommandResult.Succeeded.Should().BeTrue();
        }
        
        public void AndThenTheCommandResultValidationShouldShowSuccess()
        {
            CommandResult.Validation.Succeeded.Should().BeTrue();
        }
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    /// <summary>
    /// The Command fails validation scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenCommandFailsValidation
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        ValidationResult BadValidationResult                       { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            
            var errors = new List<ValidationFailure>();
            for(int i = 0; i < Random.Next(1, 100); i++)
                errors.Add(new ValidationFailure(RandomString.Build(), RandomString.Build()));
            BadValidationResult = new ValidationResult(errors);
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            My<IHelperService>()
               .Setup(o => o.TriggeredCommunication(It.IsAny<Credential>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ICredentialService>()))
               .Returns(new Task(() => { }));

            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(BadValidationResult);
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.MaintenanceRequirement = MaintenanceRequirementType.Required;
            issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            issuance.IssuanceStatus = IssuanceStatusType.Active;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAnInvalidCommand()
        {
            Command = CommandBuilder.InvalidCommand()
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }
        
        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().NotBeNull();
        }
        
        public void AndThenTheCommandResultStatusShouldBeRejected()
        {
            CommandResult.Status.Should().Be(CommandStatus.Rejected);
        }
        
        public void AndThenTheCommandResultDotSucceededShouldBeFalse()
        {
            CommandResult.Succeeded.Should().BeFalse();
        }
        
        public void AndThenTheCommandResultValidationShouldShowFailure()
        {
            CommandResult.Validation.Succeeded.Should().BeFalse();
        }
        
        //public void AndThenTheCommandResultShouldContainTheValidationError()
        //{
        //    CommandResult.Message.Should().Contain(BadValidationResult.ToAbimValidationResult().Message);
        //}
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    /// <summary>
    /// The Database save throws exception scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        new string ExceptionText                                       { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            My<IHelperService>()
               .Setup(o => o.TriggeredCommunication(It.IsAny<Credential>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ICredentialService>()))
               .Returns(new Task(() => { }));

            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.MaintenanceRequirement = MaintenanceRequirementType.Required;
            issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            issuance.IssuanceStatus = IssuanceStatusType.Active;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .Build();
        }
                
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }
        
        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().NotBeNull();
        }
        
        public void AndThenTheCommandResultStatusShouldBeRejected()
        {
            CommandResult.Status.Should().Be(CommandStatus.Rejected);
        }
        
        public void AndThenTheCommandResultDotSucceededShouldBeFalse()
        {
            CommandResult.Succeeded.Should().BeFalse();
        }
        
        public void AndThenTheCommandResultValidationShouldShowSuccess()
        {
            CommandResult.Validation.Succeeded.Should().BeTrue();
        }
        
        public void AndThenTheCommandResultMessageShouldMentionTheDatabase()
        {
            CommandResult.Message.ToLower().Should().Contain("database");
        }
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    /// <summary>
    /// The Database save fails domain validation scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        ValidationResult BadValidationResult                       { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            
            var errors = new List<ValidationFailure>();
            for(int i = 0; i < Random.Next(1, 100); i++)
                errors.Add(new ValidationFailure(RandomString.Build(), RandomString.Build()));
            BadValidationResult = new ValidationResult(errors);
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            My<IHelperService>()
               .Setup(o => o.TriggeredCommunication(It.IsAny<Credential>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ICredentialService>()))
               .Returns(new Task(() => { }));

            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.MaintenanceRequirement = MaintenanceRequirementType.Required;
            issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            issuance.IssuanceStatus = IssuanceStatusType.Active;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .Build();
        }
                
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }
        
        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().NotBeNull();
        }
        
        public void AndThenTheCommandResultStatusShouldBeRejected()
        {
            CommandResult.Status.Should().Be(CommandStatus.Rejected);
        }
        
        public void AndThenTheCommandResultDotSucceededShouldBeFalse()
        {
            CommandResult.Succeeded.Should().BeFalse();
        }
        
        public void AndThenTheCommandResultValidationShouldShowFailure()
        {
            CommandResult.Validation.Succeeded.Should().BeFalse();
        }
        
        public void AndThenTheCommandResultMessageShouldIncludeTheObjectValidationMessage()
        {
            CommandResult.Message.Should().Contain(BadValidationResult.ToAbimValidationResult().Message);
        }
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    /// <summary>
    /// The Repository Load throws exception scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        new string ExceptionText                                       { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new ValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .Build();
        }
                
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenAnExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().NotBeNull();
        }
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    /// <summary>
    /// The Credential not requiring corrective action scenario
    /// </summary>
    public class SetIssuanceToMaintainedCommandHandleReturnsRejectedWhenTheCredentialDoesntRequireCorrectiveAction
        : SetIssuanceToMaintainedCommandServiceScenario
    {
        ICredentialService CredentialService                       { get; set; }
        
        SetIssuanceToMaintainedCommand Command                     { get; set; }
        SetIssuanceToMaintainedCommandResult CommandResult         { get; set; }
        Mock<ILogger> Log                                          { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        new string ExceptionText                                       { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<SetIssuanceToMaintainedCommand>())
                .Returns(My<IValidator<SetIssuanceToMaintainedCommand>>().Object);
            My<IValidator<SetIssuanceToMaintainedCommand>>()
                .Setup(o => o.Validate(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.MaintenanceRequirement = MaintenanceRequirementType.NotRequired;
            issuance.MaintenanceStatus = MaintenanceStatusType.Maintained;
            issuance.IssuanceStatus = IssuanceStatusType.Active;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (SetIssuanceToMaintainedCommandResult)(CredentialService.Handle(Command));
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }
        
        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }
        
        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().NotBeNull();
        }
        
        public void AndThenTheCommandResultStatusShouldBeRejected()
        {
            CommandResult.Status.Should().Be(CommandStatus.Rejected);
        }
        
        public void AndThenTheCommandResultDotSucceededShouldBeFalse()
        {
            CommandResult.Succeeded.Should().BeFalse();
        }
        
        public void AndThenTheCommandResultValidationShouldShowSuccess()
        {
            CommandResult.Validation.Succeeded.Should().BeTrue();
        }
        
        public void AndThenTheCommandResultMessageShouldMentionTheCorrectiveAction()
        {
            CommandResult.Message.ToLower().Should().Contain("corrective");
        }
        
        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                log.Debugs.Any(s => s.Contains("Command Args for")));
        }
        
        public void AndThenThereShouldBeACommandDebugSomewhere()
        {
            LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
        }
        
        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
        }
    }
    
    #endregion
}
