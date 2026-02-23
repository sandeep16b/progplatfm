using HealthChecks.RabbitMQ;
using HealthChecks.SqlServer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;
using Newtonsoft.Json;
using Owin;
using RabbitMQ.Client;
using RimDev.AspNet.Diagnostics.HealthChecks;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// Startup class (partial class)
    /// </summary>
    public partial class Startup
    { 
        public void ConfigurationHealthyCheck(IAppBuilder app)
        {
            var config = ConfigurationManager;
            var amqp = $"amqp://{config.AppSettings["RabbitMQ.User"]}:{config.AppSettings["RabbitMQ.Password"]}@{new Uri(config.AppSettings["RabbitMQ.Path"]).Authority}{new Uri(config.AppSettings["RabbitMQ.Path"]).PathAndQuery}";
            var sslOption = new SslOption(serverName: (new Uri(config.AppSettings["RabbitMQ.Path"])).Host, enabled: Convert.ToBoolean(config.AppSettings["RabbitMQ.UseSSL"]));
            var sqlServer = config.ConnectionStrings["Certification"].ConnectionString;
            var RabbitMQHealthyCheck = new RabbitMQHealthCheck(amqp, sslOption);  
            app.UseHealthChecks(
                   "/health", new HealthCheckOptions() {
                        ResponseWriter = CustomResponseWriter
                   },
                    (new HealthCheckWrapper[] {
                            new HealthCheckWrapper( new SqlServerHealthCheck(sqlServer,"select '1'"),  "SqlServerHealthCheck"),
                            new HealthCheckWrapper( new RabbitMQHealthCheck(amqp, sslOption), "RabbitMQHealthCheck" )
                   }) 
             );  
        } 
        private static Task CustomResponseWriter(IOwinContext context, HealthReport healthReport)
        {
            context.Response.ContentType = "application/json";
            var result = JsonConvert.SerializeObject(new
            {
                OverallStatus = healthReport.Status.ToString(),
                TotalDuration = healthReport.TotalDuration,
                Entries = healthReport.Entries.Select(e => new {
                    ComponentName = e.Key,
                    Duration = e.Value.Duration.ToString(),
                    exception = e.Value.Exception != null ? e.Value.Exception?.InnerException?.ToString() : "",
                    Status = e.Value.Status.ToString()
                })
            });
            return context.Response.WriteAsync(result);
        }

    }

}
