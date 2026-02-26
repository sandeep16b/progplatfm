using Abim.Platform.Program.Resources;
using AutoMapper;
using System.Collections.Generic;
using System.Web.Http.Routing;
using Abim.Platform.Program.Util;
using System.Linq;
using Abim.Platform.Program.MembershipClient;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class PhysicianCredentialsMapping : Profile
    {
        public PhysicianCredentialsMapping()
        {
            // exact mapping for now, but later we can split to other resouce ...
            CreateMap<IList<VocProfileResource>, ProfileShortCollectionResourcePublic>(MemberList.Destination)
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.TotalPages, opt => opt.ResolveUsing(src => 1))
                .ForMember(dest => dest.CurrentPage, opt => opt.ResolveUsing(src => 1))
                .ForMember(dest => dest.TotalCount, opt => opt.ResolveUsing(src => src.Count()))
                .ForMember(dest => dest.PageSize, opt => opt.ResolveUsing(src => src.Count()))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));


            // need to adjust self link to point to the current controller ...
            CreateMap<VocProfileResource, ProfileSummaryShortResourcePublic>(MemberList.Destination)
                            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId))
                .ForMember(dest => dest.AbimId, opt => opt.MapFrom(src => src.AbimId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.MaidenName, opt => opt.Ignore())
                .ForMember(dest => dest.Suffix, opt => opt.MapFrom(src => src.Suffix))
                .ForMember(dest => dest.Salutation, opt => opt.Ignore())
                .ForMember(dest => dest.NameAliases, opt => opt.MapFrom(src => new List<NameResource>()))
                .ForMember(dest => dest.ImageHref, opt => opt.MapFrom(src => src.ImageHref))
                .AfterMap((src, dest, context) =>
                 {
                     var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;

                     dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                         ProgramResourceConstants.Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId,
                         new Dictionary<string, object> { { "abimId", src.AbimId } }
                     )));

                     dest.NameAliases.Add(new NameResource()
                     {
                         FirstName = src.AliasFirstName,
                         LastName = src.AliasLastName,
                         Id = src.AliasId,
                         MiddleName = src.AliasMiddleName,
                         Suffix = src.AliasSuffix.ToString(),
                         FirstNameSoundex = src.AliasFirstNameSoundex,
                         LastNameSoundex = src.AliasLastNameSoundex,
                     });

                     //// to find if image link is exist : 
                     //// New Split Profile does not return Image  - Images No more needed. 
                     //if (src.Links.Find(c => c.Name == ProfileResourceConstants.RouteNames.GetUserImage) != null)
                     //{
                     //    dest.Links.Add(new Link("image",
                     //        HttpVerbs.Get,
                     //        src.Links.Find(c => c.Name == ProfileResourceConstants.RouteNames.GetUserImage).Href));
                     //}

                 });

            CreateMap<ProfileAliasResource, NameResource>(MemberList.Destination)
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
               .ForMember(dest => dest.MaidenName, opt => opt.MapFrom(src => src.MaidenName))
               .ForMember(dest => dest.Suffix, opt => opt.MapFrom(src => src.Suffix.ToString()))
               .ForMember(dest => dest.Salutation, opt => opt.MapFrom(src => src.Salutation.ToString()))
               .ForMember(dest => dest.DegreeType, opt => opt.MapFrom(src => src.DegreeType))
               .ForMember(dest => dest.Honorific, opt => opt.MapFrom(src => src.Honorific))
               .ForMember(dest => dest.SalutationObject, opt => opt.MapFrom(src => src.Salutation))
               .ForMember(dest => dest.SuffixObject, opt => opt.MapFrom(src => src.Suffix));

        }
    }
}
