using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using static Abim.Platform.Program.Tests.Scenarios.Interservices.ProgramRulesHelpersTest;
using Assert = NUnit.Framework.Assert;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesInternalFunctions
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run Print Grand Father certificate"
      )]
    [TestFixture]
    public class ComputeLookBackWindowDuringLookBackSpec
    {
        [TestCase]
        public void ComputeLookBackWindowDuringLookBack_5yearWindow_Scenario()
        {
            new ComputeLookBackWindowDuringLookBack_5yearWindow().BDDfy();
        }

        [TestCase]
        public void ComputeLookBackWindowDuringLookBack_2yearWindow_Scenario()
        {
            new ComputeLookBackWindowDuringLookBack_2yearWindow().BDDfy();
        }

        public abstract class ComputeLookBackWindowDuringLookBackScenario : ProgramRulesServiceScenario
        {

            protected ProgramRulesService ProgramRulesService { get; set; }
            protected new Exception ExceptionCaught { get; set; }
            protected IList<DatesTestValues> TestCases { get; set; }
        }

        public class ComputeLookBackWindowDuringLookBack_5yearWindow : ComputeLookBackWindowDuringLookBackScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
               
                TestCases = new List<DatesTestValues>();

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(1978, 06, 27), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2022, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2025, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2026, 12, 30), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2026, 12, 31), StartWindow = new DateTime(2022, 01, 01), EndWindow = new DateTime(2026, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2027, 01, 01), StartWindow = new DateTime(2022, 01, 01), EndWindow = new DateTime(2026, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2031, 12, 30), StartWindow = new DateTime(2027, 01, 01), EndWindow = new DateTime(2031, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2031, 12, 31), StartWindow = new DateTime(2027, 01, 01), EndWindow = new DateTime(2031, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2032, 01, 01), StartWindow = new DateTime(2027, 01, 01), EndWindow = new DateTime(2031, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2022, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2023, 12, 30), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2023, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2024, 01, 01), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            }

            public void WhenICallProgramInternalFunction()
            {
                try
                {

                    foreach (var test in TestCases)
                    {

                        var result = ProgramRulesService.ComputeLookBackWindowDuringLookBack(
                                                                               earliestCertDate: test.EarliestCertDate,
                                                                               checkDate: test.CheckDate,
                                                                               windowsInterval: WindowsIntervalType.FiveYearLookBack);

                        Assert.That(test.StartWindow, Is.EqualTo(result.Item1));
                        Assert.That(test.EndWindow, Is.EqualTo(result.Item2));
                    }

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

        }

        public class ComputeLookBackWindowDuringLookBack_2yearWindow : ComputeLookBackWindowDuringLookBackScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {

                TestCases = new List<DatesTestValues>();

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2017, 12, 30), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2018, 12, 31), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2019, 01, 01), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 30), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2020, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2021, 01, 21), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2020, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2015, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2015, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2016, 01, 01), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2015, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 30), StartWindow = new DateTime(2016, 01, 01), EndWindow = new DateTime(2017, 12, 31) });

                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2016, 01, 01), EndWindow = new DateTime(2017, 12, 31) });
                TestCases.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2018, 01, 01), StartWindow = new DateTime(2016, 01, 01), EndWindow = new DateTime(2017, 12, 31) });
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            }

            public void WhenICallProgramInternalFunction()
            {
                try
                {

                    foreach (var test in TestCases)
                    {

                        var result = ProgramRulesService.ComputeLookBackWindowDuringLookBack(
                                                                               earliestCertDate: test.EarliestCertDate,
                                                                               checkDate: test.CheckDate,
                                                                               windowsInterval: WindowsIntervalType.TwoYearLookBack);

                        Assert.That(test.StartWindow, Is.EqualTo(result.Item1));
                        Assert.That(test.EndWindow, Is.EqualTo(result.Item2));
                    }

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

        }
    }
}
