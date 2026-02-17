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
        SoThat = "it can handle the ExpireAndReissueCommand"
        )]
    [TestFixture]
    public class ExpireAndReissueCommandSpec
    {
        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsAcceptedOnSuccess()
        {
            new ExpireAndReissueCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new ExpireAndReissueCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsExceptionWhenDatabaseSaveThrowsException()
        {
            new ExpireAndReissueCommandHandleReturnsExceptionWhenDatabaseSaveThrowsException().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException()
        {
            new ExpireAndReissueCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73194)]
        public void ExpireAndReissueCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException()
        {
            new ExpireAndReissueCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException().BDDfy();
        }
    }
    
    /// <summary>
    /// Command builder for the ExpireAndReissueCommand class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.App.Services.Commands.ExpireAndReissueCommand" />
    public class UnitTestExpireAndReissueCommandBuilder
    {
        public CommandBuilder<ExpireAndReissueCommand> ValidCommand()
        {
            var command = CommandBuilder<ExpireAndReissueCommand>.Valid();
            /*Add any additional setup here for specific property requirements*/
            return command;
        }
        
        public CommandBuilder<ExpireAndReissueCommand> InvalidCommand()
        {
            var command = CommandBuilder<ExpireAndReissueCommand>.Invalid();
            return command;
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class ExpireAndReissueCommandServiceScenario : CredentialServiceScenario
    {
        protected UnitTestExpireAndReissueCommandBuilder CommandBuilder = new UnitTestExpireAndReissueCommandBuilder();
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
            types.Add(typeof(IValidator<ExpireAndReissueCommand>));
            types.Add(typeof(IHelperService));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class ExpireAndReissueCommandHandleReturnsAcceptedOnSuccess
        : ExpireAndReissueCommandServiceScenario
    {
        
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));

            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);

            My<ICredentialRepository>()
                .Setup(o => o.UpdateCredential(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });

            CredentialService = Container.GetInstance<CredentialService>();

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
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
    public class ExpireAndReissueCommandHandleReturnsRejectedWhenCommandFailsValidation
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        ValidationResult BadValidationResult                     { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(BadValidationResult);
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAnInvalidCommand()
        {
            Command = CommandBuilder.InvalidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
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
    public class ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        new string ExceptionText                                     { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);

            My<ICredentialRepository>()
                 .Setup(o => o.UpdateCredential(It.IsAny<Credential>(),It.IsAny<string>()))
                 .Returns(new AbimValidationResult()
                 {
                     Succeeded = false,
                     Results = new List<IValidationResult>()
                                    {
                                        new ValidationErrorResult()
                                        {
                                            Message =  "."
                                        }
                                    }
                 });

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
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
            CommandResult.Validation.Succeeded.Should().BeFalse();
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
    public class ExpireAndReissueCommandHandleReturnsExceptionWhenDatabaseSaveThrowsException
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        ExpireAndReissueCommand Command { get; set; }
        ExpireAndReissueCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential ExistingCredential { get; set; }

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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);

            My<ICredentialRepository>()
                 .Setup(o => o.UpdateCredential(It.IsAny<Credential>(), It.IsAny<string>()))
                 .Returns(new AbimValidationResult()
                 {
                     Succeeded = false,
                     Results = new List<IValidationResult>()
                                    {
                                        new ValidationErrorResult()
                                        {
                                            Message =  "Violation of UNIQUE KEY constraint 'NK_Issuance'. Cannot insert duplicate key in object 'dbo.Issuance'. "
                                        }
                                    }
                 });

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                        {
                            Username = RandomString.Build(),
                            AbimId = Random.Next(1000, 100000).ToString()
                        })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ExpireAndReissueCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().NotBeNull();
        }

        public void ThenExceptionShouldMentionTheDatabase()
        {
            ExceptionCaught.Message.Should().Contain("Stop further execution");
        }

        public void AndThenTheCommandResultShouldNotBeNull()
        {
            CommandResult.Should().BeNull();
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
    public class ExpireAndReissueCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        ValidationResult BadValidationResult                     { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);
            My<ICredentialRepository>()
                .Setup(o => o.UpdateCredential(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
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
    public class ExpireAndReissueCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        new string ExceptionText                                     { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>()))
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
                CommandResult = (ExpireAndReissueCommandResult)(CredentialService.Handle(Command));
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
    /// The SourceService GetAbimSource throws exception scenario
    /// </summary>
    public class ExpireAndReissueCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException
        : ExpireAndReissueCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ExpireAndReissueCommand Command               { get; set; }
        ExpireAndReissueCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        new string ExceptionText                                     { get; set; }
        Credential ExistingCredential                            { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ExpireAndReissueCommand>())
                .Returns(My<IValidator<ExpireAndReissueCommand>>().Object);
            My<IValidator<ExpireAndReissueCommand>>()
                .Setup(o => o.Validate(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ValidationResult());
            ExistingCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            ExistingCredential.AddIssuance(Issuance.Create(RandomString.Build()));
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingCredential);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder.ValidCommand()
                        .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = RandomString.Build(),
                                AbimId = Random.Next(1000, 100000).ToString()
                            })
                        //.With(cmd => cmd.ExistingIssuanceId = ExistingCredential.Issuances.First().Id)
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
    
    #endregion
}
