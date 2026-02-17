

 
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.App.Services.Commands;
using System;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Setup.CommandBuilders.Certification
{
    public class AddCertificationCommandBuilder
    {
        protected AddCertificationCommand command;

        public AddCertificationCommandBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            command = new AddCertificationCommand();
        }

        public AddCertificationCommandBuilder WithSourceId(Guid sourceId)
        {
            command.SourceId = sourceId;
            return this;
        }

        public AddCertificationCommandBuilder WithCode(string code)
        {
            command.Code = code;
            return this;
        }

        public AddCertificationCommandBuilder WithName(string name)
        {
            command.Name = name;
            return this;
        }

        public AddCertificationCommandBuilder WithType(CertificationType type)
        {
            command.Type = type;
            return this;
        }

        public AddCertificationCommandBuilder WithConsecutiveAttempt(int? consecutiveAttempt)
        {
            command.ConsecutiveAttempt = consecutiveAttempt;
            return this;
        }

        public AddCertificationCommandBuilder WithUserInfo(UserInfo userInfo)
        {
            command.UserInfo = userInfo;
            return this;
        }

        public AddCertificationCommand Build()
        {
            return command;
        }
    }
}
