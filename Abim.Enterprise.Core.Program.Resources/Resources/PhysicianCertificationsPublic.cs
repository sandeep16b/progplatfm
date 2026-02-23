using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// A plain POCO type to define a PhysicianCertificationsPublicResource. This is NOT mapped
    /// to the NHibernate entities via AutoMapper.
    /// </summary>
    [DataContract]
    public class PhysicianCertificationsPublicResource
    {
        [DataMember(Order = 1)]
        public List<Link> Links { get; set; }

        [DataMember(Order = 2)]
        public virtual string AbimId { get; set; }
        [DataMember(Order = 3)]
        public virtual string LastName { get; set; }
        [DataMember(Order = 4)]
        public virtual string FirstName { get; set; }
        [DataMember(Order = 5)]
        public virtual string MiddleName { get; set; }
        [DataMember(Order = 6)]
        public virtual string MaidenName { get; set; }
        [DataMember(Order = 7)]
        public virtual string Suffix { get; set; }
        [DataMember(Order = 8)]
        public virtual string Salutation { get; set; }
        [DataMember(Order = 9)]
        public virtual IList<NameAliasPublicResource> NameAliases { get; set; }

        [DataMember(Order = 10)]
        public virtual bool IsActive { get; set; }

        [DataMember(Order = 11)]
        public virtual bool ParticipatingInMOC { get; set; }

        [DataMember(Order = 12)]
        public virtual IList<CertificationPublicResource> Certifications { get; set; }

        [DataMember(Order = 13)]
        public virtual bool? IsFocusPractice { get; set; }
    }

    [DataContract]
    public class NameAliasPublicResource 
    {
        //NOT USED links : DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual string LastName { get; set; }
        [DataMember(Order = 3)]
        public virtual string FirstName { get; set; }
        [DataMember(Order = 4)]
        public virtual string MiddleName { get; set; }
        [DataMember(Order = 5)]
        public virtual string MaidenName { get; set; }
        [DataMember(Order = 6)]
        public virtual string Salutation { get; set; }
        [DataMember(Order = 7)]
        public virtual string Suffix { get; set; }
    }

    [DataContract]
    public class CertificationPublicResource 
    {
        //NOT USED links : DataMember 1 is reserved for the inherited Links property
        [DataMember(Order = 2)]
        public virtual string Name { get; set; }
        [DataMember(Order = 3)]
        public virtual DateTime InitialIssuanceDate { get; set; }
        [DataMember(Order = 4)]
        public virtual IssuanceStatusType Status { get; set; }
        [DataMember(Order = 5)]
        public virtual MaintenanceStatusType MaintenanceStatus { get; set; }
    }
}
