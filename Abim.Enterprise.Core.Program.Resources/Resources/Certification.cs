using Abim.Platform.Program.Util;
using Abim.Platform.Program.Util.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// A plain POCO type to define a Certification. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class CertificationSummaryResource : ResourceBase
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual Guid Id { get; set; }
        [DataMember(Order = 3)]
        public virtual string Code { get; set; }
        [DataMember(Order = 4)]
        public virtual string Name { get; set; }
        [DataMember(Order = 5)]
        public virtual Guid? BaseCertificationId { get; set; }
        [DataMember(Order = 6)]
        public virtual string SourceName { get; set; }
    }

    /// <summary>
    /// A plain POCO type to define a Certification. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class CertificationResource : CertificationSummaryResource
    {
        [DataMember(Order = 7)]
        public virtual bool AddedQualification { get; set; }
        [DataMember(Order = 8)]
        public virtual EnumValueResource<CertificationType> Type { get; set; }
        [DataMember(Order = 9)]
        public virtual int? ConsecutiveAttempt { get; set; }
        [DataMember(Order = 10)]
        public virtual bool IsInternalMedicine { get; set; }
        [DataMember(Order = 11)]
        public virtual bool IsSubspecialty { get; set; }
        [DataMember(Order = 12)]
        public virtual bool IsSubspecialtyACHD { get; set; }
        [DataMember(Order = 13)]
        public virtual bool IsSubspecialtySpecificArea { get; set; }
        [DataMember(Order = 14)]
        public virtual bool IsCertificateRetired { get; set; }
    }

    /// <summary>
    /// Represents a collection of <see cref="CertificationSummaryResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class CertificationCollectionResource : ResourceBase, IPagedCollectionResource
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
        public virtual List<CertificationSummaryResource> Data { get; set; } = new List<CertificationSummaryResource>();

    }

    /// <summary>
    /// Represents a collection of <see cref="CertificationResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class CertificationFullCollectionResource : ResourceBase, IPagedCollectionResource
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
        public virtual List<CertificationResource> Data { get; set; } = new List<CertificationResource>();

    }
}
