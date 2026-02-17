using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class SetIssuanceToMaintainedCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<SetIssuanceToMaintainedCommand> Builder()
        {
            return CommandBuilder<SetIssuanceToMaintainedCommand>
                        .Valid();
        }

        public static CommandBuilder<SetIssuanceToMaintainedCommand> BuilderInvalid()
        {
            return CommandBuilder<SetIssuanceToMaintainedCommand>
                        .Invalid();
        }

        public static SetIssuanceToMaintainedCommand Build()
        {
            return Builder().Build();
        }

        public static SetIssuanceToMaintainedCommand BuildInvalid()
        {
            return CommandBuilder<SetIssuanceToMaintainedCommand>.Invalid().Build();
        }
    }
}
