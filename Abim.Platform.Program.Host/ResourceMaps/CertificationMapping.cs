using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Extensions.ExternalResponses;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi.Objects.Extensions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class CertificationMapping : Profile
    {
        public CertificationMapping()
        {
            CreateMap<Certification, CertificationSummaryResource>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.BaseCertificationId, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    if(src.BaseCertification != null)
                        dest.BaseCertificationId = src.BaseCertification.ExternalId;
                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                        ProgramResourceConstants.RouteNames.Certifications.GetCertificationById,
                        new Dictionary<string, object> { { "id", src.ExternalId } }
                    )));
                });
            
            CreateMap<Certification, CertificationResource>(MemberList.Destination)
                //Also in Summary:
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .ForMember(dest => dest.BaseCertificationId, opt => opt.Ignore())
                //Details-Only:
                .ForMember(dest => dest.Type, opt => opt.ResolveUsing(src => new EnumValueResponseResource<CertificationType>(src.Type)))
                .ForMember(dest => dest.SourceName, opt => opt.MapFrom(src => src.Source.Name))
                .ForMember(dest => dest.IsCertificateRetired, opt => opt.MapFrom(src => src.IsCertificateRetired && src.CertificateRetiredDate.HasValue && src.CertificateRetiredDate.Value.Date <= DateTime.Now.Date))
                .AfterMap((src, dest, context) =>
                {                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    //Also in Summary
                    {
                        if(src.BaseCertification != null)
                            dest.BaseCertificationId = src.BaseCertification.ExternalId;
                        
                        dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                            ProgramResourceConstants.RouteNames.Certifications.GetCertificationById,
                            new Dictionary<string, object> { { "id", src.ExternalId } }
                        )));
                    }
                    
                    //Source
                    dest.Links.Add(new Link("source",
                                        HttpVerbs.Get,
                                        urlHelper.Link(ProgramResourceConstants.RouteNames.Sources.GetSourceById,
                                        new { id = src.Source.ExternalId })));
                    
                    if (src.BaseCertification != null)
                    {
                        //Base Certification
                        dest.Links.Add(new Link("base",
                                            HttpVerbs.Get,
                                            urlHelper.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertificationById,
                                            new { id = src.BaseCertification.ExternalId })));
                    }
                });
            
            CreateMap<IEnumerable<Certification>, CertificationCollectionResource>(MemberList.Destination)
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
                    
                    string action = ProgramResourceConstants.RouteNames.Certifications.GetCertifications;
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
                        dest.Data.Add(Mapper.Map<Certification, CertificationSummaryResource>(entry, opts => opts.Items["UrlHelper"] = urlHelper));
                    }
                    
                    //Add the links
                    {                        
                        //Self
                        dest.Links.Add(new Link("self", urlHelper.CollectionSelfLinkMethod(),
                            urlHelper.Link(action, new { })));
                    }
                });

            CreateMap<IEnumerable<Certification>, CertificationFullCollectionResource>(MemberList.Destination)
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
                    
                    string action = ProgramResourceConstants.RouteNames.Certifications.GetCertifications;
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
                        dest.Data.Add(Mapper.Map<Certification, CertificationResource>(entry, opts => opts.Items["UrlHelper"] = urlHelper));
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
