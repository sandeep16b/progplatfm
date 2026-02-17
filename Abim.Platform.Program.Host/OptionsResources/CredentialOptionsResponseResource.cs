using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.Host.OptionsResources
{
    /// <summary>
    /// CredentialOptionsResponseResource
    /// </summary>
    public class CredentialOptionsResponseResource : ResourceBase
    {
        /// <summary>
        /// Public constructor for deserialization
        /// </summary>
        public CredentialOptionsResponseResource()
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        internal CredentialOptionsResponseResource(UrlHelper urlHelper)
        {
            //Self
            Links.Add(new Link()
            {
                Name = "self",
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.CredentialOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            
            //Enums
            ResourceExtensions.SetEnumLinksForType<Credential>(this, urlHelper, ProgramResourceConstants.RouteNames.Credentials.CredentialEnumValues,
                EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            
            //Get All
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentials,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.GetCredentials, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //Get All (Post)
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            
            //Get Current User Credentials
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //Get Current User Credentials (Post)
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
        }
    }
}
