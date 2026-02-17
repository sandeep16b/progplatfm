using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using FluentAssertions;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesIndividualRule
{
    [Story(
       AsA = "controller, background job, or bus consumer",
       IWant = "to be able to utilize the Program Rules service private method",
       SoThat = "it can return correct MaintenanceStatusType result"
   )]
    [TestFixture]
    public class DetermineMaintenanceStatusOnPassingInitialCertSpec
    {
        [Test]
        public void DetermineMaintenanceStatusFor_ACHD_ReturnOk()
        {
            new DetermineMaintenanceStatusFor_ACHD_ReturnOk_().BDDfy();
        }

        [Test]
        public void DetermineMaintenanceStatusFor_IM_ReturnOk()
        {
            new DetermineMaintenanceStatusFor_IM_ReturnOk_().BDDfy();
        }

        [Test]
        public void DetermineMaintenanceStatusFor_CARD_ReturnOk()
        {
            new DetermineMaintenanceStatusFor_CARD_ReturnOk_().BDDfy();
        }


        [Test]
        public void DetermineMaintenanceStatusFor_CARD_NoActivities_ReturnOk()
        {
            new DetermineMaintenanceStatusFor_CARD_NoActivities_ReturnOk_().BDDfy();
        }

        #region Common Spec Scenarios
        private abstract class DetermineMaintenanceStatusOnPassingInitialCertSpecScenario : ProgramRulesIndividualRulesScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();
            }
        }

        #endregion Scenarios

        #region Scenarios

        private class DetermineMaintenanceStatusFor_ACHD_ReturnOk_ : DetermineMaintenanceStatusOnPassingInitialCertSpecScenario
        {
            private string CertificationCode { get; set; }

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                base.PreSetup();

                // input parameters
                CertificationCode = "ACHD";
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("DetermineMaintenanceStatusOnPassingInitialCert_",
                                                                                    CertificationCode,
                                                                                    ProcessingDate);
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

            public void ThenResultShouldBe_Maintained()
            {
                ResultObject.Should().Be(MaintenanceStatusType.Maintained);
            }
        }

        private class DetermineMaintenanceStatusFor_IM_ReturnOk_ : DetermineMaintenanceStatusOnPassingInitialCertSpecScenario
        {
            private string CertificationCode { get; set; }
            private DateTime ProcessingDate { get; set; }

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                base.PreSetup();

                // input parameters
                CertificationCode = "IM";
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("DetermineMaintenanceStatusOnPassingInitialCert_",
                                                                                    CertificationCode,
                                                                                    ProcessingDate);
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

            public void ThenResultShouldBe_Maintained()
            {
                ResultObject.Should().Be(MaintenanceStatusType.Maintained);
            }
        }

        private class DetermineMaintenanceStatusFor_CARD_ReturnOk_ : DetermineMaintenanceStatusOnPassingInitialCertSpecScenario
        {
            private string CertificationCode { get; set; }

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                base.PreSetup();

                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 0.1m);

                // input parameters
                CertificationCode = "CARD";
                ProcessingDate = new DateTime(2018, 12, 31);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("DetermineMaintenanceStatusOnPassingInitialCert_",
                                                                                    CertificationCode,
                                                                                    ProcessingDate);
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

            public void ThenResultShouldBe_Maintained()
            {
                ResultObject.Should().Be(MaintenanceStatusType.Maintained);
            }
        }

        private class DetermineMaintenanceStatusFor_CARD_NoActivities_ReturnOk_ : DetermineMaintenanceStatusOnPassingInitialCertSpecScenario
        {
            private string CertificationCode { get; set; }

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                base.PreSetup();

                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                // input parameters
                CertificationCode = "CARD";
                ProcessingDate = new DateTime(2018, 12, 31);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("DetermineMaintenanceStatusOnPassingInitialCert_",
                                                                                    CertificationCode,
                                                                                    ProcessingDate);
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

            public void ThenResultShouldBe_NotMaintained()
            {
                ResultObject.Should().Be(MaintenanceStatusType.NotMaintained);
            }
        }
        #endregion
    }
}
