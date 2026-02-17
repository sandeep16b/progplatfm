using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.Certification
{
    public class AddAddedQualificationCertificationCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<AddAddedQualificationCertificationCommand> Builder()
        {
            return CommandBuilder<AddAddedQualificationCertificationCommand>
                        .Valid();
        }

        public static CommandBuilder<AddAddedQualificationCertificationCommand> BuilderInvalid()
        {
            return CommandBuilder<AddAddedQualificationCertificationCommand>
                        .Invalid();
        }

        public static AddAddedQualificationCertificationCommand Build()
        {
            return Builder().Build();
        }

        public static AddAddedQualificationCertificationCommand BuildInvalid()
        {
            return CommandBuilder<AddAddedQualificationCertificationCommand>.Invalid().Build();
        }
    }
}
