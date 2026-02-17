using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using System;

namespace Abim.Platform.Program.Tests.Setup.DomainBuilders
{
    public static class SourceBuilder
    {
        private static Random Random = new Random();
        private static EmailBuilder EmailBuilder = new EmailBuilder();
        private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

        public static App.Domain.Source Build()
        {
            return App.Domain.Source.Create(RandomString.BuildWithLength(1, 255), RandomString.BuildWithLength(1, 10), RandomString.BuildWithLength(1, 255));
        }

        public static App.Domain.Source Build(string sourceName, string sourceCode, string sourceCreatedBy)
        {
            return App.Domain.Source.Create(sourceName, sourceCode, sourceCreatedBy);
        }

        public static App.Domain.Source BuildAbim()
        {
            return App.Domain.Source.Create("American Board of Internal Medicine", "ABIM", "UnitTest_BuildAbim");
        }

        public static App.Domain.Source BuildOtherBoard()
        {
            return App.Domain.Source.Create("Unknown Board", "Other", "UnitTest_BuildOtherBoard");
        }

    }
}
