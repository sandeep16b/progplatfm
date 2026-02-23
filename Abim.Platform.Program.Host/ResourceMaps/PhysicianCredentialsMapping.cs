using Abim.Enterprise.Core.Profile.Resource;
using Abim.Enterprise.Core.Profile.Resource.Constants;
using Abim.Platform.Program.Resources;
using AutoMapper;
using System.Collections.Generic;
using System.Web.Http.Routing;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class PhysicianCredentialsMapping : Profile
    {
        public PhysicianCredentialsMapping()
        {
            // exact mapping for now, but later we can split to other resouce ...
            CreateMap<ProfileShortCollectionResource, ProfileShortCollectionResourcePublic>(MemberList.Destination)
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages))
                .ForMember(dest => dest.CurrentPage, opt => opt.MapFrom(src => src.CurrentPage))
                .ForMember(dest => dest.TotalCount, opt => opt.MapFrom(src => src.TotalCount))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data));

            // need to adjust self link to point to the current controller ...
            CreateMap<ProfileSummaryShortResource, ProfileSummaryShortResourcePublic>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AbimId, opt => opt.MapFrom(src => src.AbimId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Name.MiddleName))
                .ForMember(dest => dest.MaidenName, opt => opt.MapFrom(src => src.Name.MaidenName))
                .ForMember(dest => dest.Suffix, opt => opt.MapFrom(src => src.Name.Suffix))
                .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.Name.Salutation))
                .ForMember(dest => dest.NameAliases, opt => opt.MapFrom(src => src.NameAliases))
                .AfterMap((src, dest, context) =>
                 {
                     var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;

                     dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                         ProgramResourceConstants.Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId,
                         new Dictionary<string, object> { { "abimId", src.AbimId } }
                     )));

                     //to find if image link is exist
                     if (src.Links.Find(c => c.Name == ProfileResourceConstants.RouteNames.GetUserImage) != null)
                     {
                         dest.Links.Add(new Link("image",
                             HttpVerbs.Get,
                             src.Links.Find(c => c.Name == ProfileResourceConstants.RouteNames.GetUserImage).Href));
                     }

                 });

            CreateMap<ProfileNameAliasSummaryResource, NameResource>(MemberList.Destination)
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.Name.MiddleName))
               .ForMember(dest => dest.MaidenName, opt => opt.MapFrom(src => src.Name.MaidenName))
               .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.Name.Salutation))
               .ForMember(dest => dest.Suffix, opt => opt.MapFrom(src => src.Name.Suffix))

               .ForMember(dest => dest.SalutationObject, opt => opt.MapFrom(src => src.Name.SalutationObject))
               .ForMember(dest => dest.SuffixObject, opt => opt.MapFrom(src => src.Name.SuffixObject))
               .ForMember(dest => dest.DegreeType, opt => opt.MapFrom(src => src.Name.DegreeType))
               .ForMember(dest => dest.Honorific, opt => opt.MapFrom(src => src.Name.Honorific));
        }
    }
}
