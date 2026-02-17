using Abim.Platform.Program.Util;
using Abim.Platform.Program.Util.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// A plain POCO type to define a Credential. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class CredentialSummaryResource : ResourceBase
    {
        //DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual Guid Id { get; set; }
        [DataMember(Order = 3)]
        public virtual Guid CertificationId { get; set; }
        [DataMember(Order = 4)]
        public virtual string CertificationName { get; set; }
        [DataMember(Order = 5)]
        public virtual Guid MemberId { get; set; }
        [DataMember(Order = 6)]
        public virtual CertificationResource Certification { get; set; }
        [DataMember(Order = 7)]
        public virtual bool IsActive { get; set; }
        [DataMember(Order = 8)]
        public virtual DateTime? GracePeriodStartDate { get; set; }
        [DataMember(Order = 9)]
        public virtual DateTime? GracePeriodEndDate { get; set; }
        [DataMember(Order = 10)]
        public virtual DateTime? ExamDueDate { get; set; }
        [DataMember(Order = 11)]
        public virtual List<IssuanceResource> Issuances { get; set; }
        [DataMember(Order = 12)]
        public virtual EnumValueResource<PathwayType> Pathway { get; set; }
        [DataMember(Order = 13)]
        public virtual bool ForcedPathway { get; set; }
        [DataMember(Order = 14)]
        public virtual DateTime? ReAttestationDueDate { get; set; }
        [DataMember(Order = 15)]
        public virtual bool SelectedToMaintain { get; set; }
        [DataMember(Order = 16)]
        public virtual bool IsRevokedOrSurrendered { get; set; }
        [DataMember(Order = 17)]
        public virtual EnumValueResource<CredentialType> Type { get; set; }
        [DataMember(Order = 18)]
        public virtual DateTime? KCIExamDueDate { get; set; }
        [DataMember(Order = 19)]
        public virtual DateTime? MOCExamDueDate { get; set; }
        [DataMember(Order = 20)]
        public virtual DateTime? DisplayExamDueDate { get; set; }
        [DataMember(Order = 21)]
        public virtual bool ConsecutiveKCIPassRequired { get; set; }
        [DataMember(Order = 22)]
        public virtual bool IsInCMP { get; set; }
        [DataMember(Order = 23)]
        public virtual bool AssessmentMet { get; set; }
        [DataMember(Order = 24)]
        public virtual DateTime? AssessmentMetDate { get; set; }
        [DataMember(Order = 25)]
        public virtual bool DeselectionElected { get; set; }
        [DataMember(Order = 26)]
        public virtual DateTime? DeselectionElectedDate { get; set; }
        [DataMember(Order = 27)]
        public virtual bool DeselectionProcessed { get; set; }
        [DataMember(Order = 28)]
        public virtual DateTime? DeselectionProcessedDate { get; set; }

        //pbi 210449 (Proj 1473) Remove unnecessary elements from homepage/menu for Cosponsored physicians
        [DataMember(Order = 29)]
        public virtual bool IsCosponsored { get; set; }

        [DataMember(Order = 30)]
        public virtual string OnBehalfBoardCode { get; set; }

        [DataMember(Order = 31)]
        public virtual string OnBehalfBoardName { get; set; }
    }

    /// <summary>
    /// A plain POCO type to define a Credential. This is mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class CredentialResource : CredentialSummaryResource
    {
        [DataMember(Order = 29)]
        public virtual DateTime? LookbackDate { get; set; }
        [DataMember(Order = 30)]
        public virtual DateTime? WithdrawnDate { get; set; }
    }

    /// <summary>
    /// Represents a collection of <see cref="CredentialSummaryResource"/> objects.
    /// While this is only really a typedef, its definition is used by AutoMapper so
    /// it is important that we define it.
    /// </summary>
    [DataContract]
    public class CredentialCollectionResource : ResourceBase, IPagedCollectionResource
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
        public virtual List<CredentialSummaryResource> Data { get; set; } = new List<CredentialSummaryResource>();

    }
}
