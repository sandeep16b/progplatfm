using Abim.Enterprise.Core.Interservice;
using Abim.Enterprise.Core.Relational;
using Abim.Enterprise.Core.Relational.Classes;
using Abim.Enterprise.Core.Relational.Validation;
using Abim.Enterprise.Core.Relational.Validation.Impl;
using Abim.Enterprise.Core.Resource.Profile;
using Abim.Enterprise.Core.Resource.Program;
using Abim.Enterprise.Core.Resource.Registration;
using Abim.Enterprise.Core.Resource.Product;
using Abim.Enterprise.Core.WebApi;
using Abim.Enterprise.Core.WebApi.Authentication;
using Abim.Enterprise.Core.WebApi.Exceptions;
using Abim.Enterprise.Core.WebApi.Objects;
using Abim.Enterprise.Core.WebApi.Objects.Util;
using Abim.Enterprise.Core.WebApi.Response;
using Abim.Enterprise.Core.WebApi.Testing.Setup;
using Abim.Enterprise.Core.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesService.Base;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Abim.Enterprise.Core.Relational.Queries;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NLog;
using Hangfire;
using NUnit.Framework;
using NHibernate;
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
    ///<remarks>
    ///TODO: Note that this test class is NOT done. It is only stubbed out with very basic tests. Please add more, and then remove this note
    ///</remarks>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the ProgramRulesService",
        SoThat = "it can handle the RunRulesOnExpiredTimeLimitedCredentialCommand"
        )]
    [TestFixture]
    public class RunRulesOnExpiredTimeLimitedCredentialCommandSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsAcceptedWhenSuccess()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsAcceptedWhenSuccess().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceLoadThrowsException()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceLoadThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceHandleThrowsException()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceHandleThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenAcceptedResultAddInfoMessageThrowsException()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenAcceptedResultAddInfoMessageThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenNewIssuanceItemNotNullConditionFailure()
        {
            new RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenNewIssuanceItemNotNullConditionFailure().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.ProgramRulesServiceScenario" />
    public abstract class RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario : ProgramRulesServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ICertificationService));
            types.Add(typeof(ICertificationRepository));
            types.Add(typeof(ISession));
            types.Add(typeof(IQueryFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(ISourceService));
            types.Add(typeof(ISourceRepository));
            types.Add(typeof(IBusControl));
            types.Add(typeof(IBackgroundJobClient));
            types.Add(typeof(ICredentialService));
            types.Add(typeof(ICredentialRepository));
            types.Add(typeof(IProductInterservice));
            types.Add(typeof(IRegistrationInterservice));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsAcceptedWhenSuccess
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        ValidationResult BadValidationResult                                { get; set; }
        
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
            BadValidationResult = new FluentValidation.Results.ValidationResult(errors);
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(BadValidationResult);
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAnInvalidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Invalid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
        
        public void AndThenTheCommandResultShouldContainTheValidationError()
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
    /// The Database save throws exception scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        string ExceptionText                                                { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        ValidationResult BadValidationResult                                { get; set; }
        
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
            BadValidationResult = new FluentValidation.Results.ValidationResult(errors);
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    /// The CredentialService Load throws exception scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceLoadThrowsException
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        string ExceptionText                                                { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Throws(new Exception(ExceptionText));
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    /// The CredentialService Handle throws exception scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenCredentialServiceHandleThrowsException
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        string ExceptionText                                                { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialService>()
                ;//.Setup(o => o.Handle(/*TODO: fill in It.IsAny() parameters*/))
                //.Throws(new Exception(ExceptionText));
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    /// The acceptedResult AddInfoMessage throws exception scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenAcceptedResultAddInfoMessageThrowsException
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        string ExceptionText                                                { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            //TODO: Mock acceptedResult.AddInfoMessage to throw an exception
            //Place any other setups here
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
    /// The newIssuance Item not null condition failure scenario
    /// </summary>
    public class RunRulesOnExpiredTimeLimitedCredentialCommandHandleReturnsRejectedWhenNewIssuanceItemNotNullConditionFailure
        : RunRulesOnExpiredTimeLimitedCredentialCommandServiceScenario
    {
        IProgramRulesService ProgramRulesService                            { get; set; }
        
        RunRulesOnExpiredTimeLimitedCredentialCommand Command               { get; set; }
        RunRulesOnExpiredTimeLimitedCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                                   { get; set; }
        EmailBuilder EmailBuilder                                           { get; set; }
        DateTimeBuilder DateTimeBuilder                                     { get; set; }
        Exception ExceptionCaught                                           { get; set; }
        
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
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<RunRulesOnExpiredTimeLimitedCredentialCommand>())
                .Returns(My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>().Object);
            My<IValidator<RunRulesOnExpiredTimeLimitedCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<object>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Abim.Platform.Program.App.Domain.ProgramRules existing = default(ProgramRules);
            My<IProgramRulesRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<IProgramRulesRepository>()
                .Setup(o => o.Update(It.IsAny<Abim.Platform.Program.App.Domain.ProgramRules>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            //TODO: Mock newIssuance.Item1 != null failure
            //Place any other setups here
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<RunRulesOnExpiredTimeLimitedCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        .Build();
        }
        
        public async Task WhenICallHandle()
        {
            try
            {
                CommandResult = (RunRulesOnExpiredTimeLimitedCredentialCommandResult)(ProgramRulesService.Handle(Command));
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
        
        //TODO: Need to add specific Then checks
        
        public void AndThenTheMessageShouldBeCorrect()
        {
            CommandResult.Message.Should().Contain("");
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
