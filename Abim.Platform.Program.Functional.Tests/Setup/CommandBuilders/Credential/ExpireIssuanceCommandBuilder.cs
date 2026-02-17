using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class ExpireIssuanceCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<ExpireIssuanceCommand> Builder()
        {
            return CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .WithNoZeroIntegers();
        }

        public static CommandBuilder<ExpireIssuanceCommand> BuilderInvalid()
        {
            return CommandBuilder<ExpireIssuanceCommand>
                        .Invalid();
        }

        public static ExpireIssuanceCommand Build()
        {
            return Builder().Build();
        }

        public static ExpireIssuanceCommand BuildInvalid()
        {
            return CommandBuilder<ExpireIssuanceCommand>.Invalid().Build();
        }
    }
}
