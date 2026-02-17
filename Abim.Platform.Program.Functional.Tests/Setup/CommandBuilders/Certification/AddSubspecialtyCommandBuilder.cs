using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Certification
{
    public class AddSubspecialtyCertificationCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<AddSubspecialtyCertificationCommand> Builder()
        {
            return CommandBuilder<AddSubspecialtyCertificationCommand>
                        .Valid();
        }

        public static CommandBuilder<AddSubspecialtyCertificationCommand> BuilderInvalid()
        {
            return CommandBuilder<AddSubspecialtyCertificationCommand>
                        .Invalid();
        }

        public static AddSubspecialtyCertificationCommand Build()
        {
            return Builder().Build();
        }

        public static AddSubspecialtyCertificationCommand BuildInvalid()
        {
            return CommandBuilder<AddSubspecialtyCertificationCommand>.Invalid().Build();
        }
    }
}
