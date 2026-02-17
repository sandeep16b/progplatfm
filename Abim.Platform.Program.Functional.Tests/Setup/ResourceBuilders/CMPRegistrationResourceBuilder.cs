using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.Tests.Setup.Responses;
using System;

namespace Abim.Platform.Program.Tests.Setup.ResourceBuilders
{
    public class CMPRegistrationResourceBuilder
    {
        private CMPRegistrationResource _reg;

        public CMPRegistrationResourceBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _reg = new CMPRegistrationResource();
            _reg.CMPExam = new CMPExamSummaryResource();
        }

        public CMPRegistrationResource Build(bool resetAfterBuild = true)
        {
            if (resetAfterBuild)
            {
                var output = _reg;
                Reset();
                return output;
            }
            else
                return _reg;
        }

        public CMPRegistrationResourceBuilder WithCMPExam(CMPExamSummaryResource exam)
        {
            _reg.CMPExam = exam;
            return this;
        }

        public CMPRegistrationResourceBuilder WithExamResult(ExamResultType examResult)
        {
            _reg.ExamResult = new RegistrationEnumValueResponseResource<ExamResultType>(examResult);
            return this;
        }

        public CMPRegistrationResourceBuilder WithId(Guid id)
        {
            _reg.Id = id;
            return this;
        }

        public CMPRegistrationResourceBuilder WithMemberId(Guid id)
        {
            _reg.MemberId = id;
            return this;
        }

        public CMPRegistrationResourceBuilder WithOnHold(bool onHold)
        {
            _reg.OnHold = onHold;
            return this;
        }

        public CMPRegistrationResourceBuilder WithPhysicianIsAbim(bool isAbim)
        {
            _reg.PhysicianIsAbim = isAbim;
            return this;
        }

        public CMPRegistrationResourceBuilder WithTestDate(DateTime testDate)
        {
            _reg.TestDate = testDate;
            return this;
        }

        
    }
}
