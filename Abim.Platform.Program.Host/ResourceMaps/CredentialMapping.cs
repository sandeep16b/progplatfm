using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Extensions.ExternalResponses;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Objects.Extensions;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class CredentialMapping : Profile
    {
        public CredentialMapping()
        {
            CreateMap<Credential, CredentialSummaryResource>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.CertificationId, opt => opt.Ignore())
                .ForMember(dest => dest.CertificationName, opt => opt.MapFrom(src => src.Certification.Name))
                .ForMember(dest => dest.Issuances, opt => opt.Ignore())
                .ForMember(dest => dest.Pathway, opt => opt.ResolveUsing(src => new EnumValueResponseResource<PathwayType>(src.Pathway)))
                .ForMember(dest => dest.Type, opt => opt.ResolveUsing(src => new EnumValueResponseResource<CredentialType>(src.Type)))
                .AfterMap((src, dest, context) =>
                {
                    if(src.Certification != null)
                        dest.CertificationId = src.Certification.ExternalId;
                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                        ProgramResourceConstants.RouteNames.Credentials.GetCredentialById,
                        new Dictionary<string, object> { { "id", src.ExternalId } }
                    )));
                    
                    dest.Issuances = new List<IssuanceResource>();
                    foreach(var entry in src.Issuances)
                    {
                        dest.Issuances.Add(Mapper.Map<Issuance, IssuanceResource>(entry,
                            opts => { opts.Items["UrlHelper"] = urlHelper; opts.Items["Credential"] = src; }));
                    }
                });
            
            CreateMap<Credential, CredentialResource>(MemberList.Destination)
                //Also in Summary:
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.CertificationId, opt => opt.Ignore())
                .ForMember(dest => dest.Issuances, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.ResolveUsing(src => new EnumValueResponseResource<CredentialType>(src.Type)))
               //Details-Only:
               .ForMember(dest => dest.Pathway, opt => opt.ResolveUsing(src => new EnumValueResponseResource<PathwayType>(src.Pathway)))
                .AfterMap((src, dest, context) =>
                {                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    //Also in Summary
                    {
                        if(src.Certification != null)
                            dest.CertificationId = src.Certification.ExternalId;
                        
                        dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                            ProgramResourceConstants.RouteNames.Credentials.GetCredentialById,
                            new Dictionary<string, object> { { "id", src.ExternalId } }
                        )));

                        dest.Issuances = new List<IssuanceResource>();
                        foreach(var entry in src.Issuances)
                        {
                            dest.Issuances.Add(Mapper.Map<Issuance, IssuanceResource>(entry,
                                opts => { opts.Items["UrlHelper"] = urlHelper; opts.Items["Credential"] = src; }));
                        }
                    }
                    
                    //Certification
                    dest.Links.Add(new Link("certification",
                                       HttpVerbs.Get,
                                       urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertificationById,
                                       new { id = src.Certification.ExternalId })));
                });
            
            CreateMap<IEnumerable<Credential>, CredentialCollectionResource>(MemberList.Destination)
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
                    
                    string action = ProgramResourceConstants.RouteNames.Credentials.GetCredentials;
                    if(context.Options.Items.ContainsKey("Action"))
                        action = context.Options.Items["Action"] as string;
                    
                    //Add the collection entries
                    foreach(var entry in src)
                    {
                        if(entry == null)
                        {
                            dest.Data.Add(null);
                            continue;
                        }
                        dest.Data.Add(Mapper.Map<Credential, CredentialSummaryResource>(entry, opts => opts.Items["UrlHelper"] = urlHelper));
                    }
                    
                    //Add the links
                    {                        
                        //Self
                        dest.Links.Add(new Link("self", urlHelper.CollectionSelfLinkMethod(),
                            urlHelper.Link(action, new { })));
                    }
                });
        }
    }
}
