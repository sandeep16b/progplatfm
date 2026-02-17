using Abim.Enterprise.Core.Registration.Resources;
using Abim.Enterprise.Core.Testing.Setup.DataBuilders;

namespace Abim.Platform.Program.Tests.Setup.ResourceDataBuilders
{
    public class ExamResultResourceDataBuilder : ResourceDataBuilder<ExamResultResource, ExamResultResourceDataBuilder>
    {
       // private ExamResultResource _examResult;

        public ExamResultResourceDataBuilder(ExamResultResource examResult) : base(examResult)
        {
        }

        public ExamResultResourceDataBuilder() : base(() => GetDataCreator())
        {
        }

        public static ExamResultResource GetDataCreator()
        {
            return new ExamResultResource();
        }
    }
}
