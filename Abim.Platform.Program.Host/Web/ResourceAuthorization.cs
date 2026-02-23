using Abim.Platform.Program.App.Classes;
using Abim.Platform.Program.WebApi.Attributes;
using NLog;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace Abim.Platform.Program.Host.Classes
{
    /// <summary>
    /// Controls access to resources by checking the CurrentPrincipal's claims, and matching them against a list of 
    /// resources and actions supplied by an ResourceAuthorizeAttribute
    /// </summary>
    //the interfaces and classes in the InternalEvents folder are public out of necessity for IConsumer and other constraints
    public class ResourceAuthorization : ResourceAuthorizationManager
    {
        /// <summary>
        /// The log
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// backgroundClientId
        /// </summary>
        protected static string backgroundClientId = ConfigurationManager.AppSettings["backgroundClientId"] ?? "";

        /// <summary>
        /// adminRoleNames
        /// </summary>
        protected static string[] adminRoleNames = ConfigurationManager.AppSettings["AdminGroup"]?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        /// <summary>
        /// Checks the access asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns> <c>true</c> if allowed, otherwise <c>false</c>.</returns>
        public override Task<bool> CheckAccessAsync(ResourceAuthorizationContext context)
        {
            //start logging
            Log.Trace("Started CheckAccessAsync");

            // *** get basic info about ***
            var client = context.Principal.Claims.Where(x => x.Type == "client_id")?.FirstOrDefault()?.Value ?? "";
            var userName = context.Principal.Claims.Where(x => x.Type == "preferred_username")?.FirstOrDefault()?.Value ?? "";
            var requestedBy = $"client:'{client}' userName:'{userName}'";

            //*****************************************************************************************************
            // 1)  Check if the User is Back Ground Client (it is used for interservice calls)
            //*****************************************************************************************************
            //This check is required to allow interservice calls to hit admin endpoints
            // *** check client Ids ***
            var clientIds = context.Principal.Claims.Where(x => x.Type == "client_id");

            // backgroundClientId would have access (used in Interservices)
            if (clientIds.Any(x => x.Value == backgroundClientId))
            {
                Log.Trace($"Granting access to {requestedBy} since it is a backgroundClient.");
                return Ok();
            }

            //*****************************************************************************************************
            // 2) Check if the User/client is part of Any Administrative group
            //*****************************************************************************************************

            // *** find the user's roles ***
            var roleClaimNames = context.Principal.Claims.Where(x => x.Type == ClaimTypes.Role)?.Select(r => r.Value);

            //if they're an admin, they have access to everything
            if (roleClaimNames.Intersect(adminRoleNames).Any())
            {
                Log.Trace($"Granting access to {requestedBy} since it is an admin.");
                return Ok();
            }

            //*****************************************************************************************************
            // 3) Check if the User has proper scopes to access the resource
            //*****************************************************************************************************
            // find if recource (method) have required Scope (example [ResourceAuthorize(Actions.Create, "Certification")])
            // if nothing there then use lowest scope, which is Actions.View
            var scopesRequestedOnResource = context.Action.Where(x => x.Type == "name")?.Select(x => x.Value).FirstOrDefault() ?? Actions.View;
            var isViewType = Actions.IsViewType(scopesRequestedOnResource);

            if (Actions.IsAdminType(scopesRequestedOnResource))
            {
                Log.Warn($"Deny access to {requestedBy} since ONLY Adiminstrative group users/clients can access Admin scope: '{scopesRequestedOnResource}'");
                return Nok();
            }

            // find what scope needed spesific to this platform
            // View scope               --> read        || read_write
            // Not view (update) scope  --> read_write  || write 
            var scopesNeeded = isViewType ? Scopes.readScopes : Scopes.writeScopes;

            //find the user's scopes
            var scopesPresent = context.Principal.Claims.Where(c => c.Type == "scope")?.Select(c => c.Value).ToList();

            if (scopesPresent.Intersect(scopesNeeded).Any())
            {
                Log.Trace($"Granting access to {requestedBy} since scopes '{string.Join(", ", scopesNeeded)}' are present.");
                return Ok();
            }
            // we might need to remove this code below when we fully implement read || write scopes
            else if (scopesPresent.Any(a => a.Contains(Scopes.legacyWebApiScope)))
            {
                // change to Log.Info type when other platform would be capable to send read_write_scopes
                Log.Trace($"Granting access to {requestedBy} since legacy scope 'webapi' still present.");
                return Ok();
            }
            else
            {
                Log.Warn($"Deny access to {requestedBy} since scopes '{string.Join(", ", scopesNeeded)}' are NOT present.");
                return Nok();
            }

            #region Not used for now
            // keep it just in case : we don't verify source name with the scope provided like this (certificate:read)
            // check if name was specified in [ResourceAuthorize("", "resourceName")]
            // use controller name if not found
            //var resourceNameRequestedOnResource = context.Resource.Where(x => x.Type == "name")?.Select(x => x.Value) ?? context.Resource.Where(x => x.Type == "controller").Select(x => x.Value);
            #endregion
        }
    }
}
