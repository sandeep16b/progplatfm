extern alias SharedOldServiceBus;

using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
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
using ServiceBus.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        SoThat = "it can handle the ReinstateCredentialCommand"
        )]
    [TestFixture]
    public class ReinstateCredentialCommandSpec
    {
        [TestCase]
        [WorkItem(306060)]
        [WorkItem(335679)]
        public void ReinstateCredentialCommandHandleReturnsAcceptedOnSuccess()
        {
            new ReinstateCredentialCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        [WorkItem(306060)]
        [WorkItem(335679)]

        public void ReinstateCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new ReinstateCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(306060)]
        [WorkItem(335679)]
        public void ReinstateCredentialCommandHandleReturnsWarningDoesNotContainProperIssuanceStatuses()
        {
            new ReinstateCredentialCommandHandleReturnsWarningDoesNotContainProperIssuanceStatuses().BDDfy();
        }
                
        [TestCase]
        [WorkItem(306060)]
        [WorkItem(335679)]
        public void ReinstateCredentialCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException()
        {
            new ReinstateCredentialCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException().BDDfy();
        }
                
    }
    
    /// <summary>
    /// Command builder for the ReinstateCredentialCommand class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.App.Services.Commands.ReinstateCredentialCommand" />
    public class UnitTestReinstateCredentialCommandBuilder
    {
        public CommandBuilder<ReinstateCredentialCommand> ValidCommand()
        {
            var command = CommandBuilder<ReinstateCredentialCommand>.Valid();
            /*Add any additional setup here for specific property requirements*/
            return command;
        }
        
        public CommandBuilder<ReinstateCredentialCommand> InvalidCommand()
        {
            var command = CommandBuilder<ReinstateCredentialCommand>.Invalid();
            return command;
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class ReinstateCredentialCommandServiceScenario : CredentialServiceScenario
    {
        protected UnitTestReinstateCredentialCommandBuilder CommandBuilder = new UnitTestReinstateCredentialCommandBuilder();
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
            types.Add(typeof(IValidator<ReinstateCredentialCommand>));
            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class ReinstateCredentialCommandHandleReturnsAcceptedOnSuccess
        : ReinstateCredentialCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ReinstateCredentialCommand Command               { get; set; }
        ReinstateCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ReinstateCredentialCommand>())
                .Returns(My<IValidator<ReinstateCredentialCommand>>().Object);
            My<IValidator<ReinstateCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<ReinstateCredentialCommand>()))
                .Returns(new ValidationResult());

            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            
            var issuanceTL = Issuance.Create(RandomString.Build());
            issuanceTL.IssuanceStatus = IssuanceStatusType.Inactive;
            issuanceTL.Duration = DurationType.Timelimited;
            issuanceTL.IssuanceDate = DateTime.UtcNow.AddYears(-10);
            existing.AddIssuance(issuanceTL);

            var issuanceMBM = Issuance.Create(RandomString.Build());
            issuanceMBM.IssuanceStatus = IssuanceStatusType.Suspended;
            issuanceMBM.Duration = DurationType.Continuous;
            issuanceMBM.IssuanceDate = DateTime.UtcNow.AddYears(-2);
            existing.AddIssuance(issuanceMBM);

            var issuanceGF1 = Issuance.Create(RandomString.Build());
            issuanceGF1.IssuanceStatus = IssuanceStatusType.Inactive;
            issuanceGF1.Duration = DurationType.Lifetime;
            issuanceGF1.ExpiredDate = DateTime.UtcNow.AddYears(-15);
            issuanceGF1.ExpirationDate = DateTime.UtcNow.AddYears(-15);
            issuanceGF1.IssuanceDate = DateTime.UtcNow.AddYears(-20);
            existing.AddIssuance(issuanceGF1);

            var issuanceGF2 = Issuance.Create(RandomString.Build());
            issuanceGF2.IssuanceStatus = IssuanceStatusType.Surrendered;
            issuanceGF2.Duration = DurationType.Lifetime;
            issuanceGF2.ExpiredDate = DateTime.UtcNow.AddYears(-1);
            issuanceGF2.ExpirationDate = DateTime.UtcNow.AddYears(-1);
            issuanceGF2.IssuanceDate = DateTime.UtcNow.AddYears(-15);

            existing.IsActive = false;

            existing.AddIssuance(issuanceGF2);

            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);

            My<IProgramRulesService>()
                .Setup(mock => mock.RunCorrectiveActionForMember(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TriggeringEvent>(),
                    null))
                .Returns(Task.FromResult(true));

            My<IBusControl>()
                .Setup(mock => mock.Publish<IssuanceChanged>(
                    It.IsAny<IssuanceChanged>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
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
                CommandResult = (ReinstateCredentialCommandResult)CredentialService.Handle(Command).Result;
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

        public void AndThenCorrectiveActionRun()
        {
            My<IProgramRulesService>()
                .Verify(mock => mock.RunCorrectiveActionForMember(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TriggeringEvent>(),
                    null), Times.Once);
        }

        public void AndBusPublishEventShouldBeCalled()
        {
            My<IBusControl>()
                .Verify(mock => mock.Publish(
                        It.IsAny<IssuanceChanged>(),
                        It.IsAny<CancellationToken>()),
                        Times.Exactly(3));
        }

        public void AndIssuanceStatusesShouldBeSetToProperStatus()
        {
            CommandResult.Data.Issuances[0].IssuanceStatus.ShouldBe(IssuanceStatusType.Expired); // Timelimited
            CommandResult.Data.Issuances[1].IssuanceStatus.ShouldBe(IssuanceStatusType.Expired); // MBM
            CommandResult.Data.Issuances[2].IssuanceStatus.ShouldBe(IssuanceStatusType.Inactive); // First GF remain Inactive
            CommandResult.Data.Issuances[3].IssuanceStatus.ShouldBe(IssuanceStatusType.Active); // Most recent GF becomes Active

            CommandResult.Data.Issuances[3].ExpiredDate.ShouldBeNull();
            CommandResult.Data.Issuances[3].ExpirationDate.ShouldBeNull();
            CommandResult.Data.IsActive.ShouldBeTrue();
        }
    }
    
    /// <summary>
    /// The Command fails validation scenario
    /// </summary>
    public class ReinstateCredentialCommandHandleReturnsRejectedWhenCommandFailsValidation
        : ReinstateCredentialCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ReinstateCredentialCommand Command               { get; set; }
        ReinstateCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        ValidationResult BadValidationResult                     { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ReinstateCredentialCommand>())
                .Returns(My<IValidator<ReinstateCredentialCommand>>().Object);
            
            My<IValidator<ReinstateCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<ReinstateCredentialCommand>()))
                .Returns(BadValidationResult);

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
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (ReinstateCredentialCommandResult)(CredentialService.Handle(Command).Result);
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
            CommandResult.Message.Should().Contain("Validation Failed:");
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
        
    }
    
    /// <summary>
    /// The Database save throws exception scenario
    /// </summary>
    public class ReinstateCredentialCommandHandleReturnsWarningDoesNotContainProperIssuanceStatuses
        : ReinstateCredentialCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ReinstateCredentialCommand Command               { get; set; }
        ReinstateCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        new string ExceptionText                                     { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ReinstateCredentialCommand>())
                .Returns(My<IValidator<ReinstateCredentialCommand>>().Object);
            My<IValidator<ReinstateCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<ReinstateCredentialCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            var issuance = Issuance.Create(RandomString.Build());
            issuance.IssuanceStatus = IssuanceStatusType.Expired; // Not proper status, should be Inactive, Revoked, Surrendered, or Suspended  
            existing.AddIssuance(issuance);
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));

            My<IProgramRulesService>()
                .Setup(mock => mock.RunCorrectiveActionForMember(
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<TriggeringEvent>(),
                    null))
                .Returns(Task.FromResult(true));

            My<IBusControl>()
                .Setup(mock => mock.Publish<IssuanceChanged>(
                    It.IsAny<IssuanceChanged>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

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
                CommandResult = (ReinstateCredentialCommandResult)(CredentialService.Handle(Command).Result);
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
            CommandResult.Message.Should().Contain("is NOT in Inactive, Revoked, Surrendered or Suspended status.");
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


    }
        
    /// <summary>
    /// The Repository Load throws exception scenario
    /// </summary>
    public class ReinstateCredentialCommandHandleReturnsRejectedWhenRepositoryLoadThrowsException
        : ReinstateCredentialCommandServiceScenario
    {
        ICredentialService CredentialService                     { get; set; }
        
        ReinstateCredentialCommand Command               { get; set; }
        ReinstateCredentialCommandResult CommandResult   { get; set; }
        Mock<ILogger> Log                                        { get; set; }
        new Exception ExceptionCaught                                { get; set; }
        new string ExceptionText                                     { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<ReinstateCredentialCommand>())
                .Returns(My<IValidator<ReinstateCredentialCommand>>().Object);
            My<IValidator<ReinstateCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<ReinstateCredentialCommand>()))
                .Returns(new ValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Throws(new Exception(ExceptionText));
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
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
                CommandResult = (ReinstateCredentialCommandResult)(CredentialService.Handle(Command).Result);
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
        
    }
            
    #endregion
}
