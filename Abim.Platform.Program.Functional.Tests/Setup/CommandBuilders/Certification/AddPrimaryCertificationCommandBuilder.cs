using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Certification
{
    public class AddPrimaryCertificationCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<AddPrimaryCertificationCommand> Builder()
        {
            return CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid();
        }

        public static CommandBuilder<AddPrimaryCertificationCommand> BuilderInvalid()
        {
            return CommandBuilder<AddPrimaryCertificationCommand>
                        .Invalid();
        }

        public static AddPrimaryCertificationCommand Build()
        {
            return Builder().Build();
        }

        public static AddPrimaryCertificationCommand BuildInvalid()
        {
            return CommandBuilder<AddPrimaryCertificationCommand>.Invalid().Build();
        }
    }
}
