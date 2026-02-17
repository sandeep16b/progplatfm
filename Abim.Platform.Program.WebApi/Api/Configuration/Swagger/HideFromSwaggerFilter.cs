using Abim.Platform.Program.WebApi.Api.Attributes;
using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;

namespace Abim.Platform.Program.WebApi.Api.Configuration.Swagger 
{
    public class HideFromSwaggerFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            foreach (var apiDescription in apiExplorer.ApiDescriptions)
            {
                if(!apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<HideFromSwaggerAttribute>().Any()
                    && !apiDescription.ActionDescriptor.GetCustomAttributes<HideFromSwaggerAttribute>().Any()) continue;
                var route = "/" + apiDescription.Route.RouteTemplate.TrimEnd('/');
                swaggerDoc.paths.Remove(route);
            }
        }
    }
}
