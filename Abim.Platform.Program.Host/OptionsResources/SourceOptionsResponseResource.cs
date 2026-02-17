using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using System.Web.Http.Routing;


namespace Abim.Platform.Program.Host.OptionsResources
{
    /// <summary>
    /// SourceOptionsResponseResource
    /// </summary>
    public class SourceOptionsResponseResource : ResourceBase
    {
        /// <summary>
        /// Public constructor for deserialization
        /// </summary>
        public SourceOptionsResponseResource()
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        internal SourceOptionsResponseResource(UrlHelper urlHelper)
        {
            //Self
            Links.Add(new Link()
            {
                Name = "self",
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Sources.SourceOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            
            //Enums
            ResourceExtensions.SetEnumLinksForType<Source>(this, urlHelper, ProgramResourceConstants.RouteNames.Sources.SourceEnumValues,
                EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            
            //Get All
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Sources.GetSources,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Sources.GetSources, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //Get All (Post)
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Sources.GetSourcesPost,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Sources.GetSourcesPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
        }
    }
}
