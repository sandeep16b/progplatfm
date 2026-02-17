--ExamDueDate:

/*--Time limited (no lifetime) set to 12/31/ greatest of most recent certificate expiration date and most recent 10 year pass + 10*/
/*--Grandfather set to 12/31 greatest 2023 and most recent 10 year pass + 10*/
/*--MBM set to 12/31/ most recent 10 year pass + 10*/

--TimeLimited
update ct2
set ExamDueDate = convert(datetime2, ct1.ExamDueDate)
from Credential ct2
join 
	(select  ct.CredentialId, dbo.Greatest(cast('12/31/' + max(cast(r.ResultDateYear+10 as varchar(4))) as date), cast('12/31/' + cast(max(year(i.ExpirationDate)) as varchar(4)) as date)) as ExamDueDate
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
       --check for pass of 10 year
      left  join 
              (select r.MemberId, e.CertificationGuid, max(year(a.AdministrationDate)) as ResultDateYear
              from dbo.Exam e
              join dbo.Administration a
              on e.ExamId = a.ExamId
              join dbo.Registration r
              on a.AdministrationId = r.AdministrationId
              where 
                     r.ExamResult = 'Pass'
                     and e.ExamType in ('Cert', 'MOC')
              group by r.MemberId, e.CertificationGuid) r
       on
              ct.MemberId = r.MemberId
              and
              c.CertificationGuid = r.CertificationGuid
              and
              --make sure the exam date is greater than the previous issuance
              isnull(ResultDateYear, 1900) > year(i.IssuanceDate) 
       where 
              --exclude FPHM, has different rules so this will be done separately
              c.Code <> 'HOSP'
              --make sure this is an Active, TimeLimited credential (not a GF or MBM)
              and i.Duration = 'TimeLimited'
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
                     and c2.CertificationId = ct.CertificationId)
              --and
              --       u.AbimId='196564'
       group by  ct.CredentialId) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.ExamDueDate is null



--Grandfather
update ct2
set ExamDueDate = ct1.ExamDueDate
from Credential ct2
join 
	(select ct.CredentialId, 
		case when isnull('12/31/' + max(cast(r.ResultDateYear+10 as varchar(4))), '12/31/1900') > '12/31/2023'
			 then isnull('12/31/' + max(cast(r.ResultDateYear+10 as varchar(4))), '12/31/1900')
		else
			'12/31/2023'
		end as ExamDueDate
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
	--check for pass of 10 year
	left join 
		(select r.MemberId, e.CertificationGuid, max(year(a.AdministrationDate)) as ResultDateYear
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		where 
			r.ExamResult = 'Pass'
			and e.ExamType in ('Cert', 'MOC')
		group by r.MemberId, e.CertificationGuid) r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
		and
		--make sure the exam date is greater than the previous issuance
		isnull(ResultDateYear, 1900) > year(i.IssuanceDate) 
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--make sure this is an Active, Lifetime credential (not a TL or MBM)
		and i.Duration = 'Lifetime'
		and i.IssuanceStatus = 'Active'
		and ct.IsActive = 1
	group by  ct.CredentialId) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.ExamDueDate is null


--MBM (Continuous)
update ct2
set ExamDueDate = ct1.ExamDueDate
from Credential ct2
join 
	(select ct.CredentialId, 
		'12/31/' + max(cast(r.ResultDateYear+10 as varchar(4))) as ExamDueDate
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
	--check for pass of 10 year
	join 
		(select r.MemberId, e.CertificationGuid, max( year(a.AdministrationDate)) as ResultDateYear
		from dbo.Exam e
		join dbo.Administration a
		on e.ExamId = a.ExamId
		join dbo.Registration r
		on a.AdministrationId = r.AdministrationId
		where 
			r.ExamResult = 'Pass'
			and e.ExamType in ('Cert', 'MOC')
		group by r.MemberId, e.CertificationGuid) r
	on
		ct.MemberId = r.MemberId
		and
		c.CertificationGuid = r.CertificationGuid
		and
		--make sure the exam date is greater than the previous issuance
		isnull(ResultDateYear, 1900) > year(i.IssuanceDate) 
	where 
		--exclude FPHM, has different rules so this will be done separately
		c.Code <> 'HOSP'
		--make sure this is an Active, Continuous credential (not a GF or TL)
		and i.Duration = 'Continuous'
		and i.IssuanceStatus = 'Active'
		and ct.IsActive = 1
	group by  ct.CredentialId) ct1
on 
	ct1.CredentialId = ct2.CredentialId
where 
	ct2.ExamDueDate is null



	--NEED TO REMOVE THIS FOR PRODUCTION ONCE EXAM_HIST/REGIS CLEANUP IS DONE

--DO I NEED THIS TO CATCH ANY THAT MAY HAVE BEEN MISSED BY THE ABOVE UPDATES? OR SHOULD THOSE ABOVE HAVE CAPTURED EVERYTHING NON-FPHM?
update c1
set ExamDueDate = c2.ExamDueDate
from Credential c1
join
	(select c.CredentialId, '12/31/' + cast(max(year(i.IssuanceDate)) +10 as varchar(4)) as ExamDueDate
	from credential c
	join issuance i
	on c.CredentialId = i.CredentialId
	join UserProfile u
	on c.MemberId = u.MemberId
	join Certification ct
	on c.CertificationId = ct.CertificationId
	where isactive = 1
	and examduedate is null
	and i.IssuanceStatus = 'Active'
	and ct.Code <> 'HOSP'
	group by u.AbimId, c.CredentialId, i.Duration) c2
on
	c1.CredentialId = c2.CredentialId