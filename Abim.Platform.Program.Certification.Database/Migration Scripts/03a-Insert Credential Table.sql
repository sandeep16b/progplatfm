delete from Credential

insert into dbo.Credential ( MemberID, CertificationID, Pathway, Type, Created, CreatedBy, IsActive)
select
	distinct  v.MemberID,
	ct.CertificationID,
	'10Year' as Pathway,
	max(case when e.exam_type in ('A','O','S') then 'Subspecialty'
	else 'General'  end) as Type,
	getdate() as Created,
	'jbauer' as CreatedBy,
	MAX(case ISNULL(c.certificate_status,'HOL')
			when 'HOL' then 1
			else 0 
		end) as IsActive
from
	[Abim.reporting].pub.certif c with (nolock)
join
	[Abim.reporting].pub.exam_info e with (nolock)
on
	c.exam = e.exam
join
	UserProfile v
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct with (nolock)
on
	ct.Name = e.Name
	and
	ct.SourceId in (select SourceId from Source where Code = 'ABIM')
where
	c.exam not in ('94','95','96','97')
group by v.MemberID,
	ct.CertificationID

--CL&I & DLI&I
insert into dbo.Credential ( MemberID, CertificationID, Pathway, Type, Created, CreatedBy, IsActive)
select
	distinct  v.MemberID,
	ct.CertificationID,
	'10Year' as Pathway,
	max(case when e.exam_type in ('A','O','S') then 'Subspecialty'
	else 'General'  end) as Type,
	getdate() as Created,
	'jbauer' as CreatedBy,
	MAX(case ISNULL(c.certificate_status,'HOL')
			when 'HOL' then 1
			else 0 
		end) as IsActive
from
	[Abim.reporting].pub.certif c with (nolock)
join
	[Abim.reporting].pub.exam_info e with (nolock)
on
	c.exam = e.exam
join
	UserProfile v
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct with (nolock)
on
	ct.Name = e.Name
	and
	ct.SourceId in (select SourceId from Source where Code = 'Other')
where
	e.base_exam in ('95','96')
group by v.MemberID,
	ct.CertificationID

	
insert into dbo.Credential ( MemberID, CertificationID, Pathway, Type, Created, CreatedBy, IsActive)
select
	v.MemberID,
	ct.CertificationID,
	'10Year' as Pathway,
	case c.certificate_type when 'G' then 'General' else 'Subspecialty' end as Type,
	getdate() as Created,
	'jbauer' as CreatedBy,
	c.IsActive as IsActive
from
	(select distinct internal_id, certificate_code, CERTIFICATE_TYPE,MAX(case ISNULL(o.STATUS,'AC')
		when 'AC' then 1
		else 0 
	end) as IsActive
	from [Abim.reporting].pub.other_certificates o  with (nolock)
	group by internal_id, certificate_code, CERTIFICATE_TYPE) c
join
	[Abim.reporting].pub.OC_CERT_CODE_LV o with (nolock)
on
	c.CERTIFICATE_CODE = o.CODE 
join
	UserProfile v
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct with (nolock)
on
	cast(cast(o.code as int) as varchar(8))  = ct.code
left join Credential cd
on
	v.MemberId = cd.MemberId
	and
	ct.CertificationId = cd.CertificationId
where
	ct.code <> '1042'
	and
	cd.CredentialId is null
group by v.MemberID,
	ct.CertificationID,
	c.certificate_type,
	c.IsActive