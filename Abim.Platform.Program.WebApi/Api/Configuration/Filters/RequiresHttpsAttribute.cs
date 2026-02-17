using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Abim.Platform.Program.WebApi.Attributes
{
    /// <summary>
    /// RequiresHttpsAttribute class
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequiresHttpsAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresHttpsAttribute"/> class.
        /// </summary>
        public RequiresHttpsAttribute()
        {

        }

        /// <summary>
        /// Occurs before the action method is invoked.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
        }
    }
}
