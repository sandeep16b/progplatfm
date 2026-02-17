using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Product.Interservices;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Interservice;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Queries.Base;
using Abim.Platform.Program.Util.Extensions;
using CacheManager.Core;
using NHibernate;
using NLog;
using Polly;
using Polly.Retry;
using StructureMap;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Abim.Platform.Program.Jobs.Config
{
    public partial class Startup
    {
        /// <summary>
        /// Gets or builds the container
        /// </summary>
        /// <returns></returns>
        public static IContainer EnsureIocSetUp()
        {
            if (DependencyResolver.Container == null && !_initializing)
                return UseIoc();
            return DependencyResolver.Container;
        }

        private static bool _initializing;

        /// <summary>
        /// A log used in the creation of the Polly RetryPolicy
        /// </summary>
        private static ILogger InterserviceLog = LogManager.GetLogger("Interservice");

        /// <summary>
        /// Gets or sets the container (private).
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public static IContainer Container { get; set; }

        /// <summary>
        /// UseStructureMap method.
        /// </summary>
        /// <returns></returns>
        protected internal static IContainer UseIoc()
        {
            Logger.Trace("UseStructureMap Begin");
            _initializing = true;

            try
            {
                Container = new Container(c =>
                {
                    c.For<System.Configuration.Abstractions.IConfigurationManager>()
                        .Use(() => ConfigurationManager);

                    c.Scan(scan =>
                    {
                        scan.TheCallingAssembly();
                        scan.AssembliesFromApplicationBaseDirectory();
                        scan.IncludeNamespace("Abim.Platform.Program.App");
                        scan.LookForRegistries();
                        scan.WithDefaultConventions();
                    });

                    c.For<ISessionFactory>().Singleton().Use(ConfigureOrm());
                    c.For<ISession>().Use(ctx => ctx.GetInstance<ISessionFactory>().OpenSession());
                    c.For<IQueryFactory>().Use<QueryFactory>();
                    c.For<Relational.Validation.IValidationFactory>().Use<Relational.Validation.Impl.ValidationFactory>();
                    c.For<FluentValidation.IValidator<AuditData>>().Use<AuditData.AuditDataValidator>();
                    c.For<ICacheManager<Certification>>().Singleton().Use(p => CacheFactory.FromConfiguration<Certification>("cache", UseServiceCacheManagement()));
                    c.For<ICacheManager<Credential>>().Singleton().Use(p => CacheFactory.FromConfiguration<Credential>("cache", UseServiceCacheManagement()));
                    c.For<ICacheManager<Source>>().Singleton().Use(p => CacheFactory.FromConfiguration<Source>("cache", UseServiceCacheManagement()));
                    c.For<IDeselectCertificateChildJob>()
                        .Use<DeselectCertificateChildJob>()
                        .Ctor<string>("profileHostUrl").Is(ConfigurationManager.AppSettings["ProfileHostUrl"])
                        .Ctor<string>("enviroment").Is(ConfigurationManager.AppSettings["Abim.Common.Env"]);

                    //Set up Polly RetryPolicy for the Interservices
                    RetryPolicy retryPolicy = null;
                    if (ConfigurationManager.AppSettings["UsePollyForInterservice"] == "true")
                    {
                        int retries = int.Parse(ConfigurationManager.AppSettings["PollyRetries"]);
                        int pauseInSeconds = int.Parse(ConfigurationManager.AppSettings["PollyInterservicePauseBetweenRetries"]);
                        var pauseTimespan = TimeSpan.FromSeconds(pauseInSeconds);
                        retryPolicy = Policy.Handle<TimeoutException>(ex => LogTimeoutException(ex))
                            .WaitAndRetry(retries, i => pauseTimespan);
                    }
                    
                    var protobufContentType = Interservice.Shared.Interservice.ProtobufContentType;
                    var defaultContentType = Interservice.Shared.Interservice.DefaultContentType;

                    // Product Interservice
                    c.For<Abim.Platform.Product.Interservice.Shared.IInterservice>().Add(() => new Abim.Platform.Product.Interservice.Shared.Interservice(ConfigurationManager.AppSettings["ProductHostUrl"],
                        UseProtocolBuffersForProductInterservice ? Interservice.Shared.Interservice.ProtobufContentType : Interservice.Shared.Interservice.DefaultContentType,
                        UseProtocolBuffersForProductInterservice ? Interservice.Shared.Interservice.ProtobufContentType : Interservice.Shared.Interservice.DefaultContentType))
                        .Named("InterserviceForProduct");
                    c.For<IProductInterservice>().Singleton().Use<ProductInterservice>().Ctor<Abim.Platform.Product.Interservice.Shared.Interservice>().Named("InterserviceForProduct")
                        .SetProperty(i => i.RetryPolicy = retryPolicy); 
                                            
                    //Program Interservice
                    c.For<Interservice.Shared.IInterservice>().Add(() => new Abim.Platform.Program.Interservice.Shared.Interservice(ConfigurationManager.AppSettings["ProgramHostUrl"],
                        UseProtocolBuffersForProgramInterservice ? protobufContentType : defaultContentType,
                        UseProtocolBuffersForProgramInterservice ? protobufContentType : defaultContentType))
                        .Named("InterserviceForProgram");
                    c.For<IProgramInterservice>().Singleton().Use<ProgramInterservice>()
                        .Ctor<Interservice.Shared.IInterservice>().Named("InterserviceForProgram")
                        .SetProperty(i => i.RetryPolicy = retryPolicy);

                    // Registration Interservice
                    c.For<Enterprise.Core.Registration.Interservice.Shared.IInterservice>().Add(() => new Abim.Enterprise.Core.Registration.Interservice.Shared.Interservice(ConfigurationManager.AppSettings["RegistrationHostUrl"],
                        UseProtocolBuffersForRegistrationInterservice ? Interservice.Shared.Interservice.ProtobufContentType : Interservice.Shared.Interservice.DefaultContentType,
                        UseProtocolBuffersForRegistrationInterservice ? Interservice.Shared.Interservice.ProtobufContentType : Interservice.Shared.Interservice.DefaultContentType))
                        .Named("InterserviceForRegistration");

                    c.For<IRegistrationInterservice>().Singleton().Use<RegistrationInterservice>().Ctor<Enterprise.Core.Registration.Interservice.Shared.IInterservice>().Named("InterserviceForRegistration")
                        .SetProperty(i => i.RetryPolicy = retryPolicy);

                    // AccessTokenService
                    c.For<IAccessTokenService>().Singleton().Use<AccessTokenService>();

                    c.For<ITokenClientWraper>().Singleton()
                       .Use<TokenClientWraper>()
                       .Ctor<string>("address").Is(ConfigurationManager.AppSettings["authority"] + "connect/token")
                       .Ctor<string>("clientId").Is(ConfigurationManager.AppSettings["backgroundClientId"])
                       .Ctor<string>("clientSecret").Is(ConfigurationManager.AppSettings["backgroundClientSecret"]);

                    //In order to make IHttpClientFactory work, we must add serviceProvider first.
                    var service = new ServiceCollection();
                    var httpClientFactory = service.AddHttpClient().BuildServiceProvider().GetService<IHttpClientFactory>();
                    c.For<IHttpClientFactory>().Singleton().Use(httpClientFactory);
                    service.AddHttpClient<IHttpClientFactory>("HttpClientFactory", client =>
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    }); 

                    c.For<IApiClientFactory>().Singleton().Use<ApiClientFactory>();

                    // Wrapper class for New Membership 
                    c.For<IClientWrapperService>().Singleton().Use<ClientWrapperService>();

                    c.For<IMembershipClientService>().Singleton().Use<MembershipClientService>()
                          .Ctor<string>("apiBaseUrl").Is(ConfigurationManager.AppSettings["ProfileHostUrl"])
                          .Ctor<int>("pollyRetries").Is(int.Parse(ConfigurationManager.AppSettings["PollyRetries"]));

                    FluentValidation.AssemblyScanner.FindValidatorsInAssemblyContaining<CredentialValidator>()
                        .ForEach(result =>
                        {
                            c.For(result.InterfaceType)
                                .Singleton()
                                .Use(result.ValidatorType);
                        });
                });
                DependencyResolver.Container = Container;
            }
            finally
            {
                _initializing = false;
            }

            Logger.Trace("UseStructureMap Complete");
            return DependencyResolver.Container;
        }

        /// <summary>
        /// Logs a timeout exception.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private static bool LogTimeoutException(TimeoutException ex)
        {
            InterserviceLog.Warn($"Interservice call using Polly threw a TimeoutException: {ex.Stringify()}");
            return true;
        }
    }
}
