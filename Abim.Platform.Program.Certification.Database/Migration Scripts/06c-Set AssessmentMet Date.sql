--Time Limited
--AssessmentMet = passed an exam in last 10 years and after date of most recent tl certification set to true, otherwise set to false
--AssessmentMetDate = For IM/SS disciplines date of last 10 year exam pass

update ct2
set AssessmentMetDate = LastPassDate 
, AssessmentMet = 1
from Credential ct2
join 
	--See Time-Limited rule above
	(select ct.CredentialId, r.LastPassDate
									
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
	--check for last Passed exam result
	join 
		(select r.MemberId, e.CertificationGuid, isnull(s.SeatDate, a.AdministrationDate) as LastPassDate
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join 
			(select RegistrationId, MIN(SeatDate) as SeatDate
			from dbo.SeatRegistration
			where SeatDate is not null
			group by RegistrationId	) s
		on
			r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass') r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
		and
		--make sure the exam date is greater than the previous issuance
		year(isnull(LastPassDate,'1/1/1900'))  > year(i.IssuanceDate) 
		--make sure this is the most recent issuance for this Credential. The check to make sure its TL is below in the where clause
		--as long as I am joining on the max issuance, and filtering i.Duration = 'Timelimited' that should accomplish what you are saying
	join
		(select tl.CredentialId, max(itl.IssuanceId) as MaxIssuanceId
		from Credential tl
		join Issuance itl
		on tl.CredentialId = itl.CredentialId) maxissue
	on
		maxissue.CredentialId = ct.CredentialId
		and
		maxissue.MaxIssuanceId = i.IssuanceId
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--TimeLimited credential (not a GF or MBM)
		and i.Duration = 'Timelimited'
		--make sure this is not a grandfather that voluntarily recertified, different rule for them
		and not exists
			(select 1
			from Credential c2
			join Issuance i2
			on c2.CredentialId = i2.CredentialId
			where c2.MemberId = ct.MemberId
			and i2.Duration = 'Lifetime'
			and c2.CertificationId = ct.CertificationId
			--adding back just for Lifetime credentials
			and i2.IssuanceStatus = 'Active')) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.AssessmentMetDate is null


--MBM
--AssessmentMet = passed an exam in 2008 or later or is in grace period, set to true otherwise set to false
--AssessmentMetDate = For IM/SS disciplines date of last 10 year exam pass

update ct2
set AssessmentMetDate = LastPassDate 
, AssessmentMet = 1
from Credential ct2
join 
	--See Time-Limited rule above
	(select ct.CredentialId, r.LastPassDate
									
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
	--passed an exam, but must do outer join since MBM also considers grace period
	left join 
		(select r.MemberId, e.CertificationGuid, isnull(s.SeatDate, a.AdministrationDate) as LastPassDate
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join 
			(select RegistrationId, MIN(SeatDate) as SeatDate
			from dbo.SeatRegistration
			where SeatDate is not null
			group by RegistrationId	) s
		on
			r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass') r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--MBM credential
		and i.Duration = 'Continuous'
		--make sure this is not a grandfather that voluntarily recertified, different rule for them
		and not exists
			(select 1
			from Credential c2
			join Issuance i2
			on c2.CredentialId = i2.CredentialId
			where c2.MemberId = ct.MemberId
			and i2.Duration = 'Lifetime'
			and c2.CertificationId = ct.CertificationId
			--adding back just for Lifetime credentials
			and i2.IssuanceStatus = 'Active')
		and 
			--passed an exam in 2008 or later or is in grace period, set to true otherwise set to false
			(year(LastPassDate) >= 2008 or getdate() between ct.GracePeriodStartDate and ct.GracePeriodEndDate)) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.AssessmentMetDate is null

--Lifetime (gf) - Set to 1
--per your question on grandfathers. Assessmentmetdate is set for everyone to last pass (initial or mod)
update ct2
set AssessmentMetDate = LastPassDate 
, AssessmentMet = 1
from Credential ct2
join 
	--See Time-Limited rule above
	(select ct.CredentialId, r.LastPassDate
									
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
	--get last passed date for exam
	left join 
		(select r.MemberId, e.CertificationGuid, isnull(s.SeatDate, a.AdministrationDate) as LastPassDate
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join 
			(select RegistrationId, MIN(SeatDate) as SeatDate
			from dbo.SeatRegistration
			where SeatDate is not null
			group by RegistrationId	) s
		on
			r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass') r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--MBM credential
		and i.Duration = 'Lifetime'
		--adding back just for Lifetime credentials
		and i.IssuanceStatus = 'Active') ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.AssessmentMetDate is null
