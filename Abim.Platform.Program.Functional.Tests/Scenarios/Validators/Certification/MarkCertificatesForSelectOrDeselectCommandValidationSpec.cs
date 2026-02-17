using System;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using TestStack.BDDfy;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.WebApi.Authentication;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process deselecting certificates",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class MarkCertificatesForSelectOrDeselectCommandValidationSpec
    {
        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCommandIsValid()
        {
            new PassesValidationWhenCommandIsValidScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenBothCredentialIdListsAreEmpty()
        {
            new FailsValidationWhenBothCredentialIdListsAreEmptyScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenMemberIdIsEmpty()
        {
            new FailsValidationWhenMemberIdIsEmptyScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenBothCredentialIdListsAreNull()
        {
            new FailsValidationWhenBothCredentialIdListsAreNullScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenCredentialIdsForDeselectContainsEmptyGuid()
        {
            new FailsValidationWhenCredentialIdsForDeselectContainsEmptyGuidScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenCredentialIdsForSelectContainsEmptyGuid()
        {
            new FailsValidationWhenCredentialIdsForSelectContainsEmptyGuidScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCredentialIdsForDeselectIsEmptyButCredentialIdsForSelectAreSpecified()
        {
            new PassesValidationWhenCredentialIdsForDeselectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCredentialIdsForDeselectIsNullButCredentialIdsForSelectAreSpecified()
        {
            new PassesValidationWhenCredentialIdsForDeselectIsNullButCredentialIdsForSelectAreSpecifiedScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCredentialIdsForSelectIsEmptyButCredentialIdsForDeselectAreSpecified()
        {
            new PassesValidationWhenCredentialIdsForSelectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCredentialIdsForSelectIsNullButCredentialIdsForDeselectAreSpecified()
        {
            new PassesValidationWhenCredentialIdsForSelectIsNullButCredentialIdsForSelectAreSpecifiedScenario().BDDfy();
        }

        #region Scenario Base Classes
        private abstract class MarkCertificatesForDeSelectCommandValidationScenarioBase
        {
            protected MarkCertificatesForSelectOrDeselectCommand command;
            protected MarkCertificatesForSelectOrDeselectCommandValidator sut;
            protected CommandBuilder<MarkCertificatesForSelectOrDeselectCommand> builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public MarkCertificatesForDeSelectCommandValidationScenarioBase()
            {
                command = new MarkCertificatesForSelectOrDeselectCommand();
                sut = new MarkCertificatesForSelectOrDeselectCommandValidator();
                builder = new CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>();
            }
        }

        private abstract class ValidationFailedScenario : MarkCertificatesForDeSelectCommandValidationScenarioBase
        {
            protected string expectedValidationErrorMessage;

            public ValidationFailedScenario(string expectedValidationMessage)
            {
                expectedValidationErrorMessage = expectedValidationMessage;
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg =>
                        msg.ErrorMessage.Contains(expectedValidationErrorMessage));
            }
        }
        #endregion Scenario Base Classes

        #region Scenarios

        #region Passes When Valid Scenario
        private class PassesValidationWhenCommandIsValidScenario : MarkCertificatesForDeSelectCommandValidationScenarioBase
        {
            public PassesValidationWhenCommandIsValidScenario()
            { }

            public virtual void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid>{ Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHavePassed()
            {
                validationResult.IsValid.Should().BeTrue();
                validationResult.Errors.Should().BeEmpty();
            }
        }
        #endregion Passes When Valid Scenario

        #region Fails When Both Credential Id Lists Are Empty
        private class FailsValidationWhenBothCredentialIdListsAreEmptyScenario : ValidationFailedScenario
        {
            public FailsValidationWhenBothCredentialIdListsAreEmptyScenario() : base("CredentialIdsForSelect or CredentialIdsForDeselect are required.")
            { }

            public void GivenTheCommandHasAnEmptyCredentialIdsValue()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid>(0))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Fails When Both Credential Id Lists Are Empty

        #region Fails When Both Credential Id Lists Are Null
        private class FailsValidationWhenBothCredentialIdListsAreNullScenario : ValidationFailedScenario
        {
            public FailsValidationWhenBothCredentialIdListsAreNullScenario() : base("CredentialIdsForSelect or CredentialIdsForDeselect are required.")
            { }

            public void GivenTheCommandHasANullCredentialIdsValue()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = null)
                    .With(cmd => cmd.CredentialIdsForSelect = null)
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Fails When Both Credential Id Lists Are Null

        #region Fails When CredentialIdsForDeselect Contains Empty Guid
        private class FailsValidationWhenCredentialIdsForDeselectContainsEmptyGuidScenario : ValidationFailedScenario
        {
            public FailsValidationWhenCredentialIdsForDeselectContainsEmptyGuidScenario() : base("All CredentialIdsForDeselect must be valid, non-default GUIDs.")
            { }

            public void GivenTheCommandHasAnEmptyGuidForCredentialIdForDeselect()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid> { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Fails When CredentialIdsForDeselect Contains Empty Guid

        #region Fails When CredentialIdsForSelect Contains Empty Guid
        private class FailsValidationWhenCredentialIdsForSelectContainsEmptyGuidScenario : ValidationFailedScenario
        {
            public FailsValidationWhenCredentialIdsForSelectContainsEmptyGuidScenario() : base("All CredentialIdsForSelect must be valid, non-default GUIDs.")
            { }

            public void GivenTheCommandHasAnEmptyGuidForCredentialIdForDeselect()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForSelect = new List<Guid> { Guid.NewGuid(), Guid.Empty, Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "avanhalen" })
                    .Build();
            }
        }
        #endregion Fails When CredentialIdsForSelect Contains Empty Guid

        #region Passes When CredentialIdsForDeselect Is Empty But CredentialIdsForSelect Are Specified
        private class PassesValidationWhenCredentialIdsForDeselectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario : PassesValidationWhenCommandIsValidScenario
        {
            public PassesValidationWhenCredentialIdsForDeselectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario()
            { }

            public override void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid>(0))
                    .With(cmd => cmd.CredentialIdsForSelect = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Passes When CredentialIdsForDeselect Is Empty But CredentialIdsForSelect Are Specified

        #region Passes When CredentialIdsForDeselect Is Null But CredentialIdsForSelect Are Specified
        private class PassesValidationWhenCredentialIdsForDeselectIsNullButCredentialIdsForSelectAreSpecifiedScenario : PassesValidationWhenCommandIsValidScenario
        {
            public PassesValidationWhenCredentialIdsForDeselectIsNullButCredentialIdsForSelectAreSpecifiedScenario()
            { }

            public override void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForDeselect = null)
                    .With(cmd => cmd.CredentialIdsForSelect = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "manthony" })
                    .Build();
            }
        }
        #endregion Passes When CredentialIdsForDeselect Is Null But CredentialIdsForSelect Are Specified

        #region Passes When CredentialIdsForSelect Is Empty But CredentialIdsForSelect Are Specified
        private class PassesValidationWhenCredentialIdsForSelectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario : PassesValidationWhenCommandIsValidScenario
        {
            public PassesValidationWhenCredentialIdsForSelectIsEmptyButCredentialIdsForSelectAreSpecifiedScenario()
            { }

            public override void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForSelect = new List<Guid>(0))
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Passes When CredentialIdsForDeselect Is Empty But CredentialIdsForSelect Are Specified

        #region Passes When CredentialIdsForSelect Is Null But CredentialIdsForSelect Are Specified
        private class PassesValidationWhenCredentialIdsForSelectIsNullButCredentialIdsForSelectAreSpecifiedScenario : PassesValidationWhenCommandIsValidScenario
        {
            public PassesValidationWhenCredentialIdsForSelectIsNullButCredentialIdsForSelectAreSpecifiedScenario()
            { }

            public override void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialIdsForSelect = null)
                    .With(cmd => cmd.CredentialIdsForDeselect = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() })
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Passes When CredentialIdsForDeselect Is Null But CredentialIdsForSelect Are Specified

        #region Fails When MemberId is Empty
        private class FailsValidationWhenMemberIdIsEmptyScenario : ValidationFailedScenario
        {
            public FailsValidationWhenMemberIdIsEmptyScenario() : base("MemberId is required")
            { }

            public void GivenTheCommandHasAnEmptyMemberIdValue()
            {
                command = builder
                    .With(cmd => cmd.MemberId = Guid.Empty)
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "alex" })
                    .Build();
            }
        }
        #endregion Fails When MemberId is Empty

        #endregion Scenarios
    }
}
