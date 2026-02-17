using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.App.Services.Commands;
using System;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Setup.CommandBuilders.Credential
{
    public class AddCredentialCommandBuilder
    {
        private AddCredentialCommand command;

        public AddCredentialCommandBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            command = new AddCredentialCommand();
        }

        public AddCredentialCommandBuilder WithMemberId(Guid memberId)
        {
            command.MemberId = memberId;
            return this;
        }

        public AddCredentialCommandBuilder WithCertificationId(Guid certificationId)
        {
            command.CertificationId = certificationId;
            return this;
        }

        public AddCredentialCommandBuilder WithPathway(PathwayType pathway)
        {
            command.Pathway = pathway;
            return this;
        }

        public AddCredentialCommandBuilder WithUserInfo(UserInfo userInfo)
        {
            command.UserInfo = userInfo;
            return this;
        }

        public AddCredentialCommand Build()
        {
            return command;
        }
    }
}
