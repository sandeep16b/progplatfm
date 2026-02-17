using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Tests.Setup.DomainBuilders
{
    //TODO: Create new builder class that follows the Builder Pattern

    public static class CredentialBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static Credential Build()
        {
            return Credential.Create(CertificationBuilder.Build(), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
        }

        public static Credential Build(Source certificationSource)
        {
            return Credential.Create(CertificationBuilder.Build(certificationSource), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
        }

        public static Credential Build(string onBehalfBoardCode, string onBehalfBoardName)
        {
            return Credential.Create(CertificationBuilder.Build(), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), onBehalfBoardCode, onBehalfBoardName, RandomString.Build());
        }

        public static Credential Build(Source certificationSource, string code)
        {
            return Credential.Create(CertificationBuilder.Build(certificationSource, code), Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
        }

        public static Credential Build(PathwayType pathway)
        {
            return Credential.Create(
                CertificationBuilder.Build(), 
                Guid.NewGuid(), 
                CredentialType.Subspecialty, 
                pathway,
                null, null,
                "UnitTest");
        }

        public static Credential BuildWithoutRandoms(
            Source certificationSource, 
            string certCode, 
            string certName, 
            CertificationType certType, 
            CredentialType credType, 
            PathwayType pathwayType)
        {
            return BuildWithoutRandoms(
                certificationSource, certCode, certName, certType, credType, pathwayType, null);
        }

        public static Credential BuildWithoutRandoms(
            Source certificationSource,
            string certCode,
            string certName,
            CertificationType certType,
            CredentialType credType,
            PathwayType pathwayType,
            Issuance[] issuances = null)
        {
            var credential = Credential.Create(
                CertificationBuilder.BuildWithoutRandoms(certificationSource, certCode, certType, certName),
                Guid.NewGuid(),
                credType,
                pathwayType,
                null, null,
                "Unit Test");

            if (issuances != null)
            {
                foreach (var issuance in issuances)
                    credential.AddIssuance(issuance);
            }

            return credential;
        }

        public static Credential Build_( string certCode,
                                        Guid? memberId = null,
                                        CredentialType credType = CredentialType.General,
                                        PathwayType pathwayType = PathwayType.MOC,
                                        Issuance[] issuances = null)
         {
            memberId = memberId ?? Guid.NewGuid();

            var credential = Credential.Create(
                CertificationBuilder.Build(certCode),
                memberId.Value,
                credType,
                pathwayType,
                null, null,
                "UnitTest_CredentialBuilder");

            foreach (var issuance in issuances)
                credential.AddIssuance(issuance);

            return credential;
         }
    }
}
