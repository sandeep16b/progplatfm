using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Testing.Setup.ResourceBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using NUnit.Framework;
using Shouldly;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Util
{
    [TestFixture]
    public class RegistrationDataSpec
    {
        [Test]
        public void Should_Assign_Values_Correctly_From_RegistrationResource()
        {
            new Should_Assign_Values_Correctly_From_RegistrationResource_Scenario().BDDfy();
        }

        [Test]
        public void Should_Assign_Values_Correctly_From_CMPRegistrationResource()
        {
            new Should_Assign_Values_Correctly_From_CMPRegistrationResource_Scenario().BDDfy();
        }

        [Test]
        public void Should_Allow_NoShow_Result_From_CMPRegistrationResource()
        {
            new Should_Allow_NoShow_Result_From_CMPRegistrationResource_Scenario().BDDfy();
        }

        [Test]
        public void Should_Get_Expected_Value_From_GetExamType_For_RegistrationResource()
        {
            new Should_Get_Expected_Value_From_GetExamType_For_RegistrationResource_Scenario().BDDfy();
        }

        [Test]
        public void Should_Get_Expected_Value_From_GetExamType_For_CMPRegistrationResource()
        {
            new Should_Get_Expected_Value_From_GetExamType_For_CMPRegistrationResource_Scenario().BDDfy();
        }

        [Test]
        public void Should_Return_Correct_Exam_Type_String_For_MOC()
        {
            new Should_Return_Correct_Exam_Type_String_For_MOC_Scenario().BDDfy();
        }

        [Test]
        public void Should_Return_Correct_Exam_Type_String_For_KCI()
        {
            new Should_Return_Correct_Exam_Type_String_For_KCI_Scenario().BDDfy();
        }

        [Test]
        public void Should_Return_Correct_Exam_Type_String_For_CERT()
        {
            new Should_Return_Correct_Exam_Type_String_For_CERT_Scenario().BDDfy();
        }

        [Test]
        public void Should_Return_Correct_Exam_Type_String_For_CMPRegistrationResource()
        {
            new Should_Return_Correct_Exam_Type_String_For_CMPRegistrationResource_Scenario().BDDfy();
        }


        #region Scenarios
        private abstract class RegistrationDataScenario
        {
            protected RegistrationData _sut;
            protected Exception _caughtException;
            protected RegistrationResource _regResource;
            protected RegistrationResourceBuilder _regBuilder;
            protected ExamResultResourceBuilder _examResultBuilder;

            public void Setup()
            {
                _regBuilder = new RegistrationResourceBuilder();
            }

            public void WhenIInstantiateTheRegistrationDataObject()
            {
                try
                {
                    _sut = new RegistrationData(_regResource);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBe(null);
            }
        }

        private abstract class RegistrationDataFromCMPScenario
        {
            protected CMPRegistrationResource _resource;
            protected CMPRegistrationResourceBuilder _cmpRegBuilder;
            protected CMPExamSummaryResourceBuilder _cmpExamBuilder;
            protected RegistrationData _sut;
            protected Exception _caughtException;

            public void Setup()
            {
                _cmpRegBuilder = new CMPRegistrationResourceBuilder();
                _cmpExamBuilder = new CMPExamSummaryResourceBuilder();
            }

            public void WhenIInstantiateTheRegistrationDataObject()
            {
                try
                {
                    _sut = new RegistrationData(_resource);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBe(null);
            }
        }

        private abstract class GetExamTypeScenario : RegistrationDataScenario
        {
            protected string _examTypeString;

            public void AndWhenICallGetExamType()
            {
                _examTypeString = _sut.GetExamType();
            }
        }

        private abstract class GetExamTypeForCMPScenario : RegistrationDataFromCMPScenario
        {
            protected string _examTypeString;

            public void AndWhenICallGetExamType()
            {
                _examTypeString = _sut.GetExamType();
            }
        }

        private class Should_Assign_Values_Correctly_From_RegistrationResource_Scenario : RegistrationDataScenario
        {
            public void GivenThatIHaveARegistrationResource()
            {
                _regResource =
                    _regBuilder
                        .WithId(Guid.NewGuid())
                        .WithAdministrationDate(new DateTime(2019, 3, 4))
                        .WithAdministrationYear(2019)
                        .WithCertificationId(Guid.NewGuid())
                        .WithExamResult(ExamResultType.Pass)
                        .WithExamType(ExamType.Moc)
                        .WithMemberId(Guid.NewGuid())
                        .WithSeat(new DateTime(2019, 3, 5))
                        .WithNoConsequence(false)
                        .WithPhyisicianIsAbim(true)
                        .Build();
            }

            public void AndAllOfThePropertiesShouldBeMappedCorrectly()
            {
                Assert.AreEqual(_regResource.Id, _sut.Id);
                Assert.AreEqual(_regResource.AdministrationDate, _sut.AdministrationDate);
                Assert.AreEqual(_regResource.AdministrationYear, _sut.AdministrationYear);
                Assert.AreEqual(_regResource.CertificationId, _sut.CertificationId);
                Assert.AreEqual(_regResource.ExamResult, _sut.ExamResult);
                Assert.IsFalse(_sut.IsCmp);
                Assert.IsFalse(_sut.IsKci);
                Assert.IsTrue(_sut.IsMoc);
                Assert.AreEqual(_regResource.MemberId, _sut.MemberId);
                Assert.AreEqual(_regResource.Seats[0].SeatDate, _sut.MinSeatOrDeliveryDate);
                Assert.AreEqual(_regResource.PhysicianIsAbim, _sut.PhysicianIsAbim);
            }
        }

        private class Should_Assign_Values_Correctly_From_CMPRegistrationResource_Scenario : RegistrationDataFromCMPScenario
        {
            public void GivenThatIHaveACMPRegistrationResource()
            {
                _resource =
                    _cmpRegBuilder
                        .WithId(Guid.NewGuid())
                        .WithMemberId(Guid.NewGuid())
                        .WithExamResult(ExamResultType.Pass)
                        .WithTestDate(DateTime.Now)
                        .WithPhysicianIsAbim(true)
                        .WithCMPExam(_cmpExamBuilder.WithNoConsequenceYear(DateTime.Now.Year).WithCertificationId(Guid.NewGuid()).Build())
                        .WithOnHold(true)
                        .Build();
            }

            public void AndAllOfThePropertiesShouldBeMappedCorrectly()
            {
                Assert.AreEqual(_resource.Id, _sut.Id);
                Assert.AreEqual(_resource.TestDate, _sut.AdministrationDate);
                Assert.AreEqual(_resource.TestDate.Year, _sut.AdministrationYear);
                Assert.AreEqual(_resource.CMPExam.CertificationId, _sut.CertificationId);
                Assert.AreEqual(_resource.ExamResult.Value, _sut.ExamResult.Result.Value);
                Assert.IsTrue(_sut.IsCmp);
                Assert.IsFalse(_sut.IsKci);
                Assert.IsFalse(_sut.IsMoc);
                Assert.AreEqual(_resource.MemberId, _sut.MemberId);
                Assert.AreEqual(_resource.TestDate, _sut.MinSeatOrDeliveryDate);
                Assert.AreEqual(_resource.PhysicianIsAbim, _sut.PhysicianIsAbim);
                Assert.AreEqual(_resource.OnHold, _sut.OnHold);
            }
        }

        private class Should_Allow_NoShow_Result_From_CMPRegistrationResource_Scenario : RegistrationDataFromCMPScenario
        {
            public void GivenThatIHaveACMPRegistrationResource()
            {
                _resource =
                    _cmpRegBuilder
                        .WithId(Guid.NewGuid())
                        .WithMemberId(Guid.NewGuid())
                        .WithExamResult(ExamResultType.NoShow)
                        .WithTestDate(DateTime.Now)
                        .WithPhysicianIsAbim(true)
                        .WithCMPExam(_cmpExamBuilder.WithNoConsequenceYear(DateTime.Now.Year).WithCertificationId(Guid.NewGuid()).Build())
                        .Build();
            }

            public void AndExamResultShouldBeMappedCorrectlyAsNoShow()
            {
                Assert.AreEqual(_resource.ExamResult.Value, _sut.ExamResult.Result.Value);
            }
        }
        
        private class Should_Return_Correct_Exam_Type_String_For_MOC_Scenario : GetExamTypeScenario
        {
            public void GivenThatIHaveARegistrationResourceForAnMOCExam()
            {
                _regResource =
                    _regBuilder
                        .WithExamType(ExamType.Moc)
                        .Build();
            }

            public void AndResultShouldBeMOC()
            {
                Assert.AreEqual("MOC", _examTypeString);
            }
        }

        private class Should_Return_Correct_Exam_Type_String_For_KCI_Scenario : GetExamTypeScenario
        {
            public void GivenThatIHaveARegistrationResourceForAKCIExam()
            {
                _regResource =
                    _regBuilder
                        .WithExamType(ExamType.Kci)
                        .Build();
            }

            public void AndResultShouldBeKCI()
            {
                Assert.AreEqual("KCI", _examTypeString);
            }
        }

        private class Should_Get_Expected_Value_From_GetExamType_For_RegistrationResource_Scenario : RegistrationDataScenario
        {
            public void GivenThatIHaveARegistrationResource()
            {
                _regResource =
                    _regBuilder
                        .WithExamType(ExamType.Moc)
                        .Build();
            }

            public void AndTheResultShouldBeCorrect()
            {
                Assert.AreEqual(_regResource.ExamType.Value.ToUpper(), _sut.GetExamType());
            }
        }

        private class Should_Get_Expected_Value_From_GetExamType_For_CMPRegistrationResource_Scenario : RegistrationDataFromCMPScenario
        {
            public void GivenThatIHaveACMPRegistrationResource()
            {
                _resource =
                    _cmpRegBuilder
                        .WithId(Guid.NewGuid())
                        .WithMemberId(Guid.NewGuid())
                        .WithExamResult(ExamResultType.Pass)
                        .WithTestDate(DateTime.Now)
                        .WithPhysicianIsAbim(true)
                        .WithCMPExam(_cmpExamBuilder.WithNoConsequenceYear(DateTime.Now.Year).WithCertificationId(Guid.NewGuid()).Build())
                        .Build();
            }

            public void AndTheResultShouldBeCorrect()
            {
                Assert.AreEqual("ACC", _sut.GetExamType());
            }
        }

        private class Should_Return_Correct_Exam_Type_String_For_CERT_Scenario : GetExamTypeScenario
        {
            public void GivenThatIHaveARegistrationResourceForAnInitialCertExam()
            {
                _regResource =
                    _regBuilder
                        .WithExamType(ExamType.Cert)
                        .Build();
            }

            public void AndResultShouldBeCERT()
            {
                Assert.AreEqual("CERT", _examTypeString);
            }

        }

        private class Should_Return_Correct_Exam_Type_String_For_CMPRegistrationResource_Scenario : GetExamTypeForCMPScenario
        {
            public void GivenThatIHaveACMPRegistrationResource()
            {
                _resource = _cmpRegBuilder
                   .WithId(Guid.NewGuid())
                   .WithMemberId(Guid.NewGuid())
                   .WithExamResult(ExamResultType.Pass)
                   .WithTestDate(DateTime.Now)
                   .WithPhysicianIsAbim(true)
                   .WithCMPExam(_cmpExamBuilder.WithNoConsequenceYear(DateTime.Now.Year).WithCertificationId(Guid.NewGuid()).Build())
                   .Build();
            }

            public void AndResultShouldBeACC()
            {
                Assert.AreEqual("ACC", _examTypeString);
            }
        }

        #endregion Scenarios
    }

}