using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using FluentAssertions;
using Hangfire;
using MassTransit;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using static Abim.Platform.Program.Resources.ProgramResourceConstants;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can retrieve the Voc Letter Content for an abmId"
    )]
    [TestFixture]
    public class GetVocLetterContentCommandSpec
    {
        [Test]
        public void GetVocLetterContentWhenValid()
        {
            new GetVocLetterContentWhenValidSpec().BDDfy();
        }

        [Test]
        public void VocLetterContentAddressIsNullWhenNoAddress()
        {
            new VocLetterContentAddressIsNullWhenNoAddressSpec().BDDfy();
        }

        [Test]
        public void VocLetterContentInactiveTrueWhenNoCertsAreActive()
        {
            new VocLetterContentInactiveTrueWhenNoCertsAreActiveSpec().BDDfy();
        }

        [Test]
        public void VocLetterContentNotCertifiedTrueWhenAllCertsAreSuspended()
        {
            new VocLetterContentNotCertifiedTrueWhenAllCertsAreSuspendedSpec().BDDfy();
        }

        [Test]
        public void GetVocLetterContentWhenActiveGFAndExpiredTL()
        {
            new GetVocLetterContentWhenActiveGFAndExpiredTLSpec().BDDfy();
        }

        [Test]
        public void GetVocLetterContentWhenInActiveGFAndExpiredTL()
        {
            new GetVocLetterContentWhenInActiveGFAndExpiredTLSpec().BDDfy();
        }

        [Test]
        public void GetVocLetterContentWhenFPHMselectedToMaintainAndIMexists()
        {
            new GetVocLetterContentWhenFPHMselectedToMaintainAndIMExistsSpec().BDDfy();
        }

        [Test]
        public void GetVocLetterContentWhenFPHMWasNOTselectedToMaintainAndIMexists()
        {
            new GetVocLetterContentWhenFPHMWasNOTselectedToMaintainAndIMexistsSpec().BDDfy();
        }

        [Test]
        public void GetVocLetterContentWhenIMselectedToMaintainAndNoFPHMexists()
        {
           new GetVocLetterContentWhenIMselectedToMaintainAndNoFPHMexistsSpec().BDDfy();
        }

        public class GetVocLetterContentWhenValidSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>(){ typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource () { FirstName="Sam", MiddleName = "m", LastName="Adams"}
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2013, 8, 21),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1, cred2 };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                  new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object, 
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }
        }

        public class VocLetterContentAddressIsNullWhenNoAddressSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;
            //Abim.Platform.Program.App.Services.Impl.CredentialService CredentialService { get; set; }
            HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected PhysicianCertificationsPublicResource PhysicianResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>(),
                    Name= null
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2013, 8, 21),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1, cred2 };

            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };

                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Address.Should().BeNullOrWhiteSpace();
            }
        }

        public class VocLetterContentInactiveTrueWhenNoCertsAreActiveSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(  source: source,
                                                                        issuanceStatus: IssuanceStatusType.Inactive, // !!!
                                                                        issuanceDate: new DateTime(2013, 8, 21),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement : MaintenanceRequirementType.Required,
                                                                        maintenanceStatus : MaintenanceStatusType.Maintained,
                                                                        occurrenceType : OccurrenceType.Initial));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Inactive, // !!!
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1,cred2 };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                    new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.isActive.Should().BeFalse();

            }
        }

        public class VocLetterContentNotCertifiedTrueWhenAllCertsAreSuspendedSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;
            HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Suspended, //!!!
                                                                        issuanceDate: new DateTime(2013, 8, 21),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Suspended, //!!!
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1, cred2 };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                     new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.isCertified.Should().BeFalse();

            }
        }

        public class GetVocLetterContentWhenActiveGFAndExpiredTLSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                // Active GrandFather
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2010, 11, 02),
                                                                        durationType: DurationType.Lifetime,
                                                                        maintenanceRequirement: MaintenanceRequirementType.NotRequired,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                // Expired TL
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Expired,
                                                                        issuanceDate: new DateTime(2013, 11, 02),
                                                                        durationType: DurationType.Timelimited,
                                                                        maintenanceRequirement: MaintenanceRequirementType.NotRequired,
                                                                        maintenanceStatus: MaintenanceStatusType.NotMaintained,
                                                                        occurrenceType: OccurrenceType.Recertification));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1, cred2 };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                             new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object, 
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }

            private void AndInternalMedicineShouldBeCertified()
            {
                // SINCE GF is active then diplomate still certified in IM
                _vocLetterData.CurrentCertifications.Should().Contain("Internal Medicine: Certified");
            }
        }

        public class GetVocLetterContentWhenInActiveGFAndExpiredTLSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred1 = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                // Active GrandFather
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Inactive,
                                                                        issuanceDate: new DateTime(2010, 11, 02),
                                                                        durationType: DurationType.Lifetime,
                                                                        maintenanceRequirement: MaintenanceRequirementType.NotRequired,
                                                                        maintenanceStatus: MaintenanceStatusType.NotMaintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                // Expired TL
                cred1.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Expired,
                                                                        issuanceDate: new DateTime(2013, 11, 02),
                                                                        durationType: DurationType.Timelimited,
                                                                        maintenanceRequirement: MaintenanceRequirementType.NotRequired,
                                                                        maintenanceStatus: MaintenanceStatusType.NotMaintained,
                                                                        occurrenceType: OccurrenceType.Recertification));

                var cred2 = CredentialBuilder.BuildWithoutRandoms(source, "ID", "Infectious Disease", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred2.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));

                Credentials = new List<Credential>() { cred1, cred2 };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                  new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object, 
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }

            private void AndInternalMedicineShouldNOTBeCertified()
            {
                // Since GF is not active and TL is expired then diplomate is not certified in IM
                _vocLetterData.CurrentCertifications.Should().NotContain("Internal Medicine: Certified");
            }
        }

        public class GetVocLetterContentWhenFPHMselectedToMaintainAndIMExistsSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                // IM SelectedToMaintain = false
                credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2010, 11, 02),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credIM.SelectedToMaintain = false;

                // FPHM SelectedToMaintain = true
                var credFPHM = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
                credFPHM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credFPHM.SelectedToMaintain = true;

                // GERI SelectedToMaintain = true
                var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
                credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2018, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credGERI.SelectedToMaintain = true;

                Credentials = new List<Credential>() { credIM, credFPHM, credGERI };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                              new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }

            private void AndInternalMedicineShouldNOTBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().NotContain($"{CertificationName.IM}: Certified");
            }

            private void AndFPHMShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"{CertificationName.IMwithFPHM}: Certified");
            }

            private void AndGeriatricMedicineShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"Geriatric Medicine: Certified");
            }
        }

        public class GetVocLetterContentWhenFPHMWasNOTselectedToMaintainAndIMexistsSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                // IM SelectedToMaintain = true
                credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2010, 11, 02),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credIM.SelectedToMaintain = true;

                // FPHM SelectedToMaintain = false
                var credFPHM = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
                credFPHM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2015, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credFPHM.SelectedToMaintain = false;

                // GERI SelectedToMaintain = true
                var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
                credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2018, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credGERI.SelectedToMaintain = true;

                Credentials = new List<Credential>() { credIM, credFPHM, credGERI };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                               new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }

            private void AndInternalMedicineShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"{CertificationName.IM}: Certified");
            }

            private void AndFPHMShouldNOTBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().NotContain($"{CertificationName.IMwithFPHM}: Certified");
            }

            private void AndGeriatricMedicineShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"Geriatric Medicine: Certified");
            }
        }

        public class GetVocLetterContentWhenIMselectedToMaintainAndNoFPHMexistsSpec : CredentialServiceScenario
        {
            private VocPdfData _vocLetterData;

            protected HelperService HelperService { get; set; }
            protected ProfileNestedResource ProfileNestedResource { get; set; }
            protected IEnumerable<Credential> Credentials { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                return new List<Type>() { typeof(ICredentialRepository), typeof(ICertificationService), typeof(ISourceService), typeof(IHelperService), typeof(IBusControl), typeof(IBackgroundJobClient), typeof(IValidationFactory) };
            }

            protected override void PreSetup()
            {
                ProfileNestedResource = new ProfileNestedResource()
                {
                    AbimId = "345678",
                    Addresses = new List<AddressSummaryResource>() { new AddressSummaryResource { StreetAddress1 = "110 Dominic Dr", StreetAddress2 = null, StreetAddress3 = null, City = "Scott Depot", PostalCode = "25560", Region = new RegionSummaryResource { Code = "WV" } } },
                    Name = new NameResource() { FirstName = "Sam", MiddleName = "m", LastName = "Adams" }
                };

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                // IM SelectedToMaintain = true
                credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2010, 11, 02),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credIM.SelectedToMaintain = true;

                // GERI SelectedToMaintain = true
                var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
                credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                        issuanceStatus: IssuanceStatusType.Active,
                                                                        issuanceDate: new DateTime(2018, 11, 4),
                                                                        durationType: DurationType.Continuous,
                                                                        maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                        maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                        occurrenceType: OccurrenceType.Initial));
                credGERI.SelectedToMaintain = true;

                Credentials = new List<Credential>() { credIM, credGERI };
            }

            protected override void PostSetup()
            {
                HelperService = new HelperService(new Mock<IBusControl>().Object,
                                                 new Mock<IAccessTokenService>().Object,
                                                  new Mock<IProfileInterservice>().Object,
                                                  new Mock<IRegistrationInterservice>().Object);

                IEnumerable<Credential> credentialList = new List<Credential>()
                {
                    CredentialBuilder.Build()
                };
                My<ICredentialRepository>().Setup(r => r.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(credentialList));
            }

            private void WhenICallGetVocLetterContent()
            {
                try
                {
                    _vocLetterData = HelperService.GetVocLetterContent(ProfileNestedResource, Credentials);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            private void AndResultShouldBeAsExpected()
            {
                _vocLetterData.Name.Should().NotBeNull();
                _vocLetterData.Date.Should().NotBeNull();
                _vocLetterData.Address.Should().NotBeNull();
                _vocLetterData.IntialCertifications.Should().NotBeNull();
                _vocLetterData.certsCount.Should().BeGreaterOrEqualTo(0);

            }

            private void AndInternalMedicineShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"{CertificationName.IM}: Certified");
            }

            private void AndFPHMShouldNOTBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().NotContain($"{CertificationName.IMwithFPHM}: Certified");
            }

            private void AndGeriatricMedicineShouldBeCertified()
            {
                // WE should skip IM since FPHM was seleted to be maintained
                _vocLetterData.CurrentCertifications.Should().Contain($"Geriatric Medicine: Certified");
            }
        }
    }
}
