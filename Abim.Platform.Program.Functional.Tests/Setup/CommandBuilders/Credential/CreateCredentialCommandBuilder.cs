using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Credential
{
    public class CreateCredentialCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<CreateCredentialCommand> Builder()
        {
            return CommandBuilder<CreateCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.GracePeriodStartDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.GracePeriodEndDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.ExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull());
        }

        public static CommandBuilder<CreateCredentialCommand> BuilderInvalid()
        {
            return CommandBuilder<CreateCredentialCommand>
                        .Invalid();
        }

        public static CreateCredentialCommand Build()
        {
            return Builder().Build();
        }

        public static CreateCredentialCommand BuildInvalid()
        {
            return CommandBuilder<CreateCredentialCommand>.Invalid().Build();
        }
    }
}
