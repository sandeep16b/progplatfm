insert into Credential (MemberId, CertificationId, ForcedPathway, IsActive, Pathway, SelectedToMaintain, Type, Created, CreatedBy)
select u.MemberId, 
	(select CertificationId
	from Certification..Certification
	where Code = 'HOSP') as CertificationId
	,0 as ForcedPathway
	,1 as IsActive
	,'10Year' as Pathway
	,1 as SelectedToMaintain
	,'Subspecialty' as [Type]
	,getdate() as Created
	,'jbauer' as CreatedBy
from UserProfile u
where (exists
	--has attestation
	(select 1
	from Product..Activity ac
	join Product..Product p
	on ac.ProductId = p.ProductId
	where p.Code in ('FPHMAttestInitial','FPHMAttestMOC')
	and ac.ActivityResult = 'P'
	and ac.MemberId = u.MemberId)
	OR
	--passed IM exam
	 exists
	(select 1
	from Registration..Registration r
	join Registration..Administration a
	on r.AdministrationId = a.AdministrationId
	join Registration..Exam e
	on e.ExamId = a.ExamId
	where e.Name = 'Focused Practice in Hospital Medicine'
	and r.ExamResult = 'Pass'
	and r.MemberId = u.MemberId))
and
	not exists
	(select 1
	from Certification..Credential c
	join Certification..Certification ct
	on c.CertificationId = ct.CertificationId
	where ct.Code = 'HOSP'
	and c.MemberId = u.MemberId)