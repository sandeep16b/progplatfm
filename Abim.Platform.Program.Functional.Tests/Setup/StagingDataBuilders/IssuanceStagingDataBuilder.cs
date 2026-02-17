using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using System;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class IssuanceStagingDataBuilder
    {
        private Issuance _data;

        public IssuanceStagingDataBuilder()
        {
            Reset();
        }

        public IssuanceStagingDataBuilder WithExpirationDate(DateTime expirationDate)
        {
            _data.ExpirationDate = expirationDate;
            return this;
        }

        public IssuanceStagingDataBuilder WithSource(Source source)
        {
            _data.Source = source;
            return this;
        }

        public Issuance Build()
        {
            var output = _data;
            Reset();
            return output;
        }

        private void Reset()
        {
            _data = IssuanceBuilder.Build();
        }
    }
}
