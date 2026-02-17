using Abim.Platform.Program.Relational;
using NUnit.Framework;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// A base class for Validator test classes
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.BaseScenario" />
    public abstract class BaseValidationScenario : BaseScenario
    {
        #region Properties
        
        #endregion

        /// <summary>
        /// Anything before the main setup. Abstract.
        /// </summary>
        protected abstract void PreSetup();

        /// <summary>
        /// Anything after the main setup. Abstract. Typically for anything that requires the Container to be ready
        /// </summary>
        protected abstract void PostSetup();

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            //child class start setup
            PreSetup();
            
            CreateContainer(true);
            DependencyResolver.Container = Container;
            InjectAdditionalDependencies();
            
            //child class ending setup
            PostSetup();
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
