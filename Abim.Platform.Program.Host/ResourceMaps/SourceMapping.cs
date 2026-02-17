using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi.Objects.Extensions;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.Host.ResourceMaps
{
    internal class SourceMapping : Profile
    {
        public SourceMapping()
        {
            CreateMap<Source, SourceSummaryResource>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                        ProgramResourceConstants.RouteNames.Sources.GetSourceById,
                        new Dictionary<string, object> { { "id", src.ExternalId } }
                    )));
                });
            
            CreateMap<Source, SourceResource>(MemberList.Destination)
                //Also in Summary:
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Links, opt => opt.Ignore())
                //Details-Only: only auto-mapped properties
                .AfterMap((src, dest, context) =>
                {                    
                    var urlHelper = context.Options.Items["UrlHelper"] as UrlHelper;
                    
                    //Also in Summary
                    {
                        dest.Links.Add(new Link("self", HttpVerbs.Get, urlHelper.Link(
                            ProgramResourceConstants.RouteNames.Sources.GetSourceById,
                            new Dictionary<string, object> { { "id", src.ExternalId } }
                        )));
                    }
                });
            
            CreateMap<IEnumerable<Source>, SourceCollectionResource>(MemberList.Destination)
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
                    
                    string action = ProgramResourceConstants.RouteNames.Sources.GetSources;
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
                        dest.Data.Add(Mapper.Map<Source, SourceResource>(entry, opts => opts.Items["UrlHelper"] = urlHelper));
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
