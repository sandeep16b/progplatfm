namespace Abim.Platform.Program.Enums    //namespace kept from the original library for backwards-compatibility and to avoid namespace issues in the MassTransit type cache.Enums
{
    /// <summary>
    /// The types of Registration-like objects
    /// </summary>
    public enum ExamRegistrationType
    {
        /// <summary>
        /// A Registration
        /// </summary>
        Registration = 1,
        
        /// <summary>
        /// A CMPRegistration
        /// </summary>
        CMPRegistration = 2
    }
}
