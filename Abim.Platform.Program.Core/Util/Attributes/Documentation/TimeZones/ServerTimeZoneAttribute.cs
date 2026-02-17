using System;

namespace Abim.Platform.Program.Util.Api.Attributes
{
    /// <summary>
    /// This marks a DateTime explicitly as being stored in the server's timezone, even though that is already the expected
    /// timezone, for the sake of clarity
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All)]
    public class ServerTimeZoneAttribute : Attribute
    {
    }
}
