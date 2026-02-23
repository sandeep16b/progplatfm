using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class IssueFPHMCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<IssueFPHMCommand> Builder()
        {
            return CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build());
                        //.With(cmd => cmd.PassExamDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build());
        }

        public static CommandBuilder<IssueFPHMCommand> BuilderInvalid()
        {
            return CommandBuilder<IssueFPHMCommand>
                        .Invalid();
        }

        public static IssueFPHMCommand Build()
        {
            return Builder().Build();
        }

        public static IssueFPHMCommand BuildInvalid()
        {
            return CommandBuilder<IssueFPHMCommand>.Invalid().Build();
        }
    }
}
