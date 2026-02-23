using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Abim.Platform.Program.WebApi.Filters
{
    /// <summary>
    /// ImpersonateMemberIdAttribute Class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ImpersonateMemberIdAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The header key
        /// </summary>
        public const string HeaderKey = "x-impersonateas";

        /// <summary>
        /// The controller argument name
        /// </summary>
        public const string ControllerArgument = "memberId";
        
        /// <summary>
        /// Called when [action executed asynchronous].
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            OnActionExecuting(actionExecutedContext.ActionContext);
        }

        /// <summary>
        /// OnActionExecuting method.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            KeyValuePair<string, IEnumerable<string>> headerItem =
                actionContext.Request.Headers.FirstOrDefault(h => h.Key.ToLower() == HeaderKey);
            if(headerItem.Equals(default(KeyValuePair<string, IEnumerable<string>>)) || headerItem.Key.ToLower() != HeaderKey)
                return;
            
            string headerValue = headerItem.Value.First();
            Guid guid = Guid.Empty;
            if(Guid.TryParse(headerValue, out guid))
            {
                actionContext.ActionArguments[ControllerArgument] = guid;
            }
        }
    }
}