--FPHM Script

--ExamDueDate

--Initial fphm: if the person has passed the fphm exam, 10 years since last pass otherwise use the IM exam due date
--ALREADY REVIEWED
update ct2
set ExamDueDate = ct1.ExamDueDate
from Credential ct2
join 
	(select ct.CredentialId, 
		case when max(cast(r.ResultDateYear+10 as varchar(4))) is null
			then im.IMDueDate
			else '12/31/' + max(cast(r.ResultDateYear+10 as varchar(4))) 
		end as ExamDueDate
	from Certification c
	join Credential ct
	on 
		c.CertificationId = ct.CertificationId
	left join Issuance i
	on 
		ct.CredentialId = i.CredentialId
		and i.Duration = 'Continuous'
		and i.IssuanceStatus = 'Active'
	join UserProfile u
	on 
		ct.MemberId = u.MemberId
	--check for pass of 10 year Hosp exam
	left join 
		(select r.MemberId, e.CertificationGuid, max(year(ISNULL(s.SeatDate, a.AdministrationDate))) as ResultDateYear
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join dbo.SeatRegistration s
		on r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass'
			and e.ExamType = 'MOC'
		group by r.MemberId, e.CertificationGuid) r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
	join
		--get IM exam due date, just in case they havent passed the FPHM exam
		(select MemberId, ExamDueDate as IMDueDate
		from Credential c
		join Certification ct
		on c.CertificationId = ct.CertificationId
		and ct.Name = 'Internal Medicine') im
	on
		ct.MemberId = im.MemberId
	where 
		--FPHM
		c.Code = 'HOSP'
		----make sure this is an Active, Continuous credential (not a GF or TL)
		--and ct.IsActive = 1
	group by  ct.CredentialId,im.IMDueDate) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.ExamDueDate is null

--Grace Period

--Fphm initial: if the exam due date is 12/31/17 and the person failed fphm in 2017 and had 100 points between 2013 and 2017 and 20 mk points between 2013 and 2017 
--and an attestation 2015 completed between 2015 and  2017
update ct
set GracePeriodStartDate = '1/1/2018',
 GracePeriodEndDate = '12/31/2018'
from Certification c
	join Credential ct
	on 
		c.CertificationId = ct.CertificationId
	left join Issuance i
	on 
		ct.CredentialId = i.CredentialId
		and i.Duration = 'Continuous'
		and i.IssuanceStatus = 'Active'
	join UserProfile u
	on 
		ct.MemberId = u.MemberId
	where 
		--FPHM
		c.Code = 'HOSP'
		and
		--ExamDueDate is '12/31/2017'
		ct.ExamDueDate=  '12/31/2017'
		and 
		--100 points between 2013 and 2017
		exists	
			(select 1
			from Product..Activity a
			where Year(isnull(a.CompletedDate, a.ReceivedDate)) between 2013 and 2017
			group by a.MemberId
			having sum(a.TotalMOCPoints) >= 100
			and a.MemberId = ct.MemberId)
		and 
		--20 MK points between 2013 and 2017
		exists	
			(select 1
			from Product..Activity a2
			join Product..ActivityCredit ac2
			on a2.ActivityId = ac2.ActivityId
			join Product..CreditType c
			on ac2.CreditTypeId = c.CreditTypeId
			where c.Value = 'MK'
			and Year(isnull(a2.CompletedDate, a2.ReceivedDate)) between 2013 and 2017
			and a2.ActivityResult = 'P'
			and ac2.Claimed = 1
			group by a2.MemberId
			having sum(ac2.CreditEarned) >= 20
			and a2.MemberId = ct.MemberId)
		and 
		--Attestation between 2015 and 2017
		exists	
			(select 1
			from Product..Activity a3
			join Product..Product p3
			on a3.ProductId = p3.ProductId
			where p3.Code in ('FPHMAttestInitial','FPHMAttestMOC')
			and Year(isnull(a3.CompletedDate, a3.ReceivedDate)) between 2015 and 2017
			and a3.MemberId = ct.MemberId)
		and
		--check for fail of HOPS exam in 2017
		exists 
		(select 1
			from dbo.Exam e
			join dbo.Administration a
			on e.ExamId = a.ExamId
			join dbo.Registration r
			on a.AdministrationId = r.AdministrationId
			left join dbo.SeatRegistration s
			on r.RegistrationId = s.RegistrationId
			where 
				r.ExamResult = 'Fail'
				and Year(a.AdministrationDate) = 2017
				and r.MemberId = ct.MemberId
				and c.CertificationGuid = e.CertificationGuid)

--AssessmentMet
update ct
set AssessmentMet = 1
from Certification c
join Credential ct
on 
	c.CertificationId = ct.CertificationId
left join Issuance i
on 
	ct.CredentialId = i.CredentialId
	and i.Duration = 'Continuous'
	and i.IssuanceStatus = 'Active'
join UserProfile u
on 
	ct.MemberId = u.MemberId
where 
	--FPHM
	c.Code = 'HOSP'
	--In Grace Period
	and
	 (getdate() between ct.GracePeriodStartDate and ct.GracePeriodEndDate
	 or
	 exists
	 	--check for pass of 10 year Hosp exam 2008 or after
		(select 1
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join dbo.SeatRegistration s
		on r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass'
			and e.ExamType = 'MOC'
			and year(a.AdministrationDate) >= 2008
			and r.MemberId = ct.MemberId
			and e.CertificationGuid = c.CertificationGuid))


--AssessmentMetDate
update ct2
set AssessmentMetDate = ct1.AssessmentMetDate
from Credential ct2
join 
(select ct.CredentialId, 
		dbo.Greatest(isnull(LastHOSPPass, '1/1/1900'), isnull(LastIMPass, '1/1/1900')) as AssessmentMetDate
	from Certification c
	join Credential ct
	on 
		c.CertificationId = ct.CertificationId
	left join Issuance i
	on 
		ct.CredentialId = i.CredentialId
		and i.Duration = 'Continuous'
		and i.IssuanceStatus = 'Active'
	join UserProfile u
	on 
		ct.MemberId = u.MemberId
	--check for pass of 10 year Hosp exam
	left join 
		(select r.MemberId, e.CertificationGuid,  max(ISNULL(s.SeatDate, a.AdministrationDate)) as LastHOSPPass
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join dbo.SeatRegistration s
		on r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass'
			and e.ExamType = 'MOC'
		group by r.MemberId, e.CertificationGuid) hosp
	on
		ct.MemberId = hosp.MemberId
		and
		c.CertificationGuid = hosp.CertificationGuid

	left join 
		(select r.MemberId, max(ISNULL(s.SeatDate, a.AdministrationDate)) as LastIMPass
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		left join dbo.SeatRegistration s
		on r.RegistrationId = s.RegistrationId
		where 
			r.ExamResult = 'Pass'
			and
			e.Name = 'Internal Medicine'
		group by r.MemberId, e.CertificationGuid) im
	on
		ct.MemberId = im.MemberId
	where 
		--FPHM
		c.Code = 'HOSP'
		----make sure this is an Active, Continuous credential (not a GF or TL)
		--and ct.IsActive = 1
	group by  ct.CredentialId, isnull(LastHOSPPass, '1/1/1900'), isnull(LastIMPass, '1/1/1900')) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.ExamDueDate is null