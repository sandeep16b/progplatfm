using Abim.Platform.Program.Relational;
using NUnit.Framework;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// A base class for specification unit tests that don't represent scenarios, and don't need setup
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.BaseScenario" />
    public abstract class BaseCheckSpecification : BaseScenario
    {
        #region Properties
        
        #endregion

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            CreateContainer(true);
            DependencyResolver.Container = Container;
            InjectAdditionalDependencies();
        }

        /// <summary>
        /// Teardown method
        /// </summary>
        [TearDown]
        public override void Teardown()
        {
        }
    }
}
