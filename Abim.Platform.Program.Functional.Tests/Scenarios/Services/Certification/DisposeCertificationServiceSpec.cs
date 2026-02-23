using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CertificationService.Base;
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
    public class DisposeCertificationServiceSpec
    {
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        [Test]
        public void DisposeCertificationServiceWhenDisposeMethodCalled()
        {
            var disposeCertificationService = new DisposeCertificationServiceWhenDisposeMethodCalled();
            disposeCertificationService.SetUp();
            disposeCertificationService.Should_Dispose_Certification_Service_When_Dispose_Method_Is_Done();
        }
    }


    /// <summary>
    /// Base class
    /// </summary>
    public abstract class DisposeCertificationServiceScenario : CertificationServiceScenario
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
            types.Add(typeof(IValidator<AddAddedQualificationCertificationCommand>));
            return types;
        }
    }

    #region Scenarios

    /// <summary>
    /// The Success scenario
    /// </summary>
    public class DisposeCertificationServiceWhenDisposeMethodCalled
        : DisposeCertificationServiceScenario
    {
        ICertificationService CertificationService { get; set; }

        AddAddedQualificationCertificationCommand Command { get; set; }
        AddAddedQualificationCertificationCommandResult CommandResult { get; set; }
        Mock<ILogger> Log { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
        }

        protected override void PostSetup()
        {
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();

            My<IValidationFactory>()
                .Setup(o => o.GetValidatorInstance<AddAddedQualificationCertificationCommand>())
                .Returns(My<IValidator<AddAddedQualificationCertificationCommand>>().Object);
            My<IValidator<AddAddedQualificationCertificationCommand>>()
                .Setup(o => o.Validate(It.IsAny<AddAddedQualificationCertificationCommand>()))
                .Returns(new FluentValidation.Results.ValidationResult());
            Certification existing = Certification.Create(
                null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(
                ), RandomString.Build(), RandomString.Build(), RandomString.Build());
            My<ICertificationRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);
            My<ICertificationRepository>()
                .Setup(o => o.Add(It.IsAny<Certification>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult() { Succeeded = true });
            My<ISourceService>()
                .Setup(o => o.GetAbimSource())
                .Returns(Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()));
            ((App.Services.Impl.CertificationService)CertificationService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void Should_Dispose_Certification_Service_When_Dispose_Method_Is_Done()
        {
            Command = CommandBuilder<AddAddedQualificationCertificationCommand>
                .Valid()
                .With(cmd => cmd.UserInfo = new UserInfo()
                {
                    Username = RandomString.Build(),
                    AbimId = Random.Next(1000, 100000).ToString()
                })
                .Build();
            using (var certificationService = CertificationService)
            {
                certificationService.Handle(Command);
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
