using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using System.Web.Http.Routing;


namespace Abim.Platform.Program.Host.OptionsResources
{
    /// <summary>
    /// IssuanceOptionsResponseResource
    /// </summary>
    public class IssuanceOptionsResponseResource : ResourceBase
    {
        /// <summary>
        /// Public constructor for deserialization
        /// </summary>
        public IssuanceOptionsResponseResource()
        {
        }
        
        /// <summary>
        /// Main constructor
        /// </summary>
        internal IssuanceOptionsResponseResource(UrlHelper urlHelper)
        {
            //Self
            Links.Add(new Link()
            {
                Name = "self",
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Credentials.IssuanceOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            
            //Enums
            ResourceExtensions.SetEnumLinksForType<Issuance>(this, urlHelper, ProgramResourceConstants.RouteNames.Credentials.IssuanceEnumValues,
                EnumLinkSettings.IncludeNestedEnumTypesInLinks);
        }
    }
}
