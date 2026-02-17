using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{    
    /// <summary>
    /// Creates a command of any type, with either valid or invalid values for its properties. Uses reflection to test the properties. For unit testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <example>
    /// <code language="C#" title="Example Usage">
    /// <![CDATA[   
    ///     
    ///     var command = CommandBuilder<UpdateNonAccmeProductCommand>
    ///                     .Valid()
    ///                     .With(cmd => cmd.EffectiveStartDate = DateTime.Now.AddDays(fromNow))
    ///                     .With(cmd => cmd.EffectiveEndDate = DateTime.Now.AddDays(fromNow).AddDays(-1 * offset))
    ///                     .Build();
    /// ]]>
    /// </code>
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.Builders.ObjectBuilder{T}" />
    public class CommandBuilder<T> : BaseObjectBuilder<T, CommandBuilder<T>>
        where T : ICommand, new()
    {
    }
}