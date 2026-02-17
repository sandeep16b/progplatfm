update ct
set ReAttestationDueDate = a.DueDate
from 
	(select 
		case AttestationType 
			when 'ACHDAttestInitial' then 'ACHD'
			when 'CCEPAttestInitial' then 'CCEP'
			when 'FPHMAttestInitial' then 'HOSP'
			when 'FPHMAttestMOC' then 'HOSP'
			when 'ICARDAttestInitial' then 'ICARD'
			when 'ICARDAttestMOC' then 'ICARD'
		end as CertificationCode
		,MemberGuid
		,DueDate
	from [AttestationDueDates]) a
join Certification..Credential ct
on a.MemberGuid = ct.MemberId
join Certification..Certification c
on a.CertificationCode = c.Code
and ct.CertificationId = c.CertificationId
