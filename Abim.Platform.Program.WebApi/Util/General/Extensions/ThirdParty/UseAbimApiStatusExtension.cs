using Owin;

namespace Abim.Platform.Program.WebApi.Objects.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class UseAbimApiStatusExtension
    {
        /// <summary>
        /// Uses the abim API status.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseAbimApiStatus(this IAppBuilder app)
        {
            app.Map("/api/status", builder =>
            {
                builder.Map("/simple", appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("OK");
                    });
                });
            }); 
        }
    }
}
