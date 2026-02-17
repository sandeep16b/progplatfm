using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.Tests.Setup.Responses;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Setup.ResourceBuilders
{
    public class RegistrationResourceBuilder
    {
        private RegistrationResource _resource;

        public RegistrationResourceBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _resource = new RegistrationResource();
            _resource.Seats = new List<SeatRegistrationSummaryResource>();
        }

        public RegistrationResourceBuilder WithCertificationId(Guid certificationId)
        {
            _resource.CertificationId = certificationId;
            return this;
        }

        public RegistrationResourceBuilder WithSeatRegistrations(List<SeatRegistrationSummaryResource> seatRegistrations)
        {
            _resource.Seats = seatRegistrations;
            return this;
        }

        public RegistrationResourceBuilder WithExamType(ExamType examTypeValue)
        {
            _resource.ExamType = new RegistrationEnumValueResponseResource<ExamType>(examTypeValue);
            return this;
        }

        public RegistrationResourceBuilder WithResult(string result)
        {
            _resource.Result = result;
            return this;
        }

        public RegistrationResourceBuilder WithAdministrationYear(int year)
        {
            _resource.AdministrationYear = year;
            return this;
        }

        public RegistrationResourceBuilder WithPhyisicianIsAbim(bool isAbim)
        {
            _resource.PhysicianIsAbim = isAbim;
            return this;
        }

        public RegistrationResourceBuilder WithSeat(DateTime seatDate)
        {
            //TODO: Implement a SeatBuilder
            if (_resource.Seats == null)
                _resource.Seats = new List<SeatRegistrationSummaryResource>();

            _resource.Seats.Add(new SeatRegistrationSummaryResource { SeatDate = seatDate });
            return this;
        }

        public RegistrationResourceBuilder WithExamResult(ExamResultType result)
        {
            _resource.ExamResult = new ExamResultResource();
            _resource.ExamResult.Result = new RegistrationEnumValueResponseResource<ExamResultType>(result);
            _resource.Result = result.ToString();
            return this;
        }

        public RegistrationResourceBuilder WithExamResult(ExamResultResource examResult)
        {
            _resource.ExamResult = examResult;
            _resource.Result = examResult.Result.ToString();
            return this;
        }

        public RegistrationResourceBuilder WithAdministerType(AdministerType administerType)
        {
            _resource.AdministerType = new RegistrationEnumValueResponseResource<AdministerType>(administerType);
            return this;
        }

        public RegistrationResourceBuilder WithAdministrationDate(DateTime adminDate)
        {
            _resource.AdministrationDate = adminDate;
            return this;
        }

        public RegistrationResourceBuilder WithAdministrationId(Guid id)
        {
            _resource.AdministrationId = id;
            return this;
        }

        public RegistrationResourceBuilder WithId(Guid id)
        {
            _resource.Id = id;
            return this;
        }

        public RegistrationResourceBuilder WithMemberId(Guid memberId)
        {
            _resource.MemberId = memberId;
            return this;
        }

        public RegistrationResourceBuilder WithOnBehalfOf(string onBehalfOf)
        {
            _resource.OnBehalfOf = onBehalfOf;
            return this;
        }

        public RegistrationResourceBuilder WithProductId(Guid productId)
        {
            _resource.ProductId = productId;
            return this;
        }

        public RegistrationResourceBuilder WithNoConsequence(bool noConsequence)
        {
            _resource.NoConsequence = noConsequence;
            return this;
        }

        public RegistrationResourceBuilder WithAddedSeatRegistrations(SeatRegistrationSummaryResource seatRegistration)
        {
            if (_resource.Seats == null)
                _resource.Seats = new List<SeatRegistrationSummaryResource>();

            _resource.Seats.Add(seatRegistration);
            return this;
        }

        public RegistrationResource Build()
        {
            var output = _resource;
            _resource = new RegistrationResource();
            return output;
        }
    }
}
