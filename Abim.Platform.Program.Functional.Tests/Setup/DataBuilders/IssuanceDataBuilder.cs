using Abim.Enterprise.Core.Testing.Setup.Builders;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using System;

namespace Abim.Platform.Program.Testing.Setup.DataBuilders
{
    public class IssuanceDataBuilder : DomainDataBuilder<Issuance, IssuanceDataBuilder>
    {
        public IssuanceDataBuilder(Issuance Issuance) : base(Issuance)
        {
        }

        public IssuanceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static Issuance GetDataCreator()
        {
            return Issuance.Create(RandomString.Build());
        }

        public IssuanceDataBuilder StartWithCredentialCategory(  CredentialCategoryType category,
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
                    return (IssuanceDataBuilder)
                        With(a => a.Source = source)
                            .With(a => a.Duration = DurationType.Lifetime)
                            .With(a => a.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                            .With(a => a.MaintenanceStatus = maintenanceStatus)
                            .With(a => a.Occurrence = OccurrenceType.Initial)
                            .With(a => a.IssuanceStatus = issuanceStatus)
                            .With(a => a.IssuanceDate = issuanceDate);


                case CredentialCategoryType.TimeLimited:
                    return (IssuanceDataBuilder)
                        With(a => a.Source = source)
                            .With(a => a.Duration = DurationType.Timelimited)
                            .With(a => a.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                            .With(a => a.MaintenanceStatus = maintenanceStatus)
                            .With(a => a.Occurrence = occurrenceType)
                            .With(a => a.IssuanceStatus = issuanceStatus)
                            .With(a => a.IssuanceDate = issuanceDate)
                            .With(a => a.ExpirationDate = new DateTime(issuanceDate.Year + 10, 12, 31));


                case CredentialCategoryType.MustBeMaintained:
                    return (IssuanceDataBuilder)
                        With(a => a.Source = source)
                            .With(a => a.Duration = DurationType.Continuous)
                            .With(a => a.MaintenanceRequirement = MaintenanceRequirementType.Required)
                            .With(a => a.MaintenanceStatus = maintenanceStatus)
                            .With(a => a.Occurrence = occurrenceType)
                            .With(a => a.IssuanceStatus = issuanceStatus)
                            .With(a => a.IssuanceDate = issuanceDate);

                case CredentialCategoryType.InitialFPHM:
                    return (IssuanceDataBuilder)
                        With(a => a.Source = source)
                            .With(a => a.Duration = DurationType.Timelimited)
                            .With(a => a.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                            .With(a => a.MaintenanceStatus = maintenanceStatus)
                            .With(a => a.Occurrence = OccurrenceType.Initial)
                            .With(a => a.IssuanceStatus = issuanceStatus)
                            .With(a => a.IssuanceDate = issuanceDate);
                default:
                    return this;
            }
        }


    }
}
