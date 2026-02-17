/*Grace Period Start/End Dates:*/

--Clear out dates (remove before production?)
update Credential 
set GracePeriodStartDate = null, GracePeriodEndDate = null


/*
Time-limited (no lifetime) – if active and expiration date was 12/31/17, to 1/1/18-12/31/18, 
	if active and expiration date was 12/31/16 and pending fall 2017 result, to 1/1/17-12/31/17 otherwise set to null
*/

update ct2
set GracePeriodStartDate = ct1.GracePeriodStartDate,
GracePeriodEndDate = ct1.GracePeriodEndDate
from Credential ct2
join 
	--See Time-Limited rule above
	(select ct.CredentialId, case year(i.ExpirationDate)
								when 2017 then '1/1/2018'
								--see WHERE clause below, it only includes 2016 and 2017, hence the else statement below
								else 
									case when r.RegistrationDate is null
										then null
									else
										'1/1/2017'
									end
							end as GracePeriodStartDate
							, case year(i.ExpirationDate)
								when 2017 then '12/31/2018'
								--see WHERE clause below, it only includes 2016 and 2017, hence the else statement below
								else 
									case when r.RegistrationDate is null
										then null
									else
										'12/31/2017'
									end
							end as GracePeriodEndDate
									
	from Certification c
	join Credential ct
	on 
		c.CertificationId = ct.CertificationId
	join Issuance i
	on 
		ct.CredentialId = i.CredentialId
	join UserProfile u
	on 
		ct.MemberId = u.MemberId
	--check for Pending exam result
	left join 
		(select r.MemberId, e.CertificationGuid, r.RegistrationDate
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		where 
			r.ExamResult = 'Pending'
			and e.ExamType = 'MOC') r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
		and
		--make sure the exam date is greater than the previous issuance
		year(isnull(RegistrationDate,'1/1/1900'))  > year(i.IssuanceDate) 
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--make sure this is an Active, TimeLimited credential (not a GF or MBM)
		and i.Duration = 'TimeLimited'
		and i.IssuanceStatus = 'Active'
		and ct.IsActive = 1
		--only include 2016, 2017....everything else remains null
		and year(i.ExpirationDate) in (2016,2017)
		--make sure this is not a grandfather that voluntarily recertified, different rule for them
		and not exists
			(select 1
			from Credential c2
			join Issuance i2
			on c2.CredentialId = i2.CredentialId
			where c2.MemberId = ct.MemberId
			and i2.Duration = 'Lifetime'
			and c2.CertificationId = ct.CertificationId)) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.GracePeriodStartDate is null or ct2.GracePeriodEndDate is null

/*
Continuous (no lifetime) - if the most recent pass of a 10 year exam was in 2007 and cert is active, set to 1/1/18-12/31/18,  
if the most recent pass of a 10 year exam was in 2006 or prior and cert is active, set to 1/1/18-12/31/18,  otherwise set to null

*/

update ct2
set GracePeriodStartDate = '1/1/2018',
GracePeriodEndDate = '12/31/2018'
from Credential ct2
join 
	(select ct.CredentialId		
	from Certification c
	join Credential ct
	on 
		c.CertificationId = ct.CertificationId
	join Issuance i
	on 
		ct.CredentialId = i.CredentialId
	join UserProfile u
	on 
		ct.MemberId = u.MemberId
	--check for most recent Pass exam result 2007 or prior
	join 
		(select r.MemberId, e.CertificationGuid
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		where 
			r.ExamResult = 'Pass'
		group by r.MemberId, e.CertificationGuid
		having 	 max(year(isnull(r.ResultDate, a.AdministrationDate))) <= 2007) r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--make sure this is an Active, MBM/continuous credential (not a GF or TL)
		and i.Duration = 'Continuous'
		and i.IssuanceStatus = 'Active'
		and ct.IsActive = 1
		--make sure this is not a grandfather that voluntarily recertified, different rule for them
		and not exists
			(select 1
			from Credential c2
			join Issuance i2
			on c2.CredentialId = i2.CredentialId
			where c2.MemberId = ct.MemberId
			and i2.Duration = 'Lifetime'
			and c2.CertificationId = ct.CertificationId)) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.GracePeriodStartDate is null or ct2.GracePeriodEndDate is null

--Lifetime (gf) - Set to Null

update c
set GracePeriodStartDate = null, GracePeriodEndDate = null
from Credential c
where exists
	(select 1
	from Issuance i
	where i.IssuanceStatus = 'Active'
	and i.Duration = 'Lifetime'
	and c.CredentialId = i.CredentialId)