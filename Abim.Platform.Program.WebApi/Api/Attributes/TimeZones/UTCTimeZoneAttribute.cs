using System;

namespace Abim.Platform.Program.WebApi.Api.Attributes
{
    /// <summary>
    /// This marks a DateTime as being stored in UTC
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All)]
    public class UTCTimeZoneAttribute : Attribute
    {
    }
}
