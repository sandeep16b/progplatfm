using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Extensions.ExternalResponses;
using Abim.Platform.Program.Resources;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class IssuanceMapping : Profile
    {
        public IssuanceMapping()
        {
            CreateMap<Issuance, IssuanceResource>(MemberList.Destination)
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.Duration, opt => opt.ResolveUsing(src => new EnumValueResponseResource<DurationType>(src.Duration)))
                .ForMember(dest => dest.Requirement, opt => opt.ResolveUsing(src => new EnumValueResponseResource<MaintenanceRequirementType>(src.MaintenanceRequirement)))
                .ForMember(dest => dest.Status, opt => opt.ResolveUsing(src => new EnumValueResponseResource<MaintenanceStatusType>(src.MaintenanceStatus)))
                .ForMember(dest => dest.Occurrence, opt => opt.ResolveUsing(src => new EnumValueResponseResource<OccurrenceType>(src.Occurrence)))
                .ForMember(dest => dest.IssuanceStatus, opt => opt.ResolveUsing(src => new EnumValueResponseResource<IssuanceStatusType>(src.IssuanceStatus)))
                .ForMember(dest => dest.CertificationId, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    Credential credential = null;
                    if(context.Options.Items.ContainsKey("Credential"))
                        credential = context.Options.Items["Credential"] as Credential;

                    if(credential != null)
                        dest.CertificationId = credential.Certification.ExternalId;
                    
                    //There are no links on an issuance
                });
            
            CreateMap<IEnumerable<Issuance>, IssuanceCollectionResource>(MemberList.Destination)
                //Defaults assume no paging. If paged, these will be overridden afterward
                .ForMember(dest => dest.TotalPages, opt => opt.ResolveUsing(src => 1))
                .ForMember(dest => dest.CurrentPage, opt => opt.ResolveUsing(src => 1))
                .ForMember(dest => dest.TotalCount, opt => opt.ResolveUsing(src => src.Count()))
                .ForMember(dest => dest.PageSize, opt => opt.ResolveUsing(src => src.Count()))
                .ForMember(dest => dest.Data, opt => opt.Ignore())
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    Credential credential = null;
                    if(context.Options.Items.ContainsKey("Credential"))
                        credential = context.Options.Items["Credential"] as Credential;
                    
                    //Add the collection entries
                    foreach(var entry in src)
                    {
                        if(entry == null)
                        {
                            dest.Data.Add(null);
                            continue;
                        }
                        dest.Data.Add(Mapper.Map<Issuance, IssuanceResource>(entry,
                            opts => { opts.Items["UrlHelper"] = urlHelper; opts.Items["Credential"] = credential; }));
                    }
                    
                    //There are no links on a collection of issuances because a collection of issuances only exists within a credential
                });
        }
    }
}
