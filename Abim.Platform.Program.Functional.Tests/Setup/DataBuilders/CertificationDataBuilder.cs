

using Abim.Platform.Program.Relational.Classes;
 
using Abim.Enterprise.Core.Testing.Setup.Builders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Testing.Setup.DataBuilders
{
    public class CertificationDataBuilder : DomainDataBuilder<Certification, CertificationDataBuilder>
    {
        public CertificationDataBuilder(Certification Certification) : base(Certification)
        {
        }

        public CertificationDataBuilder( Source source) : base(() => GetDataCreator(source))
        {
        }

        public CertificationDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static Certification GetDataCreator( Source source)
        {
            return Certification.Create(null, 
                                        source, 
                                        EnumAttributes.RandomEntry<CertificationType>(), 
                                        RandomString.Build(), 
                                        RandomString.Build(), 
                                        RandomString.Build());
        }

        public static Certification GetDataCreator()
        {
            return Certification.Create(null,
                                        SourceBuilder.Build(),
                                        EnumAttributes.RandomEntry<CertificationType>(),
                                        RandomString.Build(),
                                        RandomString.Build(),
                                        RandomString.Build());
        }
    }
}
