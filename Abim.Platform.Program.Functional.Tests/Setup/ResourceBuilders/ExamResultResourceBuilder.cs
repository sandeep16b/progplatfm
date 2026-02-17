using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.Tests.Setup.Responses;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class ExamResultResourceBuilder
    {
        private ExamResultResource _examResult;

        public ExamResultResourceBuilder()
        {
            Reset();
        }

        public ExamResultResourceBuilder WithExamResultType(ExamResultType resultType)
        {
            _examResult.Result = new RegistrationEnumValueResponseResource<ExamResultType>(resultType);
            return this;
        }

        public ExamResultResource Build()
        {
            var output = _examResult;
            Reset();
            return output;
        }

        public ExamResultResourceBuilder WithResult(ExamResultType resultType)
        {
            _examResult.Result = new RegistrationEnumValueResponseResource<ExamResultType>(resultType);
            return this;
        }

        private void Reset()
        {
            _examResult = new ExamResultResource();
            
        }
    }
}
