

using Abim.Platform.Program.Relational.Classes;
 
using Abim.Enterprise.Core.Testing.Setup.Builders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.App.Domain;
using System;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Testing.Setup.DataBuilders
{
    public class CredentialDataBuilder : DomainDataBuilder<Credential, CredentialDataBuilder>
    {
        public CredentialDataBuilder(Credential Credential) : base(Credential)
        {
        }

        public CredentialDataBuilder() : base(() => GetDataCreator())
        {
        }

        public CredentialDataBuilder StartWithCertCode(string certCode,
                                        Guid? memberId = null,
                                        CredentialType credType = CredentialType.General,
                                        PathwayType pathwayType = PathwayType.MOC)
        {
            memberId = memberId ?? Guid.NewGuid();

            return (CredentialDataBuilder) With(a => a.Certification = CertificationBuilder.Build(certCode))
                                .With(a => a.MemberId = memberId.Value)
                                .With(a => a.Type = credType)
                                .With(a => a.Pathway = pathwayType);
        }

        public CredentialDataBuilder(Certification certification) : base(() => GetDataCreator(certification))
        {
        }

        public static Credential GetDataCreator(Certification certification)
        {
            return Credential.Create(certification, 
                                        Guid.NewGuid(), 
                                        EnumAttributes.RandomEntry<CredentialType>(),
                                        EnumAttributes.RandomEntry<PathwayType>(),
                                        null, null,
                                        RandomString.Build());
        }

        public static Credential GetDataCreator()
        {
            return Credential.Create(   CertificationBuilder.Build(),
                                        Guid.NewGuid(),
                                        EnumAttributes.RandomEntry<CredentialType>(),
                                        EnumAttributes.RandomEntry<PathwayType>(),
                                        null, null,
                                        RandomString.Build());
        }
    }
}
