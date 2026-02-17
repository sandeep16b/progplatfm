using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Certification;
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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Certification
{
    [Story(
     AsA = "external application",
     IWant = "to be able to call the Certification Api",
     SoThat = "to run my AddCertification command"
     )]

    [TestFixture]
    public class AddCertificationCommandRouteApiSpec
    {
        [TestCase]
        [WorkItem(95545)]
        public void AddCertificationCommandResultReturnsOK()
        {
            new AddCertificationCommandResultReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(95545)]

        public void AddCertificationInvalidCommandResultReturnsBadRequest()
        {
            new AddCertificationInvalidCommandResultReturnsBadRequest().BDDfy();
        }
    }

    public abstract class AddPrimaryCertificationCommandServiceScenario
        : CertificationServiceScenario
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
}

#region Scenarios

/// <summary>
/// The Success scenario
/// </summary>
public class AddCertificationCommandResultReturnsOK
    : AddPrimaryCertificationCommandServiceScenario
{
    ICertificationService CertificationService { get; set; }

    AddPrimaryCertificationCommand Command { get; set; }
    AddPrimaryCertificationCommandResult CommandResult { get; set; }
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
            .Returns(new AbimValidationResult() { Succeeded = true });
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
        catch (Exception ex)
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
}

public class AddCertificationInvalidCommandResultReturnsBadRequest
        : AddPrimaryCertificationCommandServiceScenario
{
    ICertificationService CertificationService { get; set; }

    AddPrimaryCertificationCommand Command { get; set; }
    AddPrimaryCertificationCommandResult CommandResult { get; set; }
    Mock<ILogger> Log { get; set; }
    new EmailBuilder EmailBuilder { get; set; }
    new DateTimeBuilder DateTimeBuilder { get; set; }
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
        for (int i = 0; i < Random.Next(1, 100); i++)
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
            .Returns(new AbimValidationResult() { Succeeded = true });
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
        catch (Exception ex)
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
#endregion


