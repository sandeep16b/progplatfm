using Abim.Platform.Program.Util;
using Abim.Platform.Program.Util.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Resources
{
    //There is no Issuance summary resource because Issuances are only ever returned in a list

    /// <summary>
    /// A plain POCO type to define an Issuance. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class IssuanceResource : ResourceBase
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual int Id { get; set; }
        [DataMember(Order = 3)]
        public virtual Guid CertificationId { get; set; }
        [DataMember(Order = 4)]
        public virtual EnumValueResource<DurationType> Duration { get; set; }
        [DataMember(Order = 5)]
        public virtual EnumValueResource<MaintenanceRequirementType> Requirement { get; set; }
        [DataMember(Order = 6)]
        public virtual EnumValueResource<MaintenanceStatusType> Status { get; set; }
        [DataMember(Order = 7)]
        public virtual EnumValueResource<OccurrenceType> Occurrence { get; set; }
        [DataMember(Order = 8)]
        public virtual EnumValueResource<IssuanceStatusType> IssuanceStatus { get; set; }
        [DataMember(Order = 9)]
        public virtual DateTime IssuanceDate { get; set; }
        [DataMember(Order = 10)]
        public virtual DateTime? ExpirationDate { get; set; }
        [DataMember(Order = 11)]
        public virtual DateTime? EffectiveDate { get; set; }
        [DataMember(Order = 12)]
        public virtual DateTime? ScheduledUpdate { get; set; }
        [DataMember(Order = 13)]
        public virtual bool UnderReview { get; set; }
        [DataMember(Order = 14)]
        public virtual SourceResource Source { get; set; }
        [DataMember(Order = 15)]
        public virtual bool ExpiringThisYear { get; set; }
        [DataMember(Order = 16)]
        public virtual Guid RegistrationGuid { get; set; }
        [DataMember(Order = 17)]
        public virtual DateTime? DeselectionSubmittedDate { get; set; }
        [DataMember(Order = 18)] 
        public virtual DateTime? DeselectionEffectiveDate { get; set; }
        [DataMember(Order = 19)] 
        public virtual DateTime? DeSelectionProcessedDate { get; set; }
        [DataMember(Order = 20)]
        public virtual string IssuanceStatusWithModifier
        {
            get
            {
                switch (IssuanceStatus.Value)
                {
                    case nameof(IssuanceStatusType.Active):
                        return "Certified";
                    case nameof(IssuanceStatusType.Suspended):
                        return "Not Certified, Suspended";
                    case nameof(IssuanceStatusType.Inactive):
                    case nameof(IssuanceStatusType.Surrendered):
                        return "Not Certified";
                    case nameof(IssuanceStatusType.Expired):
                        return "Not Certified, Lapsed";
                    case nameof(IssuanceStatusType.Revoked):
                        return "Not Certified, Revoked";
                    default:
                        return "Not Certified";
                }
            }
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="IssuanceResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class IssuanceCollectionResource : ResourceBase, IPagedCollectionResource
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
        public virtual List<IssuanceResource> Data { get; set; } = new List<IssuanceResource>();

    }
}
