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
using Abim.Platform.Program.Tests.Scenarios.Services.CertificationService.Base;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using MassTransit;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Registration.Tests.Scenarios.Services.Registration
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CertificationService",
        SoThat = "it can handle the AddPrimaryCertificationCommand"
        )]
    [TestFixture]
    public class AddPrimaryCertificationCommandSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void AddPrimaryCertificationCommandHandleReturnsAcceptedOnSuccess()
        {
            new AddPrimaryCertificationCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        public void AddPrimaryCertificationCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new AddPrimaryCertificationCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        public void AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void AddPrimaryCertificationCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException()
        {
            new AddPrimaryCertificationCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException().BDDfy();
        }
        
        [TestCase]
        //[WorkItem(/*TODO: look up work item number*/)]
        public void AddPrimaryCertificationCommandHandleReturnsRejectedWhenDefaultSourceIsNull()
        {
            new AddPrimaryCertificationCommandHandleReturnsRejectedWhenDefaultSourceIsNull().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CertificationServiceScenario" />
    public abstract class AddPrimaryCertificationCommandServiceScenario : CertificationServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ICertificationRepository));
            types.Add(typeof(ISession));
            types.Add(typeof(IQueryFactory));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(ISourceService));
            types.Add(typeof(ISourceRepository));
            types.Add(typeof(IBusControl));
            types.Add(typeof(IBackgroundJobClient));
            types.Add(typeof(IValidationFactory));
            types.Add(typeof(IValidator<AddPrimaryCertificationCommand>));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class AddPrimaryCertificationCommandHandleReturnsAcceptedOnSuccess
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(new ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
    public class AddPrimaryCertificationCommandHandleReturnsRejectedWhenCommandFailsValidation
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        ValidationResult BadValidationResult                 { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(BadValidationResult);
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAnInvalidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Invalid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
    public class AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        new string ExceptionText                                 { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(new ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
    public class AddPrimaryCertificationCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        ValidationResult BadValidationResult                 { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(new ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
    /// The SourceService GetAbimSource throws exception scenario
    /// </summary>
    public class AddPrimaryCertificationCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        new string ExceptionText                                 { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(new ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Throws(new Exception(ExceptionText));
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
    /// The defaultSource is null scenario
    /// </summary>
    public class AddPrimaryCertificationCommandHandleReturnsRejectedWhenDefaultSourceIsNull
        : AddPrimaryCertificationCommandServiceScenario
    {
        ICertificationService CertificationService           { get; set; }
        
        AddPrimaryCertificationCommand Command               { get; set; }
        AddPrimaryCertificationCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                    { get; set; }
        new EmailBuilder EmailBuilder                            { get; set; }
        new DateTimeBuilder DateTimeBuilder                      { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        
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
            CertificationService = Container.GetInstance<CertificationService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddPrimaryCertificationCommand>())
                .Returns(My<IValidator<AddPrimaryCertificationCommand>>().Object);
            My<IValidator<AddPrimaryCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddPrimaryCertificationCommand>()))
                .Returns(new ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>()
                , RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                ;//.Setup(o => o.GetAbimSource(/*TODO: fill in It.IsAny() parameters*/))
                //.Returns(/*TODO: fill in invalid return value*/);
            //Place any other setups here
            ((CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
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
                CommandResult = (AddPrimaryCertificationCommandResult)(CertificationService.Handle(Command));
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
