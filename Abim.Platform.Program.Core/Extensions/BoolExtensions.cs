using Abim.Platform.Program.Core.Api.Attributes;

namespace Abim.Platform.Program.Util.Extensions
{
    public static class BoolExtensions
    {
        /// <summary>
        /// Converts a boolean to "Yes" or "No"
        /// </summary>
        /// <param name="boolValue">if set to <c>true</c> [bool value].</param>
        /// <returns></returns>
       [ExpectedReturnExample(Arguments = new object[]{ true }, ShouldReturn = "Yes")]
        public static string ToYesNo(this bool boolValue)
        {
            return boolValue ? "Yes" : "No";
        }
    }
}
