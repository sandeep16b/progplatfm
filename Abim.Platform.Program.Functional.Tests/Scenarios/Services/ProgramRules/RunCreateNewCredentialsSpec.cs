using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can create new FPHM credentials when initial attestation is received"
      )]
    [TestFixture]
    public class RunCreateNewCredentialsSpec
    {
        [Test]
        [WorkItem(135988)]
        public void Should_Set_Exam_Due_Dates_And_Consecutive_KCI_Pass_Value_Appropriately()
        {
            new ShouldSetExamDueDatesAndConsecutiveKCIPassValueAppropriately().BDDfy();
        }

        #region Scenarios
        /// <summary>
        /// Base class for this file
        /// </summary>
        /// <seealso cref="CredentialServiceScenario" />
        public abstract class RunCreateNewCredentialsSpecScenario : ProgramRulesServiceScenario
        {
            protected Guid _memberId;
            protected DateTime _processingDate;
            protected ProgramRulesService ProgramRulesService { get; set; }
            //protected Exception ExceptionCaught { get; set; }
        }

        private class ShouldSetExamDueDatesAndConsecutiveKCIPassValueAppropriately : RunCreateNewCredentialsSpecScenario
        {
            protected override void PreSetup()
            {
            }

            protected override void PostSetup()
            {
                Credential cred = null;
                Certification cert = Certification.Create(null, null, CertificationType.FocusPractice, null, null, null);
                cert.ExternalId = Guid.NewGuid();

                ProgramRulesService = Container.GetInstance<ProgramRulesService>();

                My<ICredentialService>()
                    .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), "HOSP"))
                    .Returns(cred);

                My<ICredentialService>()
                    .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), "IM"))
                    .Returns(Credential.Create(null, Guid.NewGuid(), CredentialType.General, PathwayType.KCI, null, null, null));

                My<ICredentialService>()
                    .Setup(x => x.Handle(It.IsAny<CreateCredentialCommand>()))
                    .Returns(new CreateCredentialCommandResult());

                My<ICertificationService>()
                    .Setup(x => x.GetByCode(It.IsAny<string>()))
                    .Returns(cert);
            }

            private void GivenIHaveAMemberIdAndProcessingDate()
            {
                _memberId = Guid.NewGuid();
                _processingDate = new DateTime(2018, 11, 7);
            }

            private void WhenICallTheMethod()
            {
                try
                {
                    ProgramRulesService.RunCreateNewCredentials(_memberId, _processingDate);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveOccurred()
            {
                ExceptionCaught.ShouldBeNull();
            }

            private void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                My<ICredentialService>()
                    .Verify(x => 
                        x.Handle(It.Is<CreateCredentialCommand>(cmd => 
                            cmd.ExamDueDate == DateTime.MaxValue
                            && !cmd.MOCExamDueDate.HasValue
                            && !cmd.KCIExamDueDate.HasValue
                            && !cmd.DisplayExamDueDate.HasValue
                            && !cmd.ConsecutiveKCIPassRequired)), Times.Once);
            }

        }
        #endregion Scenarios
    }
}
