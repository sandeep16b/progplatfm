using Abim.Platform.Program.Consumers.Observers;
using Abim.Platform.Program.Relational;
using MassTransit;
using MassTransit.NLogIntegration;
using MassTransit.Util;
using System;
using System.Configuration.Abstractions;
using System.Net.Security;
using System.Security.Authentication;


namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// Startup Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// UseMassTransit method.
        /// </summary>
        private void UseServiceBus()
        {
            Logger.Trace("UseMassTrasit Begin");
            
            if (new Feature_MassTransit().FeatureEnabled)
            {
                var container = DependencyResolver.Container;
                Logger.Info("FEATURE :: MASSTRANSIT --> ENABLED");
                var configurationManager = container.GetInstance<IConfigurationManager>();
                var rabbitMqHost = configurationManager.AppSettings["RabbitMQ.Path"];
                var rabbitMqQueue = configurationManager.AppSettings["RabbitMQ.Queue"];
                bool rabbitMqUseSSL = Convert.ToBoolean(configurationManager.AppSettings["RabbitMQ.UseSSL"]);

                var rabbitMqCorrectiveActionQueue = configurationManager.AppSettings["RabbitMQ.Queue.CorrectiveAction"];
                var rabbitMqFPHMAttestInitialQueue = configurationManager.AppSettings["RabbitMQ.Queue.FPHMAttestInitial"];
                var rabbitMqExamResultRelayQueue = configurationManager.AppSettings["RabbitMQ.Queue.ExamResultRelay"];

                var rabbitMqUser = configurationManager.AppSettings["RabbitMQ.User"];
                var rabbitMqPassword = configurationManager.AppSettings["RabbitMQ.Password"];

                //Core.MassTransit.IBusFactory busFactory = container.GetInstance<Core.MassTransit.IBusFactory>();

                IBusControl busControl = Bus.Factory.CreateUsingRabbitMq((busConfigurator) =>
                {
                    busConfigurator.UseNLog();
                    busConfigurator.UseJsonSerializer();
                    var host = busConfigurator.Host(new Uri(rabbitMqHost), hostConfigurator =>
                    {
                        hostConfigurator.Username(rabbitMqUser);
                        hostConfigurator.Password(rabbitMqPassword);
                        if (rabbitMqUseSSL)
                            hostConfigurator.UseSsl(s =>
                            {
                                //During testing, this protocol was the only one that worked reliably
                                s.Protocol = SslProtocols.Tls12;
                                s.AllowPolicyErrors(SslPolicyErrors.RemoteCertificateNameMismatch |
                                                    SslPolicyErrors.RemoteCertificateChainErrors);
                            });
                    });



                });

                // register ReceiveObserver .... 
                busControl.ConnectReceiveObserver(new ProgramReceiveObserver());

                container.Configure(cfg =>
                {
                    cfg.For<IBusControl>().Singleton().Use(busControl);
                    cfg.Forward<IBus, IBusControl>();
                });
                
                try
                {
                    TaskUtil.Await(() => busControl.StartAsync());
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception thrown starting the service bus");
                    throw;
                }
            }
            else
            {
                Logger.Warn("FEATURE :: MASSTRANSIT --> DISABLED");
            }
            
            Logger.Trace("UseMassTrasit Complete");
        }
    }
}
