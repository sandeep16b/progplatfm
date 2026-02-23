using Abim.Platform.Program.App.ServiceBus;
using Abim.Platform.Program.Consumers.Observers;
using Abim.Platform.Program.Relational;
using MassTransit;
using MassTransit.NLogIntegration;
using MassTransit.Util;
using System;
using System.Configuration.Abstractions;
using System.Net.Security;
using System.Security.Authentication;

namespace Abim.Platform.Program.Jobs.Config
{
    public partial class Startup
    {
        /// <summary>
        /// UseMassTransit method.
        /// </summary>
        private void UseServiceBus()
        {
            Logger.Trace("UseMassTrasit Begin");

            var container = DependencyResolver.Container;
            Logger.Info("FEATURE :: MASSTRANSIT --> ENABLED");
            var configurationManager = container.GetInstance<IConfigurationManager>();
            var rabbitMqHost = configurationManager.AppSettings["RabbitMQ.Path"];
            var rabbitMqQueue = configurationManager.AppSettings["RabbitMQ.Queue"];
            bool rabbitMqUseSSL = Convert.ToBoolean(configurationManager.AppSettings["RabbitMQ.UseSSL"]);

            var rabbitMqRegistrationCreatedQueue = configurationManager.AppSettings["RabbitMQ.Queue.RegistrationCreated"];
            var rabbitMqCorrectiveActionQueue = configurationManager.AppSettings["RabbitMQ.Queue.CorrectiveAction"];
            var rabbitMqFPHMAttestInitialQueue = configurationManager.AppSettings["RabbitMQ.Queue.FPHMAttestInitial"];
            var rabbitMqExamResultRelayQueue = configurationManager.AppSettings["RabbitMQ.Queue.ExamResultRelay"];
            var rabbitMqLngParticipationResultQueue = configurationManager.AppSettings["RabbitMQ.Queue.LngParticipationResult"];
            var rabbitMqLkaEnrollmentQueue = configurationManager.AppSettings["RabbitMQ.Queue.LKAEnrollment"];
            var rabbitMqLkaUnenrollmentQueue = configurationManager.AppSettings["RabbitMQ.Queue.LKAUnenrollment"];

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

                busConfigurator.ReceiveEndpoint(host, rabbitMqRegistrationCreatedQueue, ec =>
                {
                    ec.Consumer<RegistrationCreatedEventConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqCorrectiveActionQueue, ec =>
                {
                    ec.Consumer<CorrectiveActionConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqFPHMAttestInitialQueue, ec =>
                {
                    ec.Consumer<FPHMAttestInitialEventConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqExamResultRelayQueue, ec =>
                {
                    ec.Consumer<ExamResultRelayConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqLngParticipationResultQueue, ec =>
                {
                    ec.Consumer<LngParticipationResultConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqLkaEnrollmentQueue, ec => 
                {
                    ec.Consumer<LKAEnrollmentConsumer>(container);
                });

                busConfigurator.ReceiveEndpoint(host, rabbitMqLkaUnenrollmentQueue, ec =>
                {
                    ec.Consumer<LKAUnenrollmentConsumer>(container);
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
            catch (Exception)
            {
                Logger.Error("Exception thrown starting the service bus");
                throw;
            }

            Logger.Trace("UseMassTrasit Complete");
        }
    }
}
