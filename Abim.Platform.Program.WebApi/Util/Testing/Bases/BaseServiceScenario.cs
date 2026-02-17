using Abim.Platform.Program.Relational;
using NUnit.Framework;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// A base class for Service test classes
    /// </summary>
    /// <example>
    /// <code language="C#" title="Example Usage">
    /// <![CDATA[
    ///
    ///     <summary>
    ///     PollyRetryLogicScenario Class.
    ///     </summary>
    ///     public class PollyRetryLogicScenario : BaseServiceScenario
    ///     {
    ///         #region Fields
    ///         
    ///         /// <summary>
    ///         Keeps track of failure.
    ///         </summary>
    ///         internal bool HasAlreadyFailed;
    ///         
    ///         #endregion
    ///         
    ///         #region Properties
    ///         
    ///         /// <summary>
    ///         The service.
    ///         </summary>
    ///         protected PearsonVueRTIService Service { get; set; }
    ///         
    ///         /// <summary>
    ///         The result.
    ///         </summary>
    ///         protected SendExamAuthorizationCommandResult Result { get; set; }
    ///         
    ///         #endregion
    ///         
    ///         /// <summary>
    ///         /// These will be injected as Mocks
    ///         /// </summary>
    ///         protected override List<Type> AdditionalDependencies()
    ///         {
    ///             var types = base.AdditionalDependencies();
    ///             types.Add(typeof(IValidationFactory));
    ///             types.Add(typeof(IPearsonClientProvider));
    ///             types.Add(typeof(IEADService));
    ///             types.Add(typeof(IBusControl));
    ///             types.Add(typeof(IAccessTokenService));
    ///             types.Add(typeof(ILogger));
    ///             types.Add(typeof(IValidator<SendExamAuthorizationCommand>));
    ///             return types;
    ///         }
    ///     
    ///         /// <summary>
    ///         /// Basic setup.
    ///         /// </summary>
    ///         protected override void PreSetup()
    ///         {
    ///             //initialize AutoMapper
    ///             Mapper.Initialize(cfg =>
    ///             {
    ///                 cfg.AddProfile<PearsonMappings>();
    ///             });
    ///         }
    ///     
    ///         /// <summary>
    ///         /// Mock setup.
    ///         /// </summary>
    ///         protected override void PostSetup()
    ///         {
    ///             Service = Container.GetInstance<PearsonVueRTIService>();
    ///             
    ///             //the command will validate successfully
    ///             My<IValidationFactory>()
    ///                 .Setup(o => o.GetValidatorInstance<SendExamAuthorizationCommand>())
    ///                 .Returns(My<IValidator<SendExamAuthorizationCommand>>().Object);
    ///             My<IValidator<SendExamAuthorizationCommand>>()
    ///                 .Setup(o => o.Validate(It.IsAny<SendExamAuthorizationCommand>()))
    ///                 .Returns(new ValidationResult());
    ///             
    ///             //the service will use our Mock EAD service when it looks for an EAD service
    ///             My<IPearsonClientProvider>().Setup(o => o.CreateEADServiceClient(It.IsAny<string>()))
    ///                 .Returns(My<IEADService>().Object);
    ///             
    ///             //watch what gets logged
    ///             Service.Log = My<ILogger>().Object;
    ///             LogTest.Watch(My<ILogger>());
    ///         }
    ///     
    ///         /// <summary>
    ///         /// Given our first call fails but our second try succeeds...
    ///         /// </summary>
    ///         public void GivenPollysRTICallFailsOnce()
    ///         {
    ///             //the first call will throw a TimeoutException
    ///             My<IEADService>().Setup(o => o.HandleImportExamAuthorization(It.Is<eadRequestType>(request => !HasAlreadyFailed)))
    ///                 .Callback(() => { HasAlreadyFailed = true; })
    ///                 .Throws(new TimeoutException());
    ///             
    ///             //the second call will succeed
    ///             My<IEADService>().Setup(o => o.HandleImportExamAuthorization(It.Is<eadRequestType>(request => HasAlreadyFailed)))
    ///                 .Returns(new eadResponseType(){ status = statusType.Accepted, message = "" });
    ///         }
    ///     
    ///         /// <summary>
    ///         /// Whens Service.Handle() is called and Polly is used...
    ///         /// </summary>
    ///         /// <returns></returns>
    ///         public async Task WhenICallHandle()
    ///         {
    ///             var command = SendExamAuthorizationCommandBuilder.Builder()
    ///                 .With(cmd => cmd.AdministrationTests = new List<Domain.AdministrationTest>(){ AdministrationTestBuilder.Build() })
    ///                 .With(cmd => cmd.AlternateDates = null)
    ///                 .With(cmd => cmd.CustomSchedule = null)
    ///                 .Build();
    ///             Result = (SendExamAuthorizationCommandResult)(await Service.Handle(command).ConfigureAwait(false));
    ///         }
    ///     
    ///         /// <summary>
    ///         /// Then:
    ///         /// </summary>
    ///         public void ThenTheCommandResultStatusShouldBeAccepted()
    ///         {
    ///             Result.Status.Should().Be(CommandStatus.Accepted);
    ///         }
    ///         
    ///         /// <summary>
    ///         /// Then:
    ///         /// </summary>
    ///         public void AndThenTheRightWarningShouldHaveBeenLogged()
    ///         {
    ///             LogTest.Warns.Should().Contain(s => s.StartsWith("Polly: EAD RTI call threw a TimeoutException: "));
    ///         }
    ///     }
    ///
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.BaseScenario" />
    public abstract class BaseServiceScenario : BaseScenario
    {
        #region Properties

        /// <summary>
        /// Gets or sets the log test.
        /// </summary>
        /// <value>
        /// The log test.
        /// </value>
        public LogTest LogTest { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseServiceScenario"/> class.
        /// </summary>
        public BaseServiceScenario()
        {
            LogTest = new LogTest();
        }
        
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
