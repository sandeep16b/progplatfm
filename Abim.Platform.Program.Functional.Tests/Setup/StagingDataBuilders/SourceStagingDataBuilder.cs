using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class SourceStagingDataBuilder
    {
        private Source _data;

        public SourceStagingDataBuilder()
        {
            Reset();
        }

        public SourceStagingDataBuilder WithSourceCode(string code)
        {
            _data.Code = code;
            return this;
        }

        public Source Build()
        {
            var output = _data;
            Reset();
            return output;
        }

        private void Reset()
        {
            _data = SourceBuilder.Build();
        }
    }
}
