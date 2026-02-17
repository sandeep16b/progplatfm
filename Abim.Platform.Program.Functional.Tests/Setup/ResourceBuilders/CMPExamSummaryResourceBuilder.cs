using Abim.Enterprise.Core.Registration.Resources;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Setup.ResourceBuilders
{
    public class CMPExamSummaryResourceBuilder
    {
        private CMPExamSummaryResource _exam;

        public CMPExamSummaryResourceBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _exam = new CMPExamSummaryResource();
            _exam.NoConsequenceYears = new List<int>();
        }

        public CMPExamSummaryResource Build(bool resetAfterBuild = true)
        {
            if (resetAfterBuild)
            {
                var output = _exam;
                Reset();
                return output;
            }
            else
                return _exam;
        }

        public CMPExamSummaryResourceBuilder WithCertificationId(Guid id)
        {
            _exam.CertificationId = id;
            return this;
        }

        public CMPExamSummaryResourceBuilder WithNoConsequenceYear(int year)
        {
            _exam.NoConsequenceYears.Add(year);
            return this;
        }
    }
}
