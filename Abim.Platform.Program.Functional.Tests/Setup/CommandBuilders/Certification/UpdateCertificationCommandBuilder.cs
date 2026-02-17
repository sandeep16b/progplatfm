

 
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.App.Services.Commands;
using System;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Setup.CommandBuilders.Certification
{
    public class UpdateCertificationCommandBuilder
    {
        protected UpdateCertificationCommand command;

        public UpdateCertificationCommandBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            command = new UpdateCertificationCommand();
        }

        public UpdateCertificationCommandBuilder WithSourceId(Guid sourceId)
        {
            command.SourceId = sourceId;
            return this;
        }

        public UpdateCertificationCommandBuilder WithCode(string code)
        {
            command.Code = code;
            return this;
        }

        public UpdateCertificationCommandBuilder WithName(string name)
        {
            command.Name = name;
            return this;
        }

        public UpdateCertificationCommandBuilder WithType(CertificationType type)
        {
            command.Type = type;
            return this;
        }

        public UpdateCertificationCommandBuilder WithConsecutiveAttempt(int? consecutiveAttempt)
        {
            command.ConsecutiveAttempt = consecutiveAttempt;
            return this;
        }

        public UpdateCertificationCommandBuilder WithUserInfo(UserInfo userInfo)
        {
            command.UserInfo = userInfo;
            return this;
        }

        public UpdateCertificationCommand Build()
        {
            return command;
        }
    }
}
