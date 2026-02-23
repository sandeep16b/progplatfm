using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Tests.Setup.DomainBuilders
{
    public static class IssuanceBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static Issuance Build()
        {
            return Issuance.Create(SourceBuilder.Build(), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Expired, DateTimeBuilder.Random().Build(), RandomString.Build());
        }
        public static Issuance BuildActiveMaintained()
        {
            return Issuance.Create(SourceBuilder.Build(), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), MaintenanceStatusType.Maintained,
                EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Active, DateTimeBuilder.Random().Build(), RandomString.Build());
        }
        public static Issuance BuildActiveMaintained( DateTime issuanceDate)
        {
            return Issuance.Create(SourceBuilder.Build(), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), MaintenanceStatusType.Maintained,
                EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Active, issuanceDate, RandomString.Build());
        }
        public static Issuance Build(Source source,DateTime issuanceDate)
        {
            return Issuance.Create(SourceBuilder.Build(), EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), MaintenanceStatusType.Maintained,
                EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Active, DateTimeBuilder.Random().Build(), RandomString.Build());
        }

        public static Issuance Build(string sourceName, string sourceCode, string sourceCreatedBy)
        {
            var source = Source.Create(sourceName, sourceCode, sourceCreatedBy);
            return Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Expired, DateTimeBuilder.Random().Build(), RandomString.Build());
        }

        public static Issuance Build(Source source, IssuanceStatusType issuanceStatus)
        {
            return Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(),
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), issuanceStatus, DateTimeBuilder.Random().Build(), RandomString.Build());
        }

        public static Issuance Build(Source source, IssuanceStatusType issuanceStatus, DurationType durationType, DateTime issuanceDate)
        {
            return Issuance.Create(source, durationType,
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                EnumAttributes.RandomEntry<OccurrenceType>(), issuanceStatus, issuanceDate, RandomString.Build());
        }

        public static Issuance Build(Source source, IssuanceStatusType issuanceStatus, DurationType durationType, MaintenanceRequirementType maintenanceRequirement, MaintenanceStatusType maintenanceStatus, DateTime issuanceDate)
        {
            return Issuance.Create(source, durationType,
                maintenanceRequirement, maintenanceStatus,
                EnumAttributes.RandomEntry<OccurrenceType>(), issuanceStatus, issuanceDate, RandomString.Build());
        }

        public static Issuance Build(Source source, IssuanceStatusType issuanceStatus, DurationType durationType, MaintenanceStatusType maintenanceStatus, DateTime issuanceDate)
        {
            return Issuance.Create(source, durationType,
                EnumAttributes.RandomEntry<MaintenanceRequirementType>(), maintenanceStatus,
                EnumAttributes.RandomEntry<OccurrenceType>(), issuanceStatus, issuanceDate, RandomString.Build());
        }

        public static Issuance BuildWithoutRandoms(
            Source source, 
            IssuanceStatusType issuanceStatus, 
            DateTime issuanceDate, 
            DurationType durationType, 
            MaintenanceRequirementType maintenanceRequirement, 
            MaintenanceStatusType maintenanceStatus, 
            OccurrenceType occurrenceType)
        {
            return Issuance.Create(
                source, 
                durationType,
                maintenanceRequirement, 
                maintenanceStatus,
                occurrenceType, 
                issuanceStatus,  
                issuanceDate,  
                "UnitTest");
        }

        public static Issuance Build_ (  CredentialCategoryType category,                                      
                                        DateTime issuanceDate,
                                        IssuanceStatusType issuanceStatus = IssuanceStatusType.Active,
                                        OccurrenceType occurrenceType = OccurrenceType.Initial,
                                        MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.Maintained,
                                        Source source = null)
        {
            source = source ?? SourceBuilder.BuildAbim();
            switch (category)
            {
                case CredentialCategoryType.GrandFather:
                    return Issuance.Create(
                       source,
                       DurationType.Lifetime,
                       MaintenanceRequirementType.NotRequired,
                       maintenanceStatus,
                       OccurrenceType.Initial,
                       issuanceStatus,
                       issuanceDate,
                       "UnitTestGF");
                case CredentialCategoryType.TimeLimited:
                    return Issuance.Create(
                       source,
                       DurationType.Timelimited,
                       MaintenanceRequirementType.NotRequired,
                       maintenanceStatus,
                       occurrenceType,
                       issuanceStatus,
                       issuanceDate,
                       "UnitTestTL");
                case CredentialCategoryType.MustBeMaintained:
                    return Issuance.Create(
                       source,
                       DurationType.Continuous,
                       MaintenanceRequirementType.Required,
                       maintenanceStatus,
                       occurrenceType,
                       issuanceStatus,
                       issuanceDate,
                       "UnitTestMBM");
                case CredentialCategoryType.InitialFPHM:
                    return Issuance.Create(
                       source,
                       DurationType.Timelimited,
                       MaintenanceRequirementType.NotRequired,
                       maintenanceStatus,
                       OccurrenceType.Initial,
                       issuanceStatus,
                       issuanceDate,
                       "UnitTestInitFPHM");
            }
                


            return Issuance.Create(
                source,
                DurationType.Timelimited,
                MaintenanceRequirementType.NotRequired,
                MaintenanceStatusType.Maintained,
                occurrenceType,
                issuanceStatus,
                issuanceDate,
                "UnitTest");
        }
    }
}
