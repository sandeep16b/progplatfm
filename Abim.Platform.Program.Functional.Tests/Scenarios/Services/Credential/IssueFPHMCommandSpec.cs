using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Abim.Platform.Program.Relational.Queries;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using Hangfire;
using NUnit.Framework;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Shouldly;
using static Abim.Platform.Program.Resources.ProgramResourceConstants;

namespace Abim.Platform.Registration.Tests.Scenarios.Services.Registration
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the IssueFPHMCommand"
        )]
    [TestFixture]
    public class IssueFPHMCommandSpec
    {
        [TestCase]
        [WorkItem(201405)]
        public void IssueFPHMCommandHandleDeactivatesIMCredentialAndPublishesEventWhenAnActiveIMCredentialExistsTest()
        {
            new IssueFPHMCommandHandleDeactivatesIMCredentialAndPublishesEventWhenAnActiveIMCredentialExists().BDDfy();
        }

        [TestCase]
        [WorkItem(201405)]
        public void IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateSameAsEffectiveAndSubmittedDateTest()
        {
            new IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateSameAsEffectiveAndSubmittedDate().BDDfy();
        }

        [TestCase]
        [WorkItem(201405)]
        public void IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateAfterEffectiveAndSubmittedDateTest()
        {
            new IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateAfterEffectiveAndSubmittedDate().BDDfy();
        }

        [TestCase]
        [WorkItem(201405)]
        public void IssueFPHMCommandHandleShouldNotPublishDeactivationOfIMCredentialEventWhenErrOnIMCredentialDeactivationTest()
        {
            new IssueFPHMCommandHandleShouldNotPublishDeactivationOfIMCredentialEventWhenErrOnIMCredentialDeactivation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsAcceptedOnSuccess()
        {
            new IssueFPHMCommandHandleReturnsAcceptedOnSuccess().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new IssueFPHMCommandHandleReturnsRejectedWhenCommandFailsValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsRejectedWhenCertificationServiceGetByCodeThrowsException()
        {
            new IssueFPHMCommandHandleReturnsRejectedWhenCertificationServiceGetByCodeThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(73615)]
        public void IssueFPHMCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException()
        {
            new IssueFPHMCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException().BDDfy();
        }
        
        [TestCase]
        [WorkItem(141737)]
        public void IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndTraversesThroughTheFunctionWhenAnIMCredentialExists()
        {
            new IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndTraversesThroughTheFunctionWhenAnIMCredentialExists().BDDfy();
        }
        
        [TestCase]
        [WorkItem(141737)]
        public void IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndDoesNotTraverseshroughTheFunctionWhenAnIMCredentialDoesNotExist()
        {
            new IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndDoesNotTraverseshroughTheFunctionWhenAnIMCredentialDoesNotExist().BDDfy();
        }

        [TestCase]
        [WorkItem(192720)]
        public void IssueFPHMCommandHandleSetsFPHMCredentialAsSelectedToMaintain()
        {
            new IssueFPHMCommandHandleSetsFPHMCredentialAsSelectedToMaintainScenario().BDDfy();
        }
    }

    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class IssueFPHMCommandServiceScenario : CredentialServiceScenario
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
            types.Add(typeof(IValidator<IssueFPHMCommand>));
            types.Add(typeof(IHelperService));
            return types;
        }
    }

    #region Scenarios

    #region Disable Active IM on earning FPHM and Publish IM Deactivated Event
    internal class IssueFPHMCommandHandleDeactivatesIMCredentialAndPublishesEventWhenAnActiveIMCredentialExists
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential IMCredential { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential =
                Credential.Create(
                    Certification.Create(null,
                        Source.Create("American Board of Internal Medicine", "ABIM", RandomString.Build()),
                        CertificationType.Primary, "Internal Medicine", RandomString.Build(),
                        RandomString.Build()), Guid.NewGuid(), CredentialType.General,
                    EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            IMCredential.SelectedToMaintain = true;
            IMCredential.AddIssuance(IssuanceBuilder.Build(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), DateTime.Now));
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeATraceStatementFromExitingUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Leaving UpdateIMCredentialSelectedToMaintain"));
        }
        
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledTwiceOnceFormFphmOnceForIM()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Exactly(2));
        }

        public void AndIMCredentialIsDeactivatedWithCurrentDate()
        {
            IMCredential.NewestIssuance.DeselectionEffectiveDate.Value.Date.Should().Be(DateTime.Now.Date);
            IMCredential.NewestIssuance.DeselectionSubmittedDate.Value.Date.Should().Be(DateTime.Now.Date);
            IMCredential.NewestIssuance.DeselectionProcessedDate.Value.Date.Should().Be(DateTime.Now.Date);
            IMCredential.NewestIssuance.ExpiredDate.Value.Date.Should().Be(DateTime.Now.Date);
            IMCredential.NewestIssuance.IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
            IMCredential.NewestIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
            IMCredential.IsActive.ShouldBeFalse();
        }

        public void AndIMCredDeactivationEventIsPublishedToBus()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Publishing IMCertificate isssuance changed event."));
        }
    }

    #endregion Disable Active IM on earning FPHM and Publish IM Deactivated Event

    #region IssueFPHMCommand handle to ignore IMCredentail Deactivation when IMCredential already deactivated with ProcessedDate same as Effective and submittedDate scenario
    internal class IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateSameAsEffectiveAndSubmittedDate
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential IMCredential { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential =
                Credential.Create(
                    Certification.Create(null,
                        Source.Create("American Board of Internal Medicine", "ABIM", RandomString.Build()),
                        CertificationType.Primary, "Internal Medicine", RandomString.Build(),
                        RandomString.Build()), Guid.NewGuid(), CredentialType.General,
                    EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            var latestIMCredIssuance = IssuanceBuilder.Build(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), DateTime.Now);
            IMCredential.SelectedToMaintain = true;

            // IMCredential deactivation already processed on the same day as effective and submitted date.
            var credDeactivationMockDate = DateTime.Now;
            latestIMCredIssuance.DeselectionProcessedDate = credDeactivationMockDate;
            latestIMCredIssuance.DeselectionEffectiveDate = credDeactivationMockDate;
            latestIMCredIssuance.DeselectionSubmittedDate = credDeactivationMockDate;
            IMCredential.AddIssuance(latestIMCredIssuance);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommandWithIMCredentialAlreadyDeactivated()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeATraceStatementFromExitingUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Leaving UpdateIMCredentialSelectedToMaintain"));
        }
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledTwiceOnceFormFphmOnceForIM()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Exactly(2));
        }
        public void AndIMCredentialDeactivatIsIgnored()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("IMCredential is already Deactivated."));
        }

        public void AndIMCredentialIssuanceChangedEventIsNotPublishedToBus()
        {
            LogTest.Traces.Should().NotContain(s => s.StartsWith("Publishing IMCertificate isssuance changed event."));
        }
    }

    #endregion IssueFPHMCommand handle to ignore IMCredentail Deactivation when IMCredential already deactivated with ProcessedDate same as Effective and submittedDate scenario


    #region IssueFPHMCommand handle to ignore IMCredentail Deactivation when IMCredential already deactivated with ProcessedDate after Effective and submittedDate scenario1
    internal class IssueFPHMCommandHandleIgnoresDeactivationOfIMCredWhenTheIMCredIsAlreadyDeactivatedWithProcessedDateAfterEffectiveAndSubmittedDate
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential IMCredential { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential =
                Credential.Create(
                    Certification.Create(null,
                        Source.Create("American Board of Internal Medicine", "ABIM", RandomString.Build()),
                        CertificationType.Primary, "Internal Medicine", RandomString.Build(),
                        RandomString.Build()), Guid.NewGuid(), CredentialType.General,
                    EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            var latestIMCredIssuance = IssuanceBuilder.Build(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), DateTime.Now);
            IMCredential.SelectedToMaintain = true;

            // IMCredential deactivation already processed after effective and submitted date. (effective and submitted date < processed Date)
            var credDeactivationSubmittedMockDate = DateTime.Now;
            
            latestIMCredIssuance.DeselectionEffectiveDate = credDeactivationSubmittedMockDate;
            latestIMCredIssuance.DeselectionSubmittedDate = credDeactivationSubmittedMockDate;

            // Processed after (1 to 365) days later (random for testing)
            latestIMCredIssuance.DeselectionProcessedDate = credDeactivationSubmittedMockDate.AddDays(new Random(1).Next(365));

            IMCredential.AddIssuance(latestIMCredIssuance);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommandWithIMCredentialAlreadyDeactivated()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeATraceStatementFromExitingUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Leaving UpdateIMCredentialSelectedToMaintain"));
        }
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledTwiceOnceFormFphmOnceForIM()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Exactly(2));
        }
        public void AndIMCredentialDeactivatIsIgnored()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("IMCredential is already Deactivated."));
        }

        public void AndIMCredentialIssuanceChangedEventIsNotPublishedToBus()
        {
            LogTest.Traces.Should().NotContain(s => s.StartsWith("Publishing IMCertificate isssuance changed event."));
        }
    }

    #endregion IssueFPHMCommand handle to ignore IMCredentail Deactivation when IMCredential already deactivated with ProcessedDate after Effective and submittedDate scenario

    #region IssueFPHMCommand handle should not publish IMCred Deactivated event When err on IMCredentail Deactivation 
    internal class IssueFPHMCommandHandleShouldNotPublishDeactivationOfIMCredentialEventWhenErrOnIMCredentialDeactivation
        : IssueFPHMCommandServiceScenario
    {
        private ICredentialService CredentialService { get; set; }

        private IssueFPHMCommand Command { get; set; }
        private IssueFPHMCommandResult CommandResult { get; set; }
        private Mock<ILogger> Log { get; set; }
        private new Exception ExceptionCaught { get; set; }
        private new string ExceptionText { get; set; }
        private Credential IMCredential { get; set; }

        private Guid mockedFPHMCredGuid = Guid.NewGuid();
        private Guid mockedIMCredGuid = Guid.NewGuid();

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential =
                Credential.Create(
                    Certification.Create(null,
                        Source.Create("American Board of Internal Medicine", "ABIM", RandomString.Build()),
                        CertificationType.Primary, "Internal Medicine", CertificationCode.InternalMedicine,
                        RandomString.Build()), Guid.NewGuid(), CredentialType.General,
                    EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            var latestIMCredIssuance = IssuanceBuilder.Build(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), DateTime.Now);
            IMCredential.SelectedToMaintain = true;
            IMCredential.AddIssuance(latestIMCredIssuance);
            IMCredential.ExternalId = mockedIMCredGuid;
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            var existingFPHMCred = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), CertificationCode.FocusedPracticeHospitalMedicine, RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(mockedFPHMCredGuid))
                .Returns(existingFPHMCred);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.Is<Credential>(_ => _.Certification.Code == CertificationCode.FocusedPracticeHospitalMedicine), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true }); // FPHM Database update succeeds.
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.Is<Credential>(_ => _.Certification.Code == CertificationCode.InternalMedicine), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = false }); // Database fails to  Updated IM cred deactivation
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommandWithIMCredDeactivationExpectedToFail()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
            Command.CredentialId = mockedFPHMCredGuid;
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeATraceStatementFromExitingUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Leaving UpdateIMCredentialSelectedToMaintain"));
        }
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledTwiceOnceFormFphmOnceForIM()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Exactly(2));
        }
        public void AndLogContainsErrorAboutIMCredUpdateFailure()
        {
            LogTest.Errors.Should().Contain(s => s.StartsWith("UpdateIMCredentialSelectedToMaintain - Unable to update IM Credential"));
        }
        public void AndIMCredentialIssuanceChangedEventIsNotPublishedToBus()
        {
            LogTest.Traces.Should().NotContain(s => s.StartsWith("Publishing IMCertificate isssuance changed event."));
        }
    }
    #endregion IssueFPHMCommand handle should not publish IMCred isssuance changed When err on IMCredentail Deactivation 
    /// <summary>
    /// The Success scenario
    /// </summary>
    public class IssueFPHMCommandHandleReturnsAcceptedOnSuccess
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }

        protected Credential ExistingFPHMCredential { get; set; }

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
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            ExistingFPHMCredential = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(ExistingFPHMCredential);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    public class IssueFPHMCommandHandleReturnsRejectedWhenCommandFailsValidation
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult BadValidationResult { get; set; }
        
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
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(BadValidationResult);
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAnInvalidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Invalid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    public class IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
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
            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    public class IssueFPHMCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult BadValidationResult { get; set; }
        
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
            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(BadValidationResult.ToAbimValidationResult());
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    /// The CertificationService GetByCode throws exception scenario
    /// </summary>
    public class IssueFPHMCommandHandleReturnsRejectedWhenCertificationServiceGetByCodeThrowsException
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
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
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICertificationService>()
                .Setup(o => o.GetByCode(It.IsAny<string>()))
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    public class IssueFPHMCommandHandleReturnsRejectedWhenSourceServiceGetAbimSourceThrowsException
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }
        
        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
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
            CredentialService = Container.GetInstance<CredentialService>();
            
            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), 
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), 
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult(){ Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Throws(new Exception(ExceptionText));
            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        //.With(cmd => cmd.UserInfo = new UserInfo()
                        //    {
                        //        Username = RandomString.Build(),
                        //        AbimId = Random.Next(1000, 100000).ToString()
                        //    })
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
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
    /// IssueFPHMCommandHandleSetsIMCertSelectedMaintainToFalse
    /// </summary>
    public class IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndTraversesThroughTheFunctionWhenAnIMCredentialExists
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential IMCredential { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential =
                Credential.Create(
                    Certification.Create(null,
                        Source.Create("American Board of Internal Medicine", "ABIM", RandomString.Build()),
                        CertificationType.Primary, "Internal Medicine", RandomString.Build(),
                        RandomString.Build()), Guid.NewGuid(), CredentialType.General,
                    EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            IMCredential.SelectedToMaintain = true;
            IMCredential.AddIssuance(IssuanceBuilder.Build(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), DateTime.Now));
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeATraceStatementFromExitingUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Leaving UpdateIMCredentialSelectedToMaintain"));
        }
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledTwiceOnceFormFphmOnceForIM()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Exactly(2));
        }
        public void AndIMCredentialSelectedToMaintainShouldBeSetToFalse()
        {
            IMCredential.SelectedToMaintain.ShouldBeFalse();
        }
    }

    /// <summary>
    /// IssueFPHMCommandHandleSetsIMCertSelectedMaintainToFalse
    /// </summary>
    public class IssueFPHMCommandHandleCallsUpdateIMCredentialSelectedToMaintainAndDoesNotTraverseshroughTheFunctionWhenAnIMCredentialDoesNotExist
        : IssueFPHMCommandServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }
        new string ExceptionText { get; set; }
        Credential IMCredential { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            ExceptionText = RandomString.Build();
            IMCredential = null;
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<IssueFPHMCommand>())
                .Returns(My<IValidator<IssueFPHMCommand>>().Object);
            My<IValidator<IssueFPHMCommand>>()
                .Setup(o => o.Validate(It.IsAny<IssueFPHMCommand>()))
                .Returns(new ValidationResult());
            Credential existing = Credential.Create(Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()),
                EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(SourceBuilder.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ICredentialRepository>()
                .Setup(o => o.GetIMCredentialByMember(It.IsAny<Guid>()))
                .Returns(IMCredential);

            ((CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                CommandResult = (IssueFPHMCommandResult)(CredentialService.Handle(Command));
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenAnExceptionShouldNotHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Traces.Any(s => s.StartsWith("Started Handle for IssueFPHMCommand")) ||
                log.Debugs.Any(s => s.Contains("Command Args for IssueFPHMCommand")));
        }

        public void AndThenThereShouldBeATraceStatementFromEnteringUpdateImCredentialSelectedToMaintain()
        {
            LogTest.Traces.Should().Contain(s => s.Contains("Entering UpdateIMCredentialSelectedToMaintain"));
        }

        public void AndThenThereShouldBeADebugLogStatementAboutNoIMCredFound()
        {
            LogTest.Debugs.Should().Contain(s => s.StartsWith("UpdateIMCredentialSelectedToMaintain - No IM Credential Found for memberId"));
        }
        public void AndCredentialRepositoryUpdateShouldHaveBeenCalledOnce()
        {
            My<ICredentialRepository>().Verify(p => p.Update(It.IsAny<Credential>(), It.IsAny<string>()), Times.Once);
        }
    }

    public class IssueFPHMCommandHandleSetsFPHMCredentialAsSelectedToMaintainScenario : IssueFPHMCommandHandleReturnsAcceptedOnSuccess
    {
        protected override void PostSetup()
        {
            base.PostSetup();
            ExistingFPHMCredential.SelectedToMaintain = false;
        }

        protected void AndTheFPHMCredentialShouldBeSetToSelectedToMaintain()
        {
            ExistingFPHMCredential.SelectedToMaintain.ShouldBeTrue();
        }
    }

    #endregion
}
