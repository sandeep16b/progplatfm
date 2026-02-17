using Abim.Platform.Program.Util;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// A plain POCO type to define a Source. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class SourceSummaryResource : ResourceBase
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual Guid Id { get; set; }
        [DataMember(Order = 3)]
        public virtual string Name { get; set; }
    }

    /// <summary>
    /// A plain POCO type to define a Source. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class SourceResource : SourceSummaryResource
    {
        [DataMember(Order = 4)]
        public virtual string Code { get; set; }
    }

    /// <summary>
    /// Represents a collection of <see cref="SourceSummaryResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class SourceCollectionResource : ResourceBase, IPagedCollectionResource
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual int CurrentPage { get; set; }
        [DataMember(Order = 3)]
        public virtual int TotalPages { get; set; }
        [DataMember(Order = 4)]
        public virtual int TotalCount { get; set; }
        [DataMember(Order = 5)]
        public virtual int PageSize { get; set; }
        [DataMember(Order = 6)]
        public virtual List<SourceResource> Data { get; set; } = new List<SourceResource>();

    }
}
