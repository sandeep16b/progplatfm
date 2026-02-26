using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.Util;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization; 

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// Represents a collection of <see cref="ProfileShortCollectionResourcePublic"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class ProfileShortCollectionResourcePublic : ResourceBase, IPagedCollectionResource
    {
        //DataMember 1 is reserved for the inherited Links property
        /// <summary>
        /// CurrentPage
        /// </summary> 
        [DataMember(Order = 2)]
        public virtual int CurrentPage { get; set; }
        /// <summary>
        /// TotalPages
        /// </summary> 
        [DataMember(Order = 3)]
        public virtual int TotalPages { get; set; }
        /// <summary>
        /// TotalCount
        /// </summary> 
        [DataMember(Order = 4)]
        public virtual int TotalCount { get; set; }
        /// <summary>
        /// PageSize
        /// </summary> 
        [DataMember(Order = 5)]
        public virtual int PageSize { get; set; }
        /// <summary>
        /// Data
        /// </summary> 
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
        /// <summary>
        /// Id Guid
        /// </summary> 
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual Guid Id { get; set; }
        /// <summary>
        /// AbimId
        /// </summary> 
        [DataMember(Order = 3)]
        public virtual string AbimId { get; set; }
        /// <summary>
        /// LastName
        /// </summary> 
        [DataMember(Order = 4)]
        public virtual string LastName { get; set; }
        /// <summary>
        /// FirstName
        /// </summary> 
        [DataMember(Order = 5)]
        public virtual string FirstName { get; set; }
        /// <summary>
        /// MiddleName
        /// </summary> 
        [DataMember(Order = 6)]
        public virtual string MiddleName { get; set; }
        /// <summary>
        /// MaidenName
        /// </summary> 
        [DataMember(Order = 7)]
        public virtual string MaidenName { get; set; }
        /// <summary>
        /// Suffix
        /// </summary> 
        [DataMember(Order = 8)]
        public virtual string Suffix { get; set; }
        /// <summary>
        /// Salutation
        /// </summary> 
        [DataMember(Order = 9)]
        public virtual string Salutation { get; set; }
        /// <summary>
        /// NameAliases
        /// </summary> 
        [DataMember(Order = 10)]
        public virtual IList<NameResource> NameAliases { get; set; }
        /// <summary>
        /// ImageHref
        /// </summary>
        [DataMember(Order = 11)]
        public virtual string ImageHref { get; set; }
    }

    /// <summary>
    /// A plain POCO type to define a NameResource. This is mapped
    /// to the EntityFramework entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class NameResource
    {
        /// <summary>
        /// FirstName
        /// </summary>
        [DataMember(Order = 1)]
        public virtual string FirstName { get; set; }
        /// <summary>
        /// LastName
        /// </summary>
        [DataMember(Order = 2)] 
        public virtual string LastName { get; set; }
        /// <summary>
        /// MiddleName
        /// </summary>
        [DataMember(Order = 3)]
        public virtual string MiddleName { get; set; }
        /// <summary>
        /// MaidenName
        /// </summary>
        [DataMember(Order = 4)]
        public virtual string MaidenName { get; set; }
        /// <summary>
        /// Salutation
        /// </summary>
        [DataMember(Order = 5)]
        public virtual string Salutation { get; set; }
        /// <summary>
        /// Suffix
        /// </summary>
        [DataMember(Order = 6)]
        public virtual string Suffix { get; set; }
        /// <summary>
        /// SalutationObject
        /// </summary>
        [DataMember(Order = 7)]
        public virtual ProfileNameResourceSalutation SalutationObject { get; set; }
        /// <summary>
        /// SuffixObject
        /// </summary>
        [DataMember(Order = 8)]
        public virtual ProfileNameResourceSuffix SuffixObject { get; set; }
        /// <summary>
        /// DegreeType
        /// </summary>
        [DataMember(Order = 9)]
        public virtual ProfileNameResourceDegreeType DegreeType { get; set; }
        /// <summary>
        /// Honorific
        /// </summary>
        [DataMember(Order = 10)]
        public virtual ProfileNameResourceHonorific Honorific { get; set; }
        /// <summary>
        /// FirstNameSoundex
        /// </summary>
        [DataMember(Order = 11)]
        public virtual string FirstNameSoundex { get; set; }
        /// <summary>
        /// LastNameSoundex
        /// </summary>
        [DataMember(Order = 12)]
        public virtual string LastNameSoundex { get; set; }
        /// <summary>
        /// Id
        /// </summary>
        [DataMember(Order = 13)]
        public int Id { get; set; }
    }

}
