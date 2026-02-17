using System;

namespace Abim.Platform.Program.WebApi.Api.Attributes
{
    /// <summary>
    /// This marks a DateTime as being stored in anything other than the server's time zone
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All)]
    public class ForeignTimeZoneAttribute : Attribute
    {
    }
}
