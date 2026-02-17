using Abim.Platform.Program.WebApi.Response;
using System.Net;
using System.Web.Http;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// ApiControllerExtensions Class.
    /// </summary>
    public static class ApiControllerExtensions
    {
        /// <summary>
        /// Accepted method 
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static AcceptedContentResult Accepted(this ApiController controller, string location)
        {
            return new AcceptedContentResult(controller.Request, location);
        }

        /// <summary>
        /// Created method.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static CreatedContentActionResult Created(this ApiController controller, string location)
        {
            return new CreatedContentActionResult(controller.Request, location);
        }

        /// <summary>
        /// Gone method.
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static GoneContentResult Gone(this ApiController controller)
        {
            return new GoneContentResult(controller.Request);
        }
        
        /// <summary>
        /// Text method.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="value"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static TextResult Text(this ApiController controller, string value, HttpStatusCode code)
        {
            return new TextResult(value, controller.Request, code);
        }
    }
}