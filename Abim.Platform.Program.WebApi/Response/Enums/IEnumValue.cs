using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// IEnumValue interface
    /// </summary>
    public interface IEnumValue
    {
        /// <summary>
        /// A single character or an integer, used to identify this enum value.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// A single word string used to represent this enum value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// The name from the display attribute.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The short name from the display attribute.
        /// </summary>
        string ShortName { get; }

        /// <summary>
        /// A longform string describing this enum value from the display attribute.
        /// </summary>
        string Description { get; }
    }
}
