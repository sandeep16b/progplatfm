
delete from Issuance 
where credentialid in 
	(select credentialid 
	from credential 
	where certificationid in (select certificationid from certification where code in ('ALLG','1042')))

delete
	from credential 
	where certificationid in (select certificationid from certification where code in ('ALLG','1042'))


insert into dbo.Credential ( MemberID, CertificationID, Pathway, Type, Created, CreatedBy, IsActive)
select
	distinct  v.MemberID,
	ct.CertificationID,
	'10Year' as Pathway,
	'Subspecialty'as Type,
	getdate() as Created,
	'jbauer' as CreatedBy,
	MAX(IsActive) as IsActive
from
	(select distinct *	,getdate() as Created
	,'jbauer' as CreatedBy from 
(select o.internal_id
,o.CERTIFICATE_DATE
,o.CERTIFICATE_EXP_DATE
,	case isnull(os.CODE,'AC')
		when 'AC' then 'Active'
		when 'EX' then 'Expired'
		when 'RV' then 'Revoked'
		when 'SS' then 'Suspended'
		else 'Inactive' end as certificate_status
,case isnull(os.CODE,'AC')
		when 'AC' then 1
		else 0 
end as IsActive
,'N' as maintain
,case os.code when 'EX' then o.certificate_exp_date else null end as certificate_expired_date
--,o.CERTIFICATE_DATE as CERTIFICATE_EFFECTIVE_DATE
,null as ScheduledUpdate
,case od.code when  'M' then 'Continuous'
	when 'L' then 'Lifetime'
	when 'TL' then 'Timelimited' end as Duration
,'NotRequired' as MaintenanceRequirement
, 'NotMaintained' as MaintenanceStatus
, case occ.code when 'R' then 'Recertification' else 'Initial' end as Occurrence
,0 as UnderReview
,o.certificate_date as certificate_effective_date
from  [Abim.reporting].pub.OTHER_CERTIFICATES o
join  [Abim.reporting].pub.OC_CERT_CODE_LV oc
on o.CERTIFICATE_CODE = oc.CODE
join  [Abim.reporting].pub.OC_CERT_DURATION_LV od
on o.DURATION = od.CODE
join  [Abim.reporting].pub.OC_CERT_OCCURANCE_LV occ
on o.OCCURANCe = occ.CODE
join  [Abim.reporting].pub.OC_CERT_STATUS_LV os
on o.STATUS = os.code
join  [Abim.reporting].pub.OC_CERT_TYPE_LV ot
on o.CERTIFICATE_TYPE= ot.code 
where oc.CODE = 1042
union all
  select c.internal_id
  , c.certificate_date
  , c.certificate_exp_date
  , case isnull(certificate_status,'ACT')
	when 'ACT' then 'Active'
	when 'EXP' then 'Expired'
	when 'REV' then 'Revoked'
	when 'SUS' then 'Suspended'
	when 'SUR' then 'Surrendered'
	when 'HOL' then 'Inactive'
	when 'URC' then 'Active'
	else 'Inactive' end as certificate_status
,case isnull(certificate_status,'ACT')
		when 'ACT' then 1
		else 0 
end as IsActive
  ,isnull( maintain,'N') as maintain
  , certificate_expired_date 
  --, c.CERTIFICATE_EFFECTIVE_DATE
  ,null as ScheduledUpdate
	,case when isnull(c.maintain,'N') = 'Y' then 'Continuous'
		else 
			case when c.CERTIFICATE_EXP_DATE is null 
			then 'Lifetime'
			else 'Timelimited' 
			end
		 end as Duration
,	case isnull(c.MAINTAIN, 'N')
		when 'Y' then 'Required'
		else 'NotRequired'
	end as MaintenanceRequirement
	, 'NotMaintained' as MaintenanceStatus
	, case e.type when 'C' then 'Initial' else 'Recertification' end as Occurrence
	,case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'URC' then 1
		else 0 
	end as UnderReview
	,c.certificate_effective_date
from [Abim.Reporting].[pub].certif c
join [Abim.reporting].pub.exam_info e
on c.exam = e.exam
left join [Abim.reporting].pub.other_certificates oc
on c.internal_id = oc.internal_id
and c.CERTIFICATE_DATE = oc.CERTIFICATE_DATE
and oc.CERTIFICATE_CODE = 1042
where c.exam  in ('94','97')
and oc.CERTIFICATE_CODE is null) a ) c
 join
	UserProfile v
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct
on
	case when year(c.certificate_date) <= 1971
		then  'ALLG'
		else '1042'
	end  = ct.code
group by v.MemberID,
	ct.CertificationID
order by 
	ct.CertificationID


--Source - Name=ABIM - Prior to 1971
--Source - Name=Allergy & Immunology - 1972 and after

insert into dbo.Issuance ([CredentialId],[SourceId],[EffectiveDate],[IssuanceDate],[ExpirationDate],[ScheduledUpdate],[Duration],[MaintenanceRequirement],[MaintenanceStatus],[Occurrence],[IssuanceStatus],[UnderReview],[Created],[CreatedBy])
select
	distinct cd.CredentialId,
	s.SourceId as SourceId,
	isnull(c.certificate_effective_date,c.certificate_date) as EffectiveDate,
	c.CERTIFICATE_DATE as IssuanceDate,
	isnull(c.CERTIFICATE_EXPIRED_DATE,c.CERTIFICATE_EXP_DATE) as ExpirationDate,
	null as ScheduledUpdate,
	c.duration as Duration,
	c.MaintenanceRequirement  as MaintenanceRequirement,
	c.MaintenanceStatus,
	c.Occurrence,
	c.certificate_status as  IssuanceStatus,
	c.UnderReview as UnderReview,
	getdate() as Created,
	'jbauer' as CreatedBy
from
	(select distinct *	,getdate() as Created
	,'jbauer' as CreatedBy from 
(select o.internal_id
,o.CERTIFICATE_DATE
,o.CERTIFICATE_EXP_DATE
,	case isnull(os.CODE,'AC')
		when 'AC' then 'Active'
		when 'EX' then 'Expired'
		when 'RV' then 'Revoked'
		when 'SS' then 'Suspended'
		else 'Inactive' end as certificate_status
,'N' as maintain
,case os.code when 'EX' then o.certificate_exp_date else null end as certificate_expired_date
--,o.CERTIFICATE_DATE as CERTIFICATE_EFFECTIVE_DATE
,null as ScheduledUpdate
,case od.code when  'M' then 'Continuous'
	when 'L' then 'Lifetime'
	when 'TL' then 'Timelimited' end as Duration
,'NotRequired' as MaintenanceRequirement
, 'NotMaintained' as MaintenanceStatus
, case occ.code when 'R' then 'Recertification' else 'Initial' end as Occurrence
,0 as UnderReview
,o.certificate_date as certificate_effective_date
from  [Abim.reporting].pub.OTHER_CERTIFICATES o
join  [Abim.reporting].pub.OC_CERT_CODE_LV oc
on o.CERTIFICATE_CODE = oc.CODE
join  [Abim.reporting].pub.OC_CERT_DURATION_LV od
on o.DURATION = od.CODE
join  [Abim.reporting].pub.OC_CERT_OCCURANCE_LV occ
on o.OCCURANCe = occ.CODE
join  [Abim.reporting].pub.OC_CERT_STATUS_LV os
on o.STATUS = os.code
join  [Abim.reporting].pub.OC_CERT_TYPE_LV ot
on o.CERTIFICATE_TYPE= ot.code 
where oc.CODE = 1042
union all
  select c.internal_id
  , c.certificate_date
  , c.certificate_exp_date
  , case isnull(certificate_status,'ACT')
	when 'ACT' then 'Active'
	when 'EXP' then 'Expired'
	when 'REV' then 'Revoked'
	when 'SUS' then 'Suspended'
	when 'SUR' then 'Surrendered'
	when 'HOL' then 'Inactive'
	when 'URC' then 'Active'
	else 'Inactive' end as certificate_status
  ,isnull( maintain,'N') as maintain
  , certificate_expired_date 
  --, c.CERTIFICATE_EFFECTIVE_DATE
  ,null as ScheduledUpdate
	,case when isnull(c.maintain,'N') = 'Y' then 'Continuous'
		else 
			case when c.CERTIFICATE_EXP_DATE is null 
			then 'Lifetime'
			else 'Timelimited' 
			end
		 end as Duration
,	case isnull(c.MAINTAIN, 'N')
		when 'Y' then 'Required'
		else 'NotRequired'
	end as MaintenanceRequirement
	, 'NotMaintained' as MaintenanceStatus
	, case e.type when 'C' then 'Initial' else 'Recertification' end as Occurrence
	,case isnull(c.CERTIFICATE_STATUS,'ACT')
		when 'URC' then 1
		else 0 
	end as UnderReview
	,c.certificate_effective_date
from [Abim.Reporting].[pub].certif c
join [Abim.reporting].pub.exam_info e
on c.exam = e.exam
left join [Abim.reporting].pub.other_certificates oc
on c.internal_id = oc.internal_id
and c.CERTIFICATE_DATE = oc.CERTIFICATE_DATE
and oc.CERTIFICATE_CODE = 1042
where c.exam  in ('94','97')
and oc.CERTIFICATE_CODE is null) a) c
join
	UserProfile v
on
	c.internal_id = v.AbimId
join
	dbo.Certification ct
on
		case when year(c.certificate_date) <= 1971
		then  'ALLG'
		else '1042'
	end  = ct.code
join
	dbo.Credential cd
on
	v.MemberID = cd.MemberId
	and
	ct.CertificationId = cd.CertificationId 
 join
	dbo.Source s
on
		case when year(c.certificate_date) <= 1971
		then  'ABIM'
		else 'Allergy & Immunology'
	end  = s.Name


update issuance
set scheduledupdate = null
where duration = 'Lifetime'

update c
set IsActive = 1
from Credential c
join  Certification ct
on c.CertificationId = ct.CertificationId
join Issuance i
on c.CredentialId = i.Credentialid
where i.IssuanceStatus = 'Active'
and c.Isactive = 0