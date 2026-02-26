extern alias SharedOldServiceBus; 
using TestStack.BDDfy;
using NUnit.Framework;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Moq;
using System;
using Abim.Platform.Program.Relational;
using System.Collections.Generic;
using static Abim.Platform.Program.App.Util.Constants;
using System.Threading.Tasks;
using FluentAssertions;
using SharedOldServiceBus::Abim.Enterprise.Core.ServiceBus.Notification;
using System.Threading;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.MembershipClient;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Helper
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the HelperService",
        SoThat = "to prevent Earned MBM Letter trigger communication to be sent for Cosponsored only"
        )]
    [TestFixture]
    public class TriggeredCommunicationEarnedMBMLetterSpec
    {
        [Test]
        public void TriggeredCommunicationForEarnedMBMLetterNotSentForCosponsoredOnlyScenarioTest()
        {
            new TriggeredCommunicationForEarnedMBMLetterNotSentForCosponsoredOnlyScenario()
                .BDDfy("Triggered communication for earned MBM letter not sent for cosponsored only scenario.");
        }

        [Test]
        public void TriggeredCommunicationForEarnedMBMLetterSentForABIMUSDiplomatesTest()
        {
            new TriggeredCommunicationForEarnedMBMLetterSentForABIMUSDiplomates()
                .BDDfy("Triggered communication for earned MBM letter sent for ABIM US diplomates scenario.");
        }

        [Test]
        public void TriggeredCommunicationForEarnedMBMLetterSentForABIMNoneUSDiplomatesTest()
        {
            new TriggeredCommunicationForEarnedMBMLetterSentForABIMNoneUSDiplomates()
                .BDDfy("Triggered communication for earned MBM letter sent for ABIM US diplomates scenario.");
        }

        [Test]
        public void TriggeredCommunicationForEarnedMBMLetterSentForJointBoardDiplomatesTest()
        {
            new TriggeredCommunicationForEarnedMBMLetterSentForJointBoardDiplomates()
                .BDDfy("Triggered communication for earned MBM letter sent for joint board scenario.");
        }

        #region Scenarios

        #region Triggered communication for earned MBM letter NOT sent for cosponsored only scenario
        private class TriggeredCommunicationForEarnedMBMLetterNotSentForCosponsoredOnlyScenario
            : HelperServiceScenario
        {
            private Credential mockedCredential;
            private int mockedTotalCount;
            private Exception _caughtException;

            public void GivenTheDiplomateIsCosponsoredOnly()
            {
                // Mocked Co-sponsored Credential.
                mockedCredential = CredentialBuilder
                    .Build(RandomString.Build(), RandomString.Build());
                var mockedResponse = new List<Credential> { mockedCredential };
                mockedTotalCount = mockedResponse.Count;
                _credentialServiceMock.Setup(_ => _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount))
                    .Returns(mockedResponse)
                    .Verifiable("Credential service is expected to be called.");
            }

            public async Task WhenRequestIsMadeForEarnedMBMLetterTriggerCommunication()
            {
                try
                {
                    await _sut.TriggeredCommunication(mockedCredential, TriggeredCommunication.EarnedMBMCertLetter, Guid.NewGuid(), _credentialServiceMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenTheRequestIsIgnoredAndNoLetterRequestIsSentToThePrinterQueue()
            {
                _busControlMock.Verify(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)), Times.Never);
            }

            public void AndThenThereShouldNotBeAnyExceptionThrown()
            {
                _caughtException.Should().BeNull();
            }

            public void AndThenVerifyAllCredentialsArePulledToDetermineCosponsoredOnlyScenario()
            {
                _credentialServiceMock.Verify(_ => 
                    _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount), Times.Once);
            }
        }

        #endregion Triggered communication for earned MBM letter NOT sent for cosponsored only scenario

        #region Triggered communication for earned MBM letter sent for ABIM certified diplomates scenario - US Address
        private class TriggeredCommunicationForEarnedMBMLetterSentForABIMUSDiplomates
            : HelperServiceScenario
        {
            private Credential mockedCredential;
            private int mockedTotalCount;
            private Exception _caughtException;

            public void GivenTheDiplomateIsNOTCosponsoredButABIMCertified()
            {
                // Mocked ABIM Credential. (Where OnBehalfOfCode and OnBehalfOfName are null/empty)
                mockedCredential = CredentialBuilder
                    .Build();
                mockedCredential.AddIssuance(Issuance.Create(RandomString.Build()));
                var mockedResponse = new List<Credential> { mockedCredential };
                mockedTotalCount = mockedResponse.Count;
                _credentialServiceMock.Setup(_ => _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount))
                    .Returns(mockedResponse)
                    .Verifiable("Credential service is expected to be called.");

                _busControlMock.Setup(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)))
                    .Returns(Task.Delay(0));

                var profile = new ProfileResource
                {
                    Addresses = new List<ProfileAddressResource>
                    {
                        new ProfileAddressResource
                        {
                            IsPrimary = true,
                            CountryId =  "US",
                            RegionId =  1
                        }
                    },
                    Name = new ProfileNameResource
                    {
                        FirstName = RandomString.Build()
                    }
                };

                var region = new List<RegionResource> { 
                    new RegionResource() {
                            Id = 1,
                            Code = "AL",
                            Name = "Alabama"
                }};
                
                var countries = new List<CountryResource>
                {
                    new CountryResource()
                    {
                        Code = "CA",
                        Name = "Canada" 
                    },                
                    new CountryResource()
                    {
                        Code = "US",
                        Name = "United States"
                    }
                }; 

                _membershipClientServiceMock.Setup(_ => _.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync(profile);

                _membershipClientServiceMock.Setup(_ => _.GetCountryRegionsAsync(It.IsAny<string>()))
                       .ReturnsAsync(region);

                _membershipClientServiceMock.Setup(_ => _.GetCountriesAsync())
                    .ReturnsAsync(countries);
            }

            public async Task WhenRequestIsMadeForEarnedMBMLetterTriggerCommunication()
            {
                try
                {
                    await _sut.TriggeredCommunication(mockedCredential, TriggeredCommunication.EarnedMBMCertLetter, Guid.NewGuid(), _credentialServiceMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenTheRequestIsHonoredAndLetterRequestIsCalledToGetCountryRegionsAsync()
            {
                _membershipClientServiceMock.Verify(_ => _.GetCountryRegionsAsync(It.IsAny<string>()), Times.Once);
            }

            public void ThenTheRequestIsHonoredAndLetterRequestIsSentToThePrinterQueue()
            {
                _busControlMock.Verify(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)), Times.Once);
            }

            public void AndThenThereShouldNotBeAnyExceptionThrown()
            {
                _caughtException.Should().BeNull();
            }

            public void AndThenVerifyAllCredentialsArePulledToDetermineABIMCertifiedScenario()
            {
                _credentialServiceMock.Verify(_ =>
                    _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount), Times.Once);
            }
        }

        #endregion Triggered communication for earned MBM letter sent for ABIM certified diplomates scenario - US Address


        #region Triggered communication for earned MBM letter sent for ABIM certified  diplomates scenario  - Japan Address
        private class TriggeredCommunicationForEarnedMBMLetterSentForABIMNoneUSDiplomates
            : HelperServiceScenario
        {
            private Credential mockedCredential;
            private int mockedTotalCount;
            private Exception _caughtException;

            public void GivenTheDiplomateIsNOTCosponsoredButABIMCertified()
            {
                // Mocked ABIM Credential. (Where OnBehalfOfCode and OnBehalfOfName are null/empty)
                mockedCredential = CredentialBuilder
                    .Build();
                mockedCredential.AddIssuance(Issuance.Create(RandomString.Build()));
                var mockedResponse = new List<Credential> { mockedCredential };
                mockedTotalCount = mockedResponse.Count;
                _credentialServiceMock.Setup(_ => _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount))
                    .Returns(mockedResponse)
                    .Verifiable("Credential service is expected to be called.");

                _busControlMock.Setup(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)))
                    .Returns(Task.Delay(0));

                var profile = new ProfileResource
                {
                    Addresses = new List<ProfileAddressResource>
                    {
                        new ProfileAddressResource
                        {
                            IsPrimary = true,
                            CountryId =  "JP",
                        }
                    },
                    Name = new ProfileNameResource
                    {
                        FirstName = RandomString.Build()
                    }
                };

                var region = new List<RegionResource> {
                    new RegionResource() {
                            Id = 1,
                            Code = "AL",
                            Name = "Alabama"
                }};

                var countries = new List<CountryResource>
                {
                    new CountryResource()
                    {
                        Code = "CA",
                        Name = "Canada"
                    },
                    new CountryResource()
                    {
                        Code = "US",
                        Name = "United States"
                    }
                };

                _membershipClientServiceMock.Setup(_ => _.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
                       .ReturnsAsync(profile);

                _membershipClientServiceMock.Setup(_ => _.GetCountryRegionsAsync(It.IsAny<string>()))
                       .ReturnsAsync(region);

                _membershipClientServiceMock.Setup(_ => _.GetCountriesAsync())
                    .ReturnsAsync(countries);
            }

            public async Task WhenRequestIsMadeForEarnedMBMLetterTriggerCommunication()
            {
                try
                {
                    await _sut.TriggeredCommunication(mockedCredential, TriggeredCommunication.EarnedMBMCertLetter, Guid.NewGuid(), _credentialServiceMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenTheRequestIsHonoredAndLetterRequestIsNeverCalledToGetCountryRegionsAsync()
            {
                _membershipClientServiceMock.Verify(_ => _.GetCountryRegionsAsync(It.IsAny<string>()), Times.Never);
            }

            public void ThenTheRequestIsHonoredAndLetterRequestIsSentToThePrinterQueue()
            {
                _busControlMock.Verify(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)), Times.Once);
            }

            public void AndThenThereShouldNotBeAnyExceptionThrown()
            {
                _caughtException.Should().BeNull();
            }

            public void AndThenVerifyAllCredentialsArePulledToDetermineABIMCertifiedScenario()
            {
                _credentialServiceMock.Verify(_ =>
                    _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount), Times.Once);
            }
        }

        #endregion Triggered communication for earned MBM letter sent for ABIM certified diplomates scenario - Japan Address



        #region Triggered communication for earned MBM letter sent for diplomates with a ABIM certified credential and a other board credential scenario (joint board)
        private class TriggeredCommunicationForEarnedMBMLetterSentForJointBoardDiplomates
            : HelperServiceScenario
        {
            private Credential _abimCredential;
            private Credential _otherBoardCredential;
            private int mockedTotalCount;
            private Exception _caughtException;

            public void GivenTheDiplomateEarnedJointBoardCredentials()
            {
                // Mocked ABIM Credential. (Where OnBehalfOfCode and OnBehalfOfName are null/empty)
                _abimCredential = CredentialBuilder
                    .Build();
                _abimCredential.AddIssuance(Issuance.Create(RandomString.Build()));

                // Mocked other-board Credential. (Where OnBehalfOfCode and OnBehalfOfName are set)
                _otherBoardCredential = CredentialBuilder
                    .Build(RandomString.Build(), RandomString.Build());
                _otherBoardCredential.AddIssuance(Issuance.Create(RandomString.Build()));

                var mockedResponse = new List<Credential> { _abimCredential, _otherBoardCredential };
                mockedTotalCount = mockedResponse.Count;
                _credentialServiceMock.Setup(_ => _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount))
                    .Returns(mockedResponse)
                    .Verifiable("Credential service is expected to be called.");

                _busControlMock.Setup(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)))
                    .Returns(Task.Delay(0));

                var profile = new ProfileResource
                {
                    Addresses = new List<ProfileAddressResource>
                    {
                        new ProfileAddressResource
                        {
                           IsPrimary = true,
                            CountryId =  "US",
                            RegionId = 1
                        }
                    },
                    Name = new ProfileNameResource
                    {
                        FirstName = RandomString.Build()
                    }
                };

                var region = new List<RegionResource> { 
                    new RegionResource() {
                        Id = 1,
                        Code = "AL",
                        Name = "Alabama"
                } };

                var countries = new List<CountryResource>
                {
                    new CountryResource()
                    {
                        Code = "CA",
                        Name = "Canada"
                    },
                    new CountryResource()
                    {
                        Code = "US",
                        Name = "United States"
                    }
                };

                _membershipClientServiceMock.Setup(_ => _.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(profile);

                _membershipClientServiceMock.Setup(_ => _.GetCountryRegionsAsync(It.IsAny<string>()))
                       .ReturnsAsync(region);

                _membershipClientServiceMock.Setup(_ => _.GetCountriesAsync())
                    .ReturnsAsync(countries);
            }

            public async Task WhenRequestIsMadeForEarnedMBMLetterTriggerCommunication()
            {
                try
                {
                    await _sut.TriggeredCommunication(_abimCredential, TriggeredCommunication.EarnedMBMCertLetter, Guid.NewGuid(), _credentialServiceMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenTheRequestIsHonoredAndLetterRequestIsSentToThePrinterQueue()
            {
                _busControlMock.Verify(_ => _.Publish(It.IsAny<NotificationEvent>(), default(CancellationToken)), Times.Once);
            }

            public void AndThenThereShouldNotBeAnyExceptionThrown()
            {
                _caughtException.Should().BeNull();
            }

            public void AndThenVerifyAllCredentialsArePulledToDetermineJointBoardScenario()
            {
                _credentialServiceMock.Verify(_ =>
                    _.SearchByMemberId(It.IsAny<Guid>(), It.IsAny<PageDefinition>(), out mockedTotalCount), Times.Once);
            }
        }

        #endregion Triggered communication for earned MBM letter sent for ABIM certified diplomates scenario
        
        #endregion
    }
}
