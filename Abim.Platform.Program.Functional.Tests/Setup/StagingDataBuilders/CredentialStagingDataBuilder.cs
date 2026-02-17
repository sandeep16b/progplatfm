using Abim.Platform.Program.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using System;

namespace Abim.Platform.Program.Testing.Setup.ResourceBuilders
{
    public class CredentialStagingDataBuilder
    {
        private Credential _data;

        public CredentialStagingDataBuilder()
        {
            Reset();
        }

        public CredentialStagingDataBuilder AddIssuance(Issuance issuance)
        {
            _data.AddIssuance(issuance);
            return this;
        }

        public CredentialStagingDataBuilder WithCertification(Certification certification)
        {
            _data.Certification= certification;
            return this;
        }

        public CredentialStagingDataBuilder WithPathWay(PathwayType pathwayType)
        {
            _data.Pathway = pathwayType;
            return this;
        }

        public CredentialStagingDataBuilder WithExamFailCount(int examFailCount)
        {
            _data.ExamFailCount = examFailCount;
            return this;
        }

        public CredentialStagingDataBuilder WithAssessmentMet(bool assessmentMet)
        {
            _data.AssessmentMet = assessmentMet;
            return this;
        }

        public CredentialStagingDataBuilder WithExamDueDate(DateTime examDueDate)
        {
            _data.ExamDueDate = examDueDate;
            return this;
        }

        public Credential Build()
        {
            var output = _data;
            Reset();
            return output;
        }

        private void Reset()
        {
            _data = CredentialBuilder.Build();
        }
    }
}
