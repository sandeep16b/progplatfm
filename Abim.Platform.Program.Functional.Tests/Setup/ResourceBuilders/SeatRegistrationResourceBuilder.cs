using Abim.Enterprise.Core.Registration.Resources;
using System;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class SeatRegistrationResourceBuilder
    {
        private SeatRegistrationSummaryResource _seatRegistration;

        public SeatRegistrationResourceBuilder()
        {
            Reset();
        }

        public SeatRegistrationResourceBuilder WithSeatDate(DateTime seatDate)
        {
            _seatRegistration.SeatDate = seatDate;
            return this;
        }

        public SeatRegistrationSummaryResource Build()
        {
            var output = _seatRegistration;
            Reset();
            return output;
        }

        private void Reset()
        {
            _seatRegistration = new SeatRegistrationSummaryResource();
            
        }
    }
}
