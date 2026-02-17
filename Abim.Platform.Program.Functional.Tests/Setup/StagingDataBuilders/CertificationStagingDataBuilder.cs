using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using System;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class CertificationStagingDataBuilder
    {
        private Certification _data;

        public CertificationStagingDataBuilder()
        {
            Reset();
        }

        public CertificationStagingDataBuilder WithCertificationCode(string code)
        {
            _data.Code = code;
            return this;
        }

        public CertificationStagingDataBuilder WithCertificationId(Guid CertificationId)
        {
            _data.ExternalId = CertificationId;
            return this;
        }

        public CertificationStagingDataBuilder WithSource(Source source)
        {
            _data.Source = source;
            return this;
        }

        public Certification Build()
        {
            var output = _data;
            Reset();
            return output;
        }

        private void Reset()
        {
            _data = CertificationBuilder.Build();

        }
    }
}
