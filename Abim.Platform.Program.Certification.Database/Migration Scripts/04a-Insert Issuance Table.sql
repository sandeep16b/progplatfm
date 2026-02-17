
delete from dbo.Issuance

Declare @SourceId int
Select @SourceId = SourceId from dbo.Source where Code = 'ABIM'

insert into dbo.Issuance ([CredentialId],[SourceId],[EffectiveDate],[IssuanceDate],[ExpirationDate],[ScheduledUpdate],[Duration],[MaintenanceRequirement],[MaintenanceStatus],[Occurrence],[IssuanceStatus],[UnderReview],[Created],[CreatedBy])
select
	distinct cd.CredentialId,
	@SourceId as SourceId,
	isnull(c.CERTIFICATE_EFFECTIVE_DATE,c.CERTIFICATE_DATE) as EffectiveDate,
	c.CERTIFICATE_DATE as IssuanceDate,
	c.CERTIFICATE_EXP_DATE as ExpirationDate,
	c.REVERIFICATION_DATE  as ScheduledUpdate,
	case when isnull(c.maintain,'N') = 'Y' then 'Continuous'
	else 
		case when c.CERTIFICATE_EXP_DATE is null 
		then 'Lifetime'
		else 'Timelimited' 
		end
	 end as Duration,
	case isnull(c.MAINTAIN, 'N')
		when 'Y' then 'Required'
		else 'NotRequired'
	end as MaintenanceRequirement,
	case when 
		isnull(c.maintain,'N') = 'N' and c.CERTIFICATE_EXP_DATE is null 
		then 'Maintained'
	else
		case when isnull(m.meeting, 'N') = 'Y' and isnull(c.CERTIFICATE_STATUS, 'URC') = 'URC'
			then 'Maintained'
			else 'NotMaintained'
		end
	end as MaintenanceStatus,
	case when c.exam ='A0' then 'Recertification'
		else case when e.type in ('C','F') then 'Initial'
			else 'Recertification' end 
	END as Occurrence,
	case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'ACT' then 'Active'
		when 'EXP' then 'Expired'
		when 'HOL' then 'Inactive'
		when 'REV' then 'Revoked'
		when 'SUR' then 'Surrendered'
		when 'SUS' then 'Suspended'
		when 'URC' then 'Active'
		else 'Inactive' end as IssuanceStatus,
	case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'URC' then 1
		else 0 
	end as UnderReview,
	getdate() as Created,
	'jbauer' as CreatedBy
from
	[Abim.reporting].pub.certif c with (nolock)
join
	[Abim.reporting].pub.exam_info e with (nolock)
on
	c.exam = e.exam
left join
	[Abim.reporting].pub.meeting_moc m with (nolock)
on
	case when c.exam ='A0' then c.exam else e.base_exam end = m.exam
	and
	m.is_current = 'Y'
	and
	c.internal_id = m.internal_id
join
	UserProfile v with (nolock)
on
	v.AbimId = c.internal_id
join
	dbo.Certification ct with (nolock)
--CHECK TO MAKE SURE THIS CAPTURES ALL
on
	e.Name = ct.Name
	and 
	ct.SourceId  = @SourceId
join
	dbo.Credential cd with (nolock)
on
	v.MemberId = cd.MemberId
	and
	ct.CertificationId = cd.CertificationId 
order by 
	cd.CredentialId

--Declare @SourceId int
Select @SourceId = SourceId from dbo.Source where Code = 'Other'

--CL&I, DL&I
insert into dbo.Issuance ([CredentialId],[SourceId],[EffectiveDate],[IssuanceDate],[ExpirationDate],[ScheduledUpdate],[Duration],[MaintenanceRequirement],[MaintenanceStatus],[Occurrence],[IssuanceStatus],[UnderReview],[Created],[CreatedBy])
select
	distinct cd.CredentialId,
	@SourceId as SourceId,
	isnull(c.CERTIFICATE_EFFECTIVE_DATE,c.CERTIFICATE_DATE) as EffectiveDate,
	c.CERTIFICATE_DATE as IssuanceDate,
	c.CERTIFICATE_EXP_DATE as ExpirationDate,
	c.REVERIFICATION_DATE  as ScheduledUpdate,
	case when isnull(c.maintain,'N') = 'Y' then 'Continuous'
	else 
		case when c.CERTIFICATE_EXP_DATE is null 
		then 'Lifetime'
		else 'Timelimited' 
		end
	 end as Duration,
	'NotRequired' as MaintenanceRequirement,
	'NotMaintained' as MaintenanceStatus,
	case when e.type in ('C','F') then 'Initial'
		else 'Recertification' end as Occurrence,
	case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'ACT' then 'Active'
		when 'EXP' then 'Expired'
		when 'HOL' then 'Inactive'
		when 'REV' then 'Revoked'
		when 'SUR' then 'Surrendered'
		when 'SUS' then 'Suspended'
		else 'Inactive' end as IssuanceStatus,
	case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'URC' then 1
		else 0 
	end as UnderReview,
	getdate() as Created,
	'jbauer' as CreatedBy
from
	[Abim.reporting].pub.certif c with (nolock)
join
	[Abim.reporting].pub.exam_info e with (nolock)
on
	c.exam = e.exam
join
	UserProfile v with (nolock)
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct with (nolock)
on
	e.Name = ct.Name
	and 
	ct.SourceId  = @SourceId
join
	dbo.Credential cd with (nolock)
on
	v.MemberID = cd.MemberId
	and
	ct.CertificationId = cd.CertificationId 
	and
	e.base_exam in ('95','96')
--24B8992E-6F56-4A68-A723-FB568B798EC0	Continuous
--24B8992E-6F56-4A68-A723-FB568B798EC0	Lifetime
--24B8992E-6F56-4A68-A723-FB568B798EC0	Timelimited



insert into dbo.Issuance ([CredentialId],[SourceId],[EffectiveDate],[IssuanceDate],[ExpirationDate],[ScheduledUpdate],[Duration],[MaintenanceRequirement],[MaintenanceStatus],[Occurrence],[IssuanceStatus],[UnderReview],[Created],[CreatedBy])
select
distinct	cd.CredentialId,
	ct.SourceId as SourceId,
	c.CERTIFICATE_DATE as EffectiveDate,
	c.CERTIFICATE_DATE as IssuanceDate,
	c.CERTIFICATE_EXP_DATE as ExpirationDate,
	null as ScheduledUpdate,
	case c.DURATION 
		when 'TL' then 'Timelimited'
		when 'L' then 'Lifetime'
		when 'M' then 'Continuous'
	else 'Timelimited' end as Duration,
	'NotRequired' as MaintenanceRequirement,
	'NotMaintained' as MaintenanceStatus,
	case when c.OCCURANCE = 'I' then 'Initial'
		else 'Recertification' end as Occurrence,
	case isnull(c.STATUS,'AC')
		when 'AC' then 'Active'
		when 'EX' then 'Expired'
		when 'RV' then 'Revoked'
		when 'SS' then 'Suspended'
		else 'Inactive' end as IssuanceStatus,
	0  as UnderReview,
	getdate() as Created,
	'jbauer' as CreatedBy
--into tmpIssuance
from
	[Abim.reporting].pub.OTHER_CERTIFICATES c with (nolock)
join
	[Abim.reporting].pub.OC_CERT_CODE_LV o with (nolock)
on
	c.CERTIFICATE_CODE = o.CODE 
join
	UserProfile v with (nolock)
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct with (nolock)
on
	cast(cast(o.code as int) as varchar(8))  = ct.code
join
	dbo.Credential cd with (nolock)
on
	v.MemberID = cd.MemberId
	and
	ct.CertificationId = cd.CertificationId 
left join
	Issuance i with (nolock)
on
	cd.CredentialId = i.CredentialId
	and
	c.CERTIFICATE_DATE = i.IssuanceDate
where 
	i.IssuanceId is null


--clear dup certs between Other and CERTIF
delete from Issuance where IssuanceId in (
select min(IssuanceId)  
from issuance 
group by credentialid, effectivedate, issuancedate 
having count(*) > 1)


update issuance
set scheduledupdate = null
where duration = 'Lifetime'

update i
set Occurrence = 'Recertification'
from Issuance i
join Credential c
on i.CredentialId = c.CredentialId
join Certification ct
on c.CertificationId = ct.CertificationId
where 
	ct.Code = 'HOSP'
	and
	i.Occurrence <> 'Recertification'

--update [Certification].[dbo].[Issuance]
--set Occurrence = 'Recertification'
--where credentialid in (select credentialid from credential where select * from certification where certificationid = 36)


--24B8992E-6F56-4A68-A723-FB568B798EC0	Continuous
--24B8992E-6F56-4A68-A723-FB568B798EC0	Lifetime
--24B8992E-6F56-4A68-A723-FB568B798EC0	Timelimited