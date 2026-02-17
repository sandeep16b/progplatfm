using Abim.Enterprise.Core.Testing.Setup.Builders;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.WebApi.Testing.Setup;

namespace Abim.Platform.Program.Testing.Setup.DataBuilders
{
    public class SourceDataBuilder : DomainDataBuilder<Source, SourceDataBuilder>
    {
        public SourceDataBuilder(Source Source) : base(Source)
        {
        }

        public SourceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static Source GetDataCreator()
        {
            return Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
        }
    }
}
