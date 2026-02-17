using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class ExpireAndReissueCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<ExpireAndReissueCommand> Builder()
        {
            return CommandBuilder<ExpireAndReissueCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .WithNoZeroIntegers();
        }

        public static CommandBuilder<ExpireAndReissueCommand> BuilderInvalid()
        {
            return CommandBuilder<ExpireAndReissueCommand>
                        .Invalid();
        }

        public static ExpireAndReissueCommand Build()
        {
            return Builder().Build();
        }

        public static ExpireAndReissueCommand BuildInvalid()
        {
            return CommandBuilder<ExpireAndReissueCommand>.Invalid().Build();
        }
    }
}
