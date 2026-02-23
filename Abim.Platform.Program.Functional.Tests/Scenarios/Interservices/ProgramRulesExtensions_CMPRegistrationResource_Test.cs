using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Abim.Platform.Program.App.Extensions.Registration;

namespace Abim.Enterprise.Core.Interservice.Tests
{
    [TestFixture]
    public class ProgramRulesExtensions_CMPRegistrationResource_Test
    {
        private CMPRegistrationResourceBuilder _regBuilder;
        private CMPExamSummaryResourceBuilder _examBuilder;

        [SetUp]
        public void Setup()
        {
            _regBuilder = new CMPRegistrationResourceBuilder();
            _examBuilder = new CMPExamSummaryResourceBuilder();
        }

        [Test]
        public void IfPassExamInRangeReturnsTrueWhenAllCriteriaMetNoTLCPIssuanceDate()
        {
            //ARRANGE
            Guid certId = Guid.NewGuid();
            var reg =
                _regBuilder
                    .WithTestDate(new DateTime(2019, 6, 20))
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(_examBuilder.WithCertificationId(certId).Build())
                    .Build();

            var sut = new List<CMPRegistrationResource>(1) { reg };

            //ACT / ASSERT
            Assert.IsTrue(sut.IfPassExamInRange(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), certId, null));
        }

        [Test]
        public void IfPassExamInRangeReturnsTrueWhenAllCriteriaMetWithValidTLCPIssuanceDate()
        {
            //ARRANGE
            Guid certId = Guid.NewGuid();
            var reg =
                _regBuilder
                    .WithTestDate(new DateTime(2019, 6, 20))
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(_examBuilder.WithCertificationId(certId).Build())
                    .Build();

            var sut = new List<CMPRegistrationResource>(1) { reg };

            //ACT / ASSERT
            Assert.IsTrue(sut.IfPassExamInRange(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), certId, new DateTime(2019, 1, 1)));
        }

        [Test]
        public void IfPassExamInRangeReturnsFalseWhenTestDateIsNotInRange()
        {
            //ARRANGE
            Guid certId = Guid.NewGuid();
            var reg =
                _regBuilder
                    .WithTestDate(new DateTime(2020, 6, 20))
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(_examBuilder.WithCertificationId(certId).Build())
                    .Build();

            var sut = new List<CMPRegistrationResource>(1) { reg };

            //ACT / ASSERT
            Assert.IsFalse(sut.IfPassExamInRange(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), certId, null));
        }

        [Test]
        public void IfPassExamInRangeReturnsFalseWhenExamResultIsNotPass()
        {
            //ARRANGE
            Guid certId = Guid.NewGuid();
            var reg =
                _regBuilder
                    .WithTestDate(new DateTime(2019, 6, 20))
                    .WithExamResult(ExamResultType.Fail)
                    .WithCMPExam(_examBuilder.WithCertificationId(certId).Build())
                    .Build();

            var sut = new List<CMPRegistrationResource>(1) { reg };

            //ACT / ASSERT
            Assert.IsFalse(sut.IfPassExamInRange(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), certId, null));
        }

        [Test]
        public void IfPassExamInRangeReturnsFalseWhenCertIdIsNotTheSame()
        {
            //ARRANGE
            Guid certId = Guid.NewGuid();
            var reg =
                _regBuilder
                    .WithTestDate(new DateTime(2019, 6, 20))
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(_examBuilder.WithCertificationId(certId).Build())
                    .Build();

            var sut = new List<CMPRegistrationResource>(1) { reg };

            //ACT / ASSERT
            Assert.IsFalse(sut.IfPassExamInRange(new DateTime(2019, 1, 1), new DateTime(2019, 12, 31), Guid.NewGuid(), null));
        }
    }
}
