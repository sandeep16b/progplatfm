using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.Host.OptionsResources
{
    /// <summary>
    /// 
    /// </summary>
    public class CertificationOptionsResponseResource : ResourceBase
    {
        /// <summary>
        /// Public constructor for deserialization
        /// </summary>
        public CertificationOptionsResponseResource()
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        internal CertificationOptionsResponseResource(UrlHelper urlHelper)
        {
            //Self
            Links.Add(new Link()
            {
                Name = "self",
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.CertificationOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            
            //Enums
            ResourceExtensions.SetEnumLinksForType<Certification>(this, urlHelper, ProgramResourceConstants.RouteNames.Certifications.CertificationEnumValues,
                EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            
            //Get All
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCertifications,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertifications, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //Get All (Post)
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            
            //Get Current User Certifications
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //Get Current User Certifications (Post)
            Links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost,
                Href = urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
        }
    }
}
