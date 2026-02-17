using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Relational;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Repositories
{
    [Story(
        AsA = "service",
        IWant = "to be able to use the CredentialRepository",
        SoThat = "I can get Credential information"
        )]
    [TestFixture]
    public class CredentialRepositorySpec
    {
        [TestCase]
        [WorkItem(185606)]
        public void ShouldGetExpiredCredentialsFromABIMandNotFromABIM()
        {
            new ShouldGetExpiredCredentialsFromABIMandNotFromABIMScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void ShouldGetIdsOfCredentialsMarkedForDeselection()
        {
            new ShouldGetIdsOfCredentialsMarkedForDeselectionScenario().BDDfy();
        }

        private abstract class CredentialRepositoryScenarioBase
        {
            protected ICredentialRepository _sut;

            public CredentialRepositoryScenarioBase()
            {
                if (DependencyResolver.Container == null)
                    Startup.UseIoc();

                _sut = DependencyResolver.Container.GetInstance<ICredentialRepository>();
            }
        }

        private class ShouldGetExpiredCredentialsFromABIMandNotFromABIMScenario : CredentialRepositoryScenarioBase
        {
            private IEnumerable<Tuple<Guid, int>> _expiringCredsFromAllSources;
            private IEnumerable<Tuple<Guid, int>> _expiringCredsFromABIMOnly;
            private Exception exceptionFromGettingCredsFromAllSources;
            private Exception exceptionFromGettingCredsFromABIMOnly;

            public ShouldGetExpiredCredentialsFromABIMandNotFromABIMScenario()
            {
            }

            public void GivenThatIHaveAnInstanceOfTheCredentialRepository()
            {
                _sut.Should().NotBeNull();
            }

            public void WhenIQueryForExpiredCredentialsFromAllSources()
            {
                try
                {
                    _expiringCredsFromAllSources = _sut.GetExpiredCredentials(DateTime.Now, new DateTime(DateTime.Now.Year, 12, 31));
                }
                catch (Exception ex)
                {
                    exceptionFromGettingCredsFromAllSources = ex;
                }
            }

            public void AndWhenIQueryForExpiredSourcesFromABIMOnly()
            {
                try
                {
                    _expiringCredsFromABIMOnly = _sut.GetExpiredCredentials(DateTime.Now, new DateTime(DateTime.Now.Year, 12, 31), true);
                }
                catch (Exception ex)
                {
                    exceptionFromGettingCredsFromABIMOnly = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurredWhenGettingExpiringCredsFromAllSources()
            {
                exceptionFromGettingCredsFromAllSources.Should().BeNull();
            }

            public void AndNoExceptionShouldHaveOccurredWhenGettingExpiringCredsFromABIMOnly()
            {
                exceptionFromGettingCredsFromABIMOnly.Should().BeNull();
            }

            public void AndTheNumberOfCredsFromAllSourcesShouldBeGreaterThanThatOfABIMOnly()
            {
                _expiringCredsFromAllSources.Count().ShouldBeGreaterThan(_expiringCredsFromABIMOnly.Count());
                //A better test would be to go through the creds and verify none with a source of ABIM exist
                //in that result set. But the results here are IDs, not actual credentials.
                //We'd need to then retrieve EACH OF THOSE IDs for that test, so this will do. :/
            }
        }

        private class ShouldGetIdsOfCredentialsMarkedForDeselectionScenario : CredentialRepositoryScenarioBase
        {
            private IEnumerable<Tuple<Guid, Guid, string, bool>> _idsOfCredsMarkedForDeselection;
            private Exception _exception;

            public void GivenThatIHaveAnInstanceOfTheCredentialRepository()
            {
                _sut.Should().NotBeNull();
            }

            public void WhenIQueryForCredentialIdsMarkedForDeselection()
            {
                try
                {
                    _idsOfCredsMarkedForDeselection = _sut.GetInfoOfCredentialsMarkedForDeselection(new DateTime(DateTime.Now.Year, 2, 1));
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurred()
            {
                _exception.Should().BeNull();
            }

            public void AndIdsShouldNotBeNull()
            {
                _idsOfCredsMarkedForDeselection.Should().NotBeNull();
            }
        }
    }
}
