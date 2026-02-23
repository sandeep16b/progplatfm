using Abim.Enterprise.Core.Registration.Resources;
using Abim.Enterprise.Core.Testing.Setup.DataBuilders;

namespace Abim.Platform.Program.Tests.Setup.ResourceDataBuilders
{
    public class RegistrationResourceDataBuilder : ResourceDataBuilder<RegistrationResource, RegistrationResourceDataBuilder>
    {
        private RegistrationResource _resource;

        public RegistrationResourceDataBuilder(RegistrationResource registrationResource) : base(registrationResource)
        {
        }

        public RegistrationResourceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static RegistrationResource GetDataCreator()
        {
            return new RegistrationResource();
        }
    }
}
