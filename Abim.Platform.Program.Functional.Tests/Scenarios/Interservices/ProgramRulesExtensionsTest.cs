using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Program.Testing.Setup.ResourceBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using Abim.Platform.Program.App.Extensions.Registration;

namespace Abim.Platform.Program.Tests.Scenarios.Interservices
{
    [TestFixture]
    public class ProgramRulesExtensionsTest
    {
        private RegistrationResourceBuilder _regBuilder;
        private ExamResultResourceBuilder _examResultBuilder;

        [SetUp]
        public void Setup()
        {
            _regBuilder = new RegistrationResourceBuilder();
            _examResultBuilder = new ExamResultResourceBuilder();
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultPassWhenCriteriaMetAndActualResultWasFail()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Fail).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Pass);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultPassWhenCriteriaMetAndActualResultWasIndeterminate()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Indeterminate).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Pass);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultPassWhenCriteriaMetAndActualResultWasIncomplete()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Incomplete).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Pass);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultPassWhenCriteriaMetAndActualResultWasUnableToTest()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.UnableToTest).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Pass);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultIsActualResultWhenNotNoConseqeunce()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(false)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Incomplete).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Incomplete);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultIsActualResultWhenResultNotOneOfTheAllowedPassingTypes()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.NotScored).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.NotScored);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultIsActualResultWhenAdminYearGreaterThanDueDateYear()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2019)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Incomplete).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), false);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Incomplete);
        }

        [Test]
        [WorkItem(134151)]
        public void EffectiveExamResultIsActualResultWhenConsecutiveKCIPassRequired()
        {
            //ARRANGE
            var reg =
                _regBuilder
                    .WithAdministrationYear(2018)
                    .WithNoConsequence(true)
                    .WithExamResult(_examResultBuilder.WithResult(ExamResultType.Incomplete).Build())
                    .Build();

            //ACT
            var result = reg.GetEffectiveExamResult(new DateTime(2018, 1, 1), true);

            //ASSERT
            NUnit.Framework.Assert.AreEqual(result, ExamResultType.Incomplete);
        }
    }
}
