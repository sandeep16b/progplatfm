using Abim.Enterprise.Core.Registration.Resources;
using Abim.Enterprise.Core.Testing.Setup.DataBuilders;

namespace Abim.Platform.Program.Tests.Setup.ResourceDataBuilders
{
    public class SeatRegistrationResourceDataBuilder : ResourceDataBuilder<SeatRegistrationSummaryResource, SeatRegistrationResourceDataBuilder>
    {
        private SeatRegistrationSummaryResource _seatRegistration;

        public SeatRegistrationResourceDataBuilder(SeatRegistrationSummaryResource seatRegistration) : base(seatRegistration)
        {
        }

        public SeatRegistrationResourceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static SeatRegistrationSummaryResource GetDataCreator()
        {
            return new SeatRegistrationSummaryResource();
        }
    }
}
