using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class ReissueCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<ReissueCommand> Builder()
        {
            return CommandBuilder<ReissueCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build());
        }

        public static CommandBuilder<ReissueCommand> BuilderInvalid()
        {
            return CommandBuilder<ReissueCommand>
                        .Invalid();
        }

        public static ReissueCommand Build()
        {
            return Builder().Build();
        }

        public static ReissueCommand BuildInvalid()
        {
            return CommandBuilder<ReissueCommand>.Invalid().Build();
        }
    }
}
