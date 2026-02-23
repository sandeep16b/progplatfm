using Abim.Platform.Program.Util;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NameResource = Abim.Enterprise.Core.Profile.Resource.NameResource;


namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// Represents a collection of <see cref="ProfileShortCollectionResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class ProfileShortCollectionResourcePublic : ResourceBase, IPagedCollectionResource
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
        public virtual List<ProfileSummaryShortResourcePublic> Data { get; set; } = new List<ProfileSummaryShortResourcePublic>();

    }


    /// <summary>
    /// A plain POCO type to define a Profile. This is mapped
    /// to the EntityFramework entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class ProfileSummaryShortResourcePublic : ResourceBase
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual Guid Id { get; set; }

        [DataMember(Order = 3)]
        public virtual string AbimId { get; set; }

        [DataMember(Order = 4)]
        public virtual string LastName { get; set; }

        [DataMember(Order = 5)]
        public virtual string FirstName { get; set; }

        [DataMember(Order = 6)]
        public virtual string MiddleName { get; set; }

        [DataMember(Order = 7)]
        public virtual string MaidenName { get; set; }

        [DataMember(Order = 8)]
        public virtual string Suffix { get; set; }

        [DataMember(Order = 9)]
        public virtual string Salutation { get; set; }

        [DataMember(Order = 10)]
        public virtual IList<NameResource> NameAliases { get; set; }
    }

}
