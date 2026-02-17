using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Objects.Comparison;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.WebApi.Util.Testing.Extensions;
using Moq;
using NUnit.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// The root base class for test classes
    /// </summary>
    public abstract class BaseScenario
    {
        #region Properties

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        protected IContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the random.
        /// </summary>
        /// <value>
        /// The random.
        /// </value>
        protected Random Random { get; set; }

        /// <summary>
        /// Gets or sets the email builder.
        /// </summary>
        /// <value>
        /// The email builder.
        /// </value>
        protected EmailBuilder EmailBuilder { get; set; }

        /// <summary>
        /// Gets or sets the date time builder.
        /// </summary>
        /// <value>
        /// The date time builder.
        /// </value>
        protected DateTimeBuilder DateTimeBuilder { get; set; }

        /// <summary>
        /// Gets or sets the mocks.
        /// </summary>
        /// <value>
        /// The mocks.
        /// </value>
        protected Dictionary<Type, Object> Mocks { get; set; }

        /// <summary>
        /// The exception that occurred, if one occurred.
        /// </summary>
        /// <value>
        /// The exception caught.
        /// </value>
        protected Exception ExceptionCaught { get; set; }

        /// <summary>
        /// The text of the exception, if one occurred.
        /// </summary>
        /// <value>
        /// The exception text.
        /// </value>
        protected string ExceptionText { get; set; }
        
        #endregion

        #region Fields

        #region Settings

        /// <summary>
        /// Whether or not to allow late Mock injections
        /// </summary>
        protected bool AllowLateInjections = true;

        /// <summary>
        /// Whether or not to allow The automatic mock List & IEnumerable methods to return empty lists
        /// </summary>
        protected bool AutoMockEnumerableMethodsToReturnEmptyLists = false;

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseScenario"/> class.
        /// </summary>
        public BaseScenario()
        {
            Random = new Random();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();
            Mocks = new Dictionary<Type, Object>();
        }

        /// <summary>
        /// Gets any additional dependencies that need to be injected, to remove boilerplate code from child classes.
        /// </summary>
        /// <returns></returns>
        protected virtual List<Type> AdditionalDependencies()
        {
            return new List<Type>();
        }

        /// <summary>
        /// Creates the container.
        /// </summary>
        /// <param name="tryUseExistingContainer">if set to <c>true</c> [try use existing container].</param>
        protected void CreateContainer(bool tryUseExistingContainer)
        {
            if(tryUseExistingContainer && DependencyResolver.Container != null)
            {
                Container = DependencyResolver.Container;
            }
            else Container = new Container();
        }

        /// <summary>
        /// Injects the additional dependencies.
        /// </summary>
        protected void InjectAdditionalDependencies()
        {
            var additionalDependencies = AdditionalDependencies();
            if(additionalDependencies == null) return;
            var distincted = additionalDependencies.DistinctOn(t => t.ReadableName());
            foreach(var type in distincted)
            {
                CreateMock(type);
            }
        }
        
        /// <summary>
        /// Adds a new Mock object.
        /// </summary>
        protected void CreateMock(Type type)
        {
            Type mockType = typeof(Mock<>).MakeGenericType(type);
            var mock = (Mock)(mockType.GetConstructor(new Type[0]).Invoke(new object[0]));
            if(AutoMockEnumerableMethodsToReturnEmptyLists)
            {
                typeof(MoqExtensions).GetMethod("AutoMockAllEnumerableMethodsToReturnEmptyLists", BindingFlags.Static).MakeGenericMethod(type)
                    .Invoke(null, mock as dynamic);
            }
            Mocks[type] = mock;
            Container.Inject(type, mock.Object);
        }

        /// <summary>
        /// If a type is in the list returned by AdditionalDependencies(), it will have been automatically injected with a mock
        /// object. To get access to that object, typically to run .Setup() on it, use this method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        protected Mock<T> My<T>()
            where T : class
        {
            if(!Mocks.ContainsKey(typeof(T)))
            {
                if(AllowLateInjections) CreateMock(typeof(T));
                else throw new Exception(string.Format("There is no Type {0} in the Mock collection. Remember to add it to the " +
                    "AdditionalDependencies() list of types to inject", typeof(T).ReadableName()));
            }
            return (Mock<T>)(Mocks[typeof(T)]);
        }

        /// <summary>
        /// Teardown method
        /// </summary>
        [TearDown]
        public abstract void Teardown();
    }
}
