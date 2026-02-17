
using Abim.Platform.Program.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Interservices
{
    public class ProgramRulesHelpersTest
    {

        /// <summary>
        /// TLPC validate 5 year window formula
        /// </summary>
        [Test]
        //[WorkItem(73194)]
        public void TLPCValidate5yearWindow()
        {

            IList<DatesTestValues> list = new List<DatesTestValues>();

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(1978, 06, 27), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) }) ;
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2022, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2025, 12, 31), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2026, 12, 30), StartWindow = new DateTime(2016, 11, 1), EndWindow = new DateTime(2021, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2026, 12, 31), StartWindow = new DateTime(2022, 01, 01), EndWindow = new DateTime(2026, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2027, 01, 01), StartWindow = new DateTime(2022, 01, 01), EndWindow = new DateTime(2026, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2031, 12, 30), StartWindow = new DateTime(2022, 01, 01), EndWindow = new DateTime(2026, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2031, 12, 31), StartWindow = new DateTime(2027, 01, 01), EndWindow = new DateTime(2031, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2032, 01, 01), StartWindow = new DateTime(2027, 01, 01), EndWindow = new DateTime(2031, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2022, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2023, 12, 30), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2018, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2023, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2024, 01, 01), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2023, 12, 31) });

            #region 5YearWindowsDates
            /*
            EARLIESTCERT       CHECKDATE          START5WINDOW       END5WINDOW
            ------------------ ------------------ ------------------ ------------------
            01-NOV-16          31-DEC-16          01-NOV-16          31-DEC-21
            01-NOV-16          31-DEC-17          01-NOV-16          31-DEC-21
            01-NOV-16          31-DEC-20          01-NOV-16          31-DEC-21
            01-NOV-16          31-DEC-22          01-NOV-16          31-DEC-21
            01-NOV-16          31-DEC-25          01-NOV-16          31-DEC-21
            01-NOV-16          30-DEC-26          01-NOV-16          31-DEC-21

            01-NOV-16          31-DEC-26          01-JAN-22          31-DEC-26
            01-NOV-16          01-JAN-27          01-JAN-22          31-DEC-26
            01-NOV-16          30-DEC-31          01-JAN-22          31-DEC-26

            01-NOV-16          31-DEC-31          01-JAN-27          31-DEC-31
            01-NOV-16          01-JAN-32          01-JAN-27          31-DEC-31

            01-NOV-13          31-DEC-16          01-JAN-14          31-DEC-18
            01-NOV-13          31-DEC-17          01-JAN-14          31-DEC-18
            01-NOV-13          31-DEC-20          01-JAN-14          31-DEC-18
            01-NOV-13          31-DEC-22          01-JAN-14          31-DEC-18
            01-NOV-13          30-DEC-23          01-JAN-14          31-DEC-18

            01-NOV-13          31-DEC-23          01-JAN-19          31-DEC-23
            01-NOV-13          01-JAN-24          01-JAN-19          31-DEC-23
            */
            #endregion

            foreach (var test in list)
            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(test.StartWindow, Is.EqualTo(result.Item1));
                Assert.That(test.EndWindow, Is.EqualTo(result.Item2));
            }

        }

        /// <summary>
        /// TLPC validate 2 year window formula
        /// </summary>
        [Test]
        //[WorkItem(73194)]
        public void TLPCValidate2yearWindow()
        {

            IList<DatesTestValues> list = new List<DatesTestValues>();

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2016, 12, 31), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2017, 12, 30), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2018, 12, 31), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2019, 01, 01), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 30), StartWindow = new DateTime(2016, 11, 01), EndWindow = new DateTime(2018, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2020, 12, 31), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2020, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2016, 11, 01), CheckDate = new DateTime(2021, 01, 21), StartWindow = new DateTime(2019, 01, 01), EndWindow = new DateTime(2020, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2015, 12, 31), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2015, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2016, 01, 01), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2015, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 30), StartWindow = new DateTime(2014, 01, 01), EndWindow = new DateTime(2015, 12, 31) });

            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2017, 12, 31), StartWindow = new DateTime(2016, 01, 01), EndWindow = new DateTime(2017, 12, 31) });
            list.Add(new DatesTestValues() { EarliestCertDate = new DateTime(2013, 11, 01), CheckDate = new DateTime(2018, 01, 01), StartWindow = new DateTime(2016, 01, 01), EndWindow = new DateTime(2017, 12, 31) });

            #region 2YearWindowsDates
            /*
            EARLIESTCERT       CHECKDATE          START2WINDOW       END2WINDOW
            ------------------ ------------------ ------------------ ------------------
            01-NOV-16          31-DEC-16          01-NOV-16          31-DEC-18
            01-NOV-16          30-DEC-17          01-NOV-16          31-DEC-18
            01-NOV-16          31-DEC-18          01-NOV-16          31-DEC-18
            01-NOV-16          01-JAN-19          01-NOV-16          31-DEC-18
            01-NOV-16          30-DEC-20          01-NOV-16          31-DEC-18

            01-NOV-16          31-DEC-20          01-JAN-19          31-DEC-20
            01-NOV-16          01-JAN-21          01-JAN-19          31-DEC-20

            01-NOV-13          31-DEC-15          01-JAN-14          31-DEC-15
            01-NOV-13          01-JAN-16          01-JAN-14          31-DEC-15
            01-NOV-13          30-DEC-17          01-JAN-14          31-DEC-15

            01-NOV-13          31-DEC-17          01-JAN-16          31-DEC-17
            01-NOV-13          01-JAN-18          01-JAN-16          31-DEC-17
            */
            #endregion

            foreach (var test in list)
            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.TwoYearLookBack);

                Assert.That(test.StartWindow, Is.EqualTo(result.Item1));
                Assert.That(test.EndWindow, Is.EqualTo(result.Item2));
            }

        }

        public class DatesTestValues
        {
            internal DateTime EarliestCertDate { get; set; }
            internal DateTime CheckDate { get; set; }
            internal DateTime StartWindow { get; set; }
            internal DateTime EndWindow { get; set; }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateEaliestCertifaceSameYearTest()
        {
            DatesTestValues test = new DatesTestValues(){ EarliestCertDate = new DateTime(2017, 11, 01),
                                             CheckDate = new DateTime(2017, 12, 31),
                                             StartWindow = new DateTime(2017, 11, 1),
                                             EndWindow = new DateTime(2022, 12, 31)
                                            };
           

        
            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateIsOnNextYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2018, 12, 31),
                StartWindow = new DateTime(2017, 11, 1),
                EndWindow = new DateTime(2022, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateAfterFourYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2021, 12, 31),
                StartWindow = new DateTime(2017, 11, 1),
                EndWindow = new DateTime(2022, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }


        [Test]
        public void ComputeMOCLookBackWindowCheckDateAfterSixYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2023, 12, 31),
                StartWindow = new DateTime(2017, 11, 1),
                EndWindow = new DateTime(2022, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateAfterNineYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2026, 12, 31),
                StartWindow = new DateTime(2017, 11, 1),
                EndWindow = new DateTime(2022, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateAfterTenYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2027, 12, 31),
                StartWindow = new DateTime(2023, 1, 1),
                EndWindow = new DateTime(2027, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void ComputeMOCLookBackWindowCheckDateAfterElevenYearThanEaliestCertifaceYearTest()
        {
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 11, 01),
                CheckDate = new DateTime(2028, 1, 1),
                StartWindow = new DateTime(2023, 1, 1),
                EndWindow = new DateTime(2027, 12, 31)
            };



            {
                var result = Interservice.Shared.ProgramRulesHelpers.ComputeLookBackWindow(test.EarliestCertDate,
                                                                       test.CheckDate,
                                                                       WindowsIntervalType.FiveYearLookBack);

                Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
                Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            }

        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_First_Cert_Pre_2014_And_LastLookbackDate_Is_2018()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2013, 11, 01),
                CheckDate = new DateTime(2018, 12, 31),
                StartWindow = new DateTime(2019, 1, 1),
                EndWindow = new DateTime(2023, 12, 31)
            };

            //ACT
            var result = 
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    test.CheckDate,
                    WindowsIntervalType.FiveYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(2));
        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_First_Cert_Post_2014_And_LastLookbackDate_Is_2020()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2015, 11, 01),
                CheckDate = new DateTime(2020, 12, 31), //Lookback date - falls at end of first window
                StartWindow = new DateTime(2021, 1, 1),
                EndWindow = new DateTime(2025, 12, 31)
            };

            /*
             * Windows should be:
             * 11/1/2015 - 12/31/2020
             * 1/1/2021 - 12/31/2025
             * 1/1/2026 - 12/31/2030
            */

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    test.CheckDate,
                    WindowsIntervalType.FiveYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(2));
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.WorkItem(143323)]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_First_Cert_Is_2017_And_LastLookbackDate_Is_2018()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2017, 8, 16),
                CheckDate = new DateTime(2018, 12, 31), //Lookback date

                //We're expecting the window to be their first lookback window
                StartWindow = new DateTime(2017, 8, 16), 
                EndWindow = new DateTime(2022, 12, 31) 
            };

            /*
             * Windows should be:
             * 8/16/2017 - 12/31/2022 (Current Window)
             * 1/1/2023 - 12/31/2027
             * 1/1/2028 - 12/31/2032
            */

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    test.CheckDate,
                    WindowsIntervalType.FiveYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(1)); //This should be their first window
        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_First_Cert_Pre_2014_And_No_Prior_Lookback()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2013, 11, 01),
                StartWindow = new DateTime(2014, 1, 1),
                EndWindow = new DateTime(2018, 12, 31)
            };

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    null,
                    WindowsIntervalType.FiveYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(1));
        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_2YearLookback_When_First_Cert_Pre_2014_And_LastLookbackDate_Is_2017()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2013, 11, 01),
                CheckDate = new DateTime(2017, 12, 31), //Lookback Date - falls at end of 2 year window
                StartWindow = new DateTime(2018, 1, 1),
                EndWindow = new DateTime(2019, 12, 31)
            };

            /*
             * Windows should be:
             * - 1/1/2014 - 12/31/2015
             * - 1/1/2016 - 12/31/2017
             * - 1/1/2018 - 12/31/2019
            */

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    test.CheckDate,
                    WindowsIntervalType.TwoYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(2));
        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_2YearLookback_When_First_Cert_Post_2014_And_LastLookbackDate_Is_2018()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2015, 11, 01),
                CheckDate = new DateTime(2018, 12, 31), //Lookback Date - falls within a 2 year window
                StartWindow = new DateTime(2018, 1, 1),
                EndWindow = new DateTime(2019, 12, 31)
            };

            /*
             * Windows should be:
             * 11/1/2015 - 12/31/2017
             * 1/1/2018 - 12/31/2019
             * 1/1/2020 - 12/31/2021
            */

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    test.CheckDate,
                    WindowsIntervalType.TwoYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(2));
        }

        [Test]
        public void UIComputeLookbackWindow_Should_Return_Correct_Values_For_2YearLookback_When_First_Cert_Pre_2014_And_No_Prior_Lookback()
        {
            //ARRANGE
            DatesTestValues test = new DatesTestValues()
            {
                EarliestCertDate = new DateTime(2013, 11, 01),
                StartWindow = new DateTime(2014, 1, 1),
                EndWindow = new DateTime(2015, 12, 31)
            };

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputeLookBackWindow(
                    test.EarliestCertDate,
                    null,
                    WindowsIntervalType.TwoYearLookBack);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(test.StartWindow));
            Assert.That(result.Item2, Is.EqualTo(test.EndWindow));
            Assert.That(result.Item3, Is.EqualTo(1));
        }

        [Test]
        public void UIComputePreviousLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_Previous_Window_Was_First_Window()
        {
            //ARRANGE
            var earliestCert = new DateTime(2013, 1, 1); 
            var firstWindowStart = new DateTime(2014, 1, 1);
            var firstWindowEnd = new DateTime(2018, 12, 31);
            var currentWindowStart = new DateTime(2019, 1, 1);
            var currentWindowEnd = new DateTime(2023, 12, 31);
            var interval = WindowsIntervalType.FiveYearLookBack;

            //ACT
            var result = 
                Interservice.Shared.ProgramRulesHelpers.UIComputePreviousLookbackWindow(
                    earliestCert, currentWindowStart, currentWindowEnd, interval);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(firstWindowStart), "Start Date not as expected");
            Assert.That(result.Item2, Is.EqualTo(firstWindowEnd), "End Date not as expected");
        }

        [Test]
        public void UIComputePreviousLookbackWindow_Should_Return_Correct_Values_For_5YearLookback_When_Previous_Window_Was_Not_First_Window()
        {
            //ARRANGE
            var earliestCert = new DateTime(2013, 1, 1);
            var firstWindowStart = new DateTime(2014, 1, 1);
            var firstWindowEnd = new DateTime(2018, 12, 31);
            var previousWindowStart = new DateTime(2019, 1, 1);
            var previousWindowEnd = new DateTime(2023, 12, 31);
            var currentWindowStart = new DateTime(2024, 1, 1);
            var currentWindowEnd = new DateTime(2028, 12, 31);
            var interval = WindowsIntervalType.FiveYearLookBack;

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputePreviousLookbackWindow(
                    earliestCert, currentWindowStart, currentWindowEnd, interval);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(previousWindowStart), "Start Date not as expected");
            Assert.That(result.Item2, Is.EqualTo(previousWindowEnd), "End Date not as expected");
        }

        [Test]
        public void UIComputePreviousLookbackWindow_Should_Return_Correct_Values_For_2YearLookback_When_Previous_Window_Was_First_Window()
        {
            //ARRANGE
            var earliestCert = new DateTime(2013, 1, 1);
            var firstWindowStart = new DateTime(2014, 1, 1);
            var firstWindowEnd = new DateTime(2015, 12, 31);
            var currentWindowStart = new DateTime(2016, 1, 1);
            var currentWindowEnd = new DateTime(2017, 12, 31);
            var interval = WindowsIntervalType.TwoYearLookBack;

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputePreviousLookbackWindow(
                    earliestCert, currentWindowStart, currentWindowEnd, interval);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(firstWindowStart), "Start Date not as expected");
            Assert.That(result.Item2, Is.EqualTo(firstWindowEnd), "End Date not as expected");
        }

        [Test]
        public void UIComputePreviousLookbackWindow_Should_Return_Correct_Values_For_2YearLookback_When_Previous_Window_Was_Not_First_Window()
        {
            //ARRANGE
            var earliestCert = new DateTime(2013, 1, 1);
            var firstWindowStart = new DateTime(2014, 1, 1);
            var firstWindowEnd = new DateTime(2015, 12, 31);
            var previousWindowStart = new DateTime(2016, 1, 1);
            var previousWindowEnd = new DateTime(2017, 12, 31);
            var currentWindowStart = new DateTime(2018, 1, 1);
            var currentWindowEnd = new DateTime(2019, 12, 31);
            var interval = WindowsIntervalType.TwoYearLookBack;

            //ACT
            var result =
                Interservice.Shared.ProgramRulesHelpers.UIComputePreviousLookbackWindow(
                    earliestCert, currentWindowStart, currentWindowEnd, interval);

            //ASSERT
            Assert.That(result.Item1, Is.EqualTo(previousWindowStart), "Start Date not as expected");
            Assert.That(result.Item2, Is.EqualTo(previousWindowEnd), "End Date not as expected");
        }

    }
}
