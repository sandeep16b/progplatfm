using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Testing.Setup.CommandBuilders.ProgramRules
{
    public class RunRulesForMustBeMaintainedCertificateCommandBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand> Builder()
        {
            return CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand>
                        .Valid()
                        .WithNoZeroIntegers();
        }

        public static CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand> BuilderInvalid()
        {
            return CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand>
                        .Invalid();
        }

        public static RunRulesForMustBeMaintainedCertificateCommand Build()
        {
            return Builder().Build();
        }

        public static RunRulesForMustBeMaintainedCertificateCommand BuildInvalid()
        {
            return CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand>.Invalid().Build();
        }
    }
}
