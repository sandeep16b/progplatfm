using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults.LookbackDateLog;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Tests.Scenarios.Services.LookBackDatesInfoService.Base;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
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

namespace Abim.Platform.Registration.Tests.Scenarios.Services.Registration
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the LookBackDatesInfoService",
        SoThat = "it can handle the UpdateLookBackDatesInfoCommand"
        )]
    [TestFixture]
    public class UpdateLookBackDatesInfoCommandCommandSpec
    {
        [TestCase]
        [WorkItem(133038)]
        public void UpdateLookBackDatesInfoCommandHandleToUpdateRecord()
        {
            new UpdateLookBackDatesInfoCommandHandleToUpdateRecord().BDDfy();
        }
        
       [TestCase]
       [WorkItem(133038)]
        public void UpdateLookBackDatesInfoCommandHandleToAddRecord()
        {
            new UpdateLookBackDatesInfoCommandHandleToAddRecord().BDDfy();
        }
 
        [TestCase]
        [WorkItem(137967)]
        public void UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewTwoYearValues()
        {
            new UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewTwoYearValues().BDDfy();
        }
        
        [TestCase]
        [WorkItem(137967)]
        public void UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearValues()
        {
            new UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearValues().BDDfy();
        }
        
        [TestCase]
        [WorkItem(137967)]
        public void UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearAndTwoYearValues()
        {
            new UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearAndTwoYearValues().BDDfy();
        }
    }

    /// <summary>
    /// Base class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.Program.Base.CredentialServiceScenario" />
    public abstract class UpdateLookBackDatesInfoCommandServiceScenario : LookBackDatesServiceScenario
    {

        protected DateTime Lookback2YearStartDate { get; set; }
        protected DateTime Lookback2YearEndDate { get; set; }

        protected DateTime Lookback5YearStartDate { get; set; }
        protected DateTime Lookback5YearEndDate { get; set; }

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ILookBackDatesInfoRepository));
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
            types.Add(typeof(IValidator<UpdateLookBackDatesInfoCommand>));

            types.Add(typeof(ILookbackDateLogService));
            types.Add(typeof(IValidator<AddLookbackDateLogEntry>));

            return types;
        }
    }
    
    #region Scenarios
    
    /// <summary>
    /// The Update scenario
    /// </summary>
    public class UpdateLookBackDatesInfoCommandHandleToUpdateRecord
        : UpdateLookBackDatesInfoCommandServiceScenario
    {
        ILookBackDatesInfoService LookBackDatesInfoService          { get; set; }
        
        UpdateLookBackDatesInfoCommand Command               { get; set; }
        Mock<ILogger> Log                             { get; set; }
        new DateTimeBuilder DateTimeBuilder               { get; set; }
        new Exception ExceptionCaught                     { get; set; }
        
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            DateTimeBuilder = new DateTimeBuilder();

            Lookback2YearStartDate = new DateTime(2018, 1, 1);
            Lookback2YearEndDate = new DateTime(2019, 12, 31);

            Lookback5YearStartDate = new DateTime(2014, 1, 1);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);
        }
        
        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            LookBackDatesInfoService = Container.GetInstance<LookBackDatesInfoService>();

            // return existing LookBackDates
            My<ILookBackDatesInfoRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(LookBackDatesInfo.Create(new Guid(),
                                                    Lookback2YearStartDate,
                                                    Lookback2YearEndDate,
                                                    Lookback5YearStartDate,
                                                    Lookback5YearEndDate,
                                                    ""));

            My<ILookbackDateLogService>()
                .Setup(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()))
                .Returns(new AddLookbackDateLogCommandResult());

            ((LookBackDatesInfoService)LookBackDatesInfoService).Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<UpdateLookBackDatesInfoCommand>
                        .Valid()
                        .With(cmd=>cmd.MemberId=new Guid())
                        .With(cmd => cmd.UserName = RandomString.Build())
                        .With(cmd => cmd.Lookback2YearStartDate= Lookback2YearStartDate)
                        .With(cmd => cmd.Lookback2YearEndDate = Lookback2YearEndDate)
                        .With(cmd => cmd.Lookback5YearStartDate = Lookback5YearStartDate)
                        .With(cmd => cmd.Lookback5YearEndDate = Lookback5YearEndDate)
                        .Build();
        }
        
        public void WhenICallHandle()
        {
            try
            {
                LookBackDatesInfoService.Handle(Command);
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

        public void AndThen_UpdateLookBackDatesInfo_Repository_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoRepository>()
                .Verify(p => p.UpdateLookBackDatesInfo(It.IsAny<LookBackDatesInfo>()), 
                Times.Once());
        }

        public void AndThen_AddLookbackDateLogEntry_Should_Not_BeCalled_When_UpdatedValues_AreTheSame_As_Previous()
        {
            My<ILookbackDateLogService>()
            .Verify(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()),
                Times.Never());
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Debugs.Any(s => s.StartsWith("UpdateLookBackDatesInfoCommand Command Args:")));
        }

        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Ended Handle for UpdateLookBackDatesInfoCommand"));
        }
    }

    /// <summary>
    /// The Add scenario
    /// </summary>
    public class UpdateLookBackDatesInfoCommandHandleToAddRecord
        : UpdateLookBackDatesInfoCommandServiceScenario
    {
        ILookBackDatesInfoService LookBackDatesInfoService { get; set; }

        UpdateLookBackDatesInfoCommand Command { get; set; }
        Mock<ILogger> Log { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            DateTimeBuilder = new DateTimeBuilder();

            Lookback2YearStartDate = new DateTime(2018, 1, 1);
            Lookback2YearEndDate = new DateTime(2019, 12, 31);

            Lookback5YearStartDate = new DateTime(2014, 1, 1);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            LookBackDatesInfoService = Container.GetInstance<LookBackDatesInfoService>();

            // return null (no record is exist)
            My<ILookBackDatesInfoRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns((LookBackDatesInfo)null);

            My<ILookbackDateLogService>()
               .Setup(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()))
               .Returns(new AddLookbackDateLogCommandResult());

            ((LookBackDatesInfoService)LookBackDatesInfoService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<UpdateLookBackDatesInfoCommand>
                        .Valid()
                        .With(cmd => cmd.MemberId = new Guid())
                        .With(cmd => cmd.UserName = RandomString.Build())
                        .With(cmd => cmd.Lookback2YearStartDate = Lookback2YearStartDate)
                        .With(cmd => cmd.Lookback2YearEndDate = Lookback2YearEndDate)
                        .With(cmd => cmd.Lookback5YearStartDate = Lookback5YearStartDate)
                        .With(cmd => cmd.Lookback5YearEndDate = Lookback5YearEndDate)
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                LookBackDatesInfoService.Handle(Command);
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

        public void AndThen_UpdateLookBackDatesInfo_Repository_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoRepository>()
                .Verify(p => p.AddLookBackDatesInfo(It.IsAny<LookBackDatesInfo>()),
                Times.Once());
        }

        public void AndThen_AddLookbackDateLogEntry_Should_BeCalled_ForEach_NewDate()
        {
            My<ILookbackDateLogService>()
                .Verify(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()),
                    Times.Exactly(4));
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Debugs.Any(s => s.StartsWith("UpdateLookBackDatesInfoCommand Command Args:")));
        }

        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Ended Handle for UpdateLookBackDatesInfoCommand"));
        }
    }


    /// <summary>
    /// The Update with new 2 year lookback values scenario
    /// </summary>
    public class UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewTwoYearValues
        : UpdateLookBackDatesInfoCommandServiceScenario
    {
        ILookBackDatesInfoService LookBackDatesInfoService { get; set; }

        UpdateLookBackDatesInfoCommand Command { get; set; }
        Mock<ILogger> Log { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            DateTimeBuilder = new DateTimeBuilder();

            Lookback2YearStartDate = new DateTime(2018, 1, 1);
            Lookback2YearEndDate = new DateTime(2019, 12, 31);

            Lookback5YearStartDate = new DateTime(2014, 1, 1);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            LookBackDatesInfoService = Container.GetInstance<LookBackDatesInfoService>();

            // return existing LookBackDates
            My<ILookBackDatesInfoRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(LookBackDatesInfo.Create(new Guid(),
                                                    Lookback2YearStartDate.AddYears(-2),
                                                    Lookback2YearEndDate.AddYears(-2),
                                                    Lookback5YearStartDate,
                                                    Lookback5YearEndDate,
                                                    ""));

            My<ILookbackDateLogService>()
                .Setup(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()))
                .Returns(new AddLookbackDateLogCommandResult());

            ((LookBackDatesInfoService)LookBackDatesInfoService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<UpdateLookBackDatesInfoCommand>
                        .Valid()
                        .With(cmd => cmd.MemberId = new Guid())
                        .With(cmd => cmd.UserName = RandomString.Build())
                        .With(cmd => cmd.Lookback2YearStartDate = Lookback2YearStartDate)
                        .With(cmd => cmd.Lookback2YearEndDate = Lookback2YearEndDate)
                        .With(cmd => cmd.Lookback5YearStartDate = Lookback5YearStartDate)
                        .With(cmd => cmd.Lookback5YearEndDate = Lookback5YearEndDate)
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                LookBackDatesInfoService.Handle(Command);
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

        public void AndThen_UpdateLookBackDatesInfo_Repository_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoRepository>()
                .Verify(p => p.UpdateLookBackDatesInfo(It.IsAny<LookBackDatesInfo>()),
                Times.Once());
        }

        public void AndThen_AddLookbackDateLogEntry_Should_We_BeCalled_With_NewValues_For_TwoYearLookback()
        {
            My<ILookbackDateLogService>()
            .Verify(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()),
                Times.Exactly(2));
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Debugs.Any(s => s.StartsWith("UpdateLookBackDatesInfoCommand Command Args:")));
        }

        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Ended Handle for UpdateLookBackDatesInfoCommand"));
        }
    }

    /// <summary>
    /// The Update with new 5 year lookback values scenario
    /// </summary>
    public class UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearValues
        : UpdateLookBackDatesInfoCommandServiceScenario
    {
        ILookBackDatesInfoService LookBackDatesInfoService { get; set; }

        UpdateLookBackDatesInfoCommand Command { get; set; }
        Mock<ILogger> Log { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            DateTimeBuilder = new DateTimeBuilder();

            Lookback2YearStartDate = new DateTime(2018, 1, 1);
            Lookback2YearEndDate = new DateTime(2019, 12, 31);

            Lookback5YearStartDate = new DateTime(2014, 1, 1);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            LookBackDatesInfoService = Container.GetInstance<LookBackDatesInfoService>();

            // return existing LookBackDates
            My<ILookBackDatesInfoRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(LookBackDatesInfo.Create(new Guid(),
                                                    Lookback2YearStartDate,
                                                    Lookback2YearEndDate,
                                                    Lookback5YearStartDate.AddYears(-2),
                                                    Lookback5YearEndDate.AddYears(-2),
                                                    ""));

            My<ILookbackDateLogService>()
                .Setup(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()))
                .Returns(new AddLookbackDateLogCommandResult());

            ((LookBackDatesInfoService)LookBackDatesInfoService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<UpdateLookBackDatesInfoCommand>
                        .Valid()
                        .With(cmd => cmd.MemberId = new Guid())
                        .With(cmd => cmd.UserName = RandomString.Build())
                        .With(cmd => cmd.Lookback2YearStartDate = Lookback2YearStartDate)
                        .With(cmd => cmd.Lookback2YearEndDate = Lookback2YearEndDate)
                        .With(cmd => cmd.Lookback5YearStartDate = Lookback5YearStartDate)
                        .With(cmd => cmd.Lookback5YearEndDate = Lookback5YearEndDate)
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                LookBackDatesInfoService.Handle(Command);
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

        public void AndThen_UpdateLookBackDatesInfo_Repository_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoRepository>()
                .Verify(p => p.UpdateLookBackDatesInfo(It.IsAny<LookBackDatesInfo>()),
                Times.Once());
        }

        public void AndThen_AddLookbackDateLogEntry_Should_We_BeCalled_With_NewValues_For_FiveYearLookback()
        {
            My<ILookbackDateLogService>()
            .Verify(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()),
                Times.Exactly(2));
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Debugs.Any(s => s.StartsWith("UpdateLookBackDatesInfoCommand Command Args:")));
        }

        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Ended Handle for UpdateLookBackDatesInfoCommand"));
        }
    }

    /// <summary>
    /// The Update with new 5 year lookback values scenario
    /// </summary>
    public class UpdateLookBackDatesInfoCommandHandleToUpdateRecordWithNewFiveYearAndTwoYearValues
        : UpdateLookBackDatesInfoCommandServiceScenario
    {
        ILookBackDatesInfoService LookBackDatesInfoService { get; set; }

        UpdateLookBackDatesInfoCommand Command { get; set; }
        Mock<ILogger> Log { get; set; }
        new DateTimeBuilder DateTimeBuilder { get; set; }
        new Exception ExceptionCaught { get; set; }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            DateTimeBuilder = new DateTimeBuilder();

            Lookback2YearStartDate = new DateTime(2018, 1, 1);
            Lookback2YearEndDate = new DateTime(2019, 12, 31);

            Lookback5YearStartDate = new DateTime(2014, 1, 1);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            LookBackDatesInfoService = Container.GetInstance<LookBackDatesInfoService>();

            // return existing LookBackDates
            My<ILookBackDatesInfoRepository>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(LookBackDatesInfo.Create(new Guid(),
                                                    Lookback2YearStartDate.AddYears(-2),
                                                    Lookback2YearEndDate.AddYears(-2),
                                                    Lookback5YearStartDate.AddYears(-2),
                                                    Lookback5YearEndDate.AddYears(-2),
                                                    ""));

            My<ILookbackDateLogService>()
                .Setup(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()))
                .Returns(new AddLookbackDateLogCommandResult());

            ((LookBackDatesInfoService)LookBackDatesInfoService).Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenIInputAValidCommand()
        {
            Command = CommandBuilder<UpdateLookBackDatesInfoCommand>
                        .Valid()
                        .With(cmd => cmd.MemberId = new Guid())
                        .With(cmd => cmd.UserName = RandomString.Build())
                        .With(cmd => cmd.Lookback2YearStartDate = Lookback2YearStartDate)
                        .With(cmd => cmd.Lookback2YearEndDate = Lookback2YearEndDate)
                        .With(cmd => cmd.Lookback5YearStartDate = Lookback5YearStartDate)
                        .With(cmd => cmd.Lookback5YearEndDate = Lookback5YearEndDate)
                        .Build();
        }

        public void WhenICallHandle()
        {
            try
            {
                LookBackDatesInfoService.Handle(Command);
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

        public void AndThen_UpdateLookBackDatesInfo_Repository_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoRepository>()
                .Verify(p => p.UpdateLookBackDatesInfo(It.IsAny<LookBackDatesInfo>()),
                Times.Once());
        }

        public void AndThen_AddLookbackDateLogEntry_Should_We_BeCalled_With_NewValues_For_TwoYearAndFiveYearLookback()
        {
            My<ILookbackDateLogService>()
            .Verify(o => o.Handle(It.IsAny<AddLookbackDateLogEntry>()),
                Times.Exactly(4));
        }

        public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
        {
            LogTest.Should().Match<LogTest>(log =>
                log.Debugs.Any(s => s.StartsWith("UpdateLookBackDatesInfoCommand Command Args:")));
        }

        public void AndThenThereShouldBeAnEndTrace()
        {
            LogTest.Traces.Should().Contain(s => s.StartsWith("Ended Handle for UpdateLookBackDatesInfoCommand"));
        }
    }

    #endregion

}
