namespace Abim.Platform.Program.Util.Enums
{
    /// <summary>
    /// IEnumValue interface
    /// </summary>
    public interface IEnumValue
    {
        /// <summary>
        /// A single character or an integer, used to identify this enum value.
        /// </summary>
        string Code { get; set; }
        
        /// <summary>
        /// A single word string used to represent this enum value.
        /// </summary>
        string Value { get; set; }
        
        /// <summary>
        /// The name from the display attribute.
        /// </summary>
        string Name { get; set; }
        
        /// <summary>
        /// The short name from the display attribute.
        /// </summary>
        string ShortName { get; set; }
        
        /// <summary>
        /// A longform string describing this enum value from the display attribute.
        /// </summary>
        string Description { get; set; }
    }
}
