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
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Authentication;
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
        SoThat = "it can handle the ExpireIssuanceCommand"
        )]
    [TestFixture]
    public class ExpireIssuanceCommandSpec
    {
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsAcceptedOnSuccess()
        {
            new ExpireIssuanceCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceDoesntExist()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceDoesntExist().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenCredentialIsNull()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenCredentialIsNull().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceCannotExpire()
        {
            new ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceCannotExpire().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class ExpireIssuanceCommandServiceScenario : CredentialServiceScenario
    {
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
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidator<ExpireIssuanceCommand>));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class ExpireIssuanceCommandHandleReturnsAcceptedOnSuccess
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
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
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenCommandFailsValidation
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        ValidationResult BadValidationResult        { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(BadValidationResult);
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
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
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Invalid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        new string ExceptionText { get; set; }

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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
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
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireAndReissueCommandResult)(CredentialService.Handle(Command));
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
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        ValidationResult BadValidationResult        { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
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
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireAndReissueCommandResult)(CredentialService.Handle(Command));
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
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        new string ExceptionText                        { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
            existing.AddIssuance(issuance);
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
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
    /// The invalid IssuanceId scenario
    /// </summary>
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceDoesntExist
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        int IssuanceId                              { get; set; }
        int InvalidIssuanceId                       { get; set; }
        new string ExceptionText                        { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(), DateTimeBuilder.Random().Build(),
                RandomString.Build());
            issuance.Duration = DurationType.Timelimited;
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            IssuanceId = issuance.Id;
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputACommandWithAnInvalidIssuanceId()
        {
            InvalidIssuanceId = Random.Next(1, int.MaxValue);
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .With(cmd => cmd.IssuanceId = InvalidIssuanceId)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
        
        public void AndThenTheCommandResultShouldContainTheError()
        {
            var expected = ErrorMessages.NotFound("Issuance", InvalidIssuanceId);
            CommandResult.Message.Should().Contain(expected);
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
    /// The credential is null scenario
    /// </summary>
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenCredentialIsNull
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        Guid CredentialId                           { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            CredentialId = Guid.NewGuid();
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
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns((Credential)null);
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.CredentialId = CredentialId)
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
        
        public void AndThenTheCommandResultShouldContainTheError()
        {
            var expected = ErrorMessages.NotFound("Credential", CredentialId);
            CommandResult.Message.Should().Contain(expected);
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
    /// The credential cannot expire scenario
    /// </summary>
    public class ExpireIssuanceCommandHandleReturnsRejectedWhenIssuanceCannotExpire
        : ExpireIssuanceCommandServiceScenario
    {
        ICredentialService CredentialService        { get; set; }
        
        ExpireIssuanceCommand Command               { get; set; }
        ExpireIssuanceCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                           { get; set; }
        new EmailBuilder EmailBuilder                   { get; set; }
        new DateTimeBuilder DateTimeBuilder             { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        Guid CredentialId                           { get; set; }
        int IssuanceId                              { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            CredentialId = Guid.NewGuid();
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
            
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
                ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.ExpirationDate = DateTimeBuilder.Random().Build();
            if(RandomBool.IsTrue)
                issuance.Duration = RandomBool.IsTrue ? DurationType.Continuous : DurationType.Lifetime;
            else
                issuance.ExpirationDate = null;
            IssuanceId = issuance.Id;
            existing.AddIssuance(issuance);
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<ExpireIssuanceCommand>())
                .Returns(My<IValidator<ExpireIssuanceCommand>>().Object);
            My<IValidator<ExpireIssuanceCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireIssuanceCommand>()))
                .Returns(new ValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.CredentialId = CredentialId)
                        .With(cmd => cmd.IssuanceId = IssuanceId)
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireIssuanceCommandResult)(CredentialService.Handle(Command));
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
        
        public void AndThenTheCommandResultShouldContainTheError()
        {
            CommandResult.Message.Should().Contain("expire");
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
