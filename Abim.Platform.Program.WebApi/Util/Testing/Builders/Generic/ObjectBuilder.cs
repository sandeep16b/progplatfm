namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Creates an object of any type, with either valid or invalid values for its properties. Uses reflection to test the properties. For general testing
    /// purposes. See CommandBuilder, which inherits from this class, for detailed examples
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.Builders.BaseObjectBuilder{T,Abim.Platform.Program.WebApi.Testing.Setup.Builders.ObjectBuilder{T}}" />
    public class ObjectBuilder<T> : BaseObjectBuilder<T, ObjectBuilder<T>>
        where T : new()
    {
    }
}
