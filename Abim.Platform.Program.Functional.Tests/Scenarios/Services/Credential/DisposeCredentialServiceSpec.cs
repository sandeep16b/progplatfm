using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentValidation;
using Hangfire;
using JetBrains.dotMemoryUnit;
using MassTransit;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Registration
{
    [TestFixture]
    public class DisposeCredentialServiceSpec
    {
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        [Test]
        public void DisposeCredentialServiceWhenDisposeMethodIsDone()
        {
            var disposeCredentialService = new DisposeCredentialServiceWhenDisposeMethodCalled();
            disposeCredentialService.SetUp();
            disposeCredentialService.Should_Dispose_Credential_Service_When_Dispose_Method_Is_Done();
        }
    }

    /// <summary>
    /// Base class
    /// </summary>
    public abstract class DisposeCredentialServiceScenario : CredentialServiceScenario
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
            types.Add(typeof(IValidator<CreateCredentialCommand>));
            types.Add(typeof(IHelperService));
            return types;
        }
    }

    #region Scenarios

    /// <summary>
    /// The Success scenario
    /// </summary>
    public class DisposeCredentialServiceWhenDisposeMethodCalled
        : DisposeCredentialServiceScenario
    {
        ICredentialService CredentialService { get; set; }

        CreateCredentialCommand Command { get; set; }
        CreateCredentialCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
        }

        protected override void PostSetup()
        {
            CredentialService = Container.GetInstance<App.Services.Impl.CredentialService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<CreateCredentialCommand>())
                .Returns(My<IValidator<CreateCredentialCommand>>().Object);
            My<IValidator<CreateCredentialCommand>>()
                .Setup(o => o.Validate(It.IsAny<CreateCredentialCommand>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            App.Domain.Credential existing = App.Domain.Credential.Create(App.Domain.Certification.Create(null, App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build(
                )), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build()), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(
            ), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            My<ICredentialRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICredentialRepository>()
                .Setup(o => o.Add(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            ((App.Services.Impl.CredentialService)CredentialService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void Should_Dispose_Credential_Service_When_Dispose_Method_Is_Done()
        {
            Command = CommandBuilder<CreateCredentialCommand>
                .Valid()
                .With(cmd => cmd.UserInfo = new UserInfo()
                {
                    Username = RandomString.Build(),
                    AbimId = Random.Next(1000, 100000).ToString()
                })
                .Build();

            using (CredentialService)
            {
                CredentialService.Handle(Command);
            }

            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<ISession>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));
            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<ICertificationService>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));
            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<ICredentialService>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));
            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<IHelperService>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));
            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<IProgramRulesService>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));
            dotMemory.Check(memory =>
                Assert.That(memory.GetObjects(where => where.Interface.Is<ISourceService>()).ObjectsCount,
                    Is.LessThanOrEqualTo(1)));

        }
    }
    #endregion
}
