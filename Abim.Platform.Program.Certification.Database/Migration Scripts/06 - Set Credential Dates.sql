/*SelectedToMaintain

*/
if  object_id('dbo.Exam','SN') is not null
begin

	--No 90 record ever, just select all
	update c
	set SelectedToMaintain = 1
	from Credential c
	join UserProfile u
	on c.MemberId = u.MemberId
	left join [Abim.Reporting].pub.regis r
	on cast(r.internal_id as int) = u.AbimId
	and r.exam = '90'
	where r.INTERNAL_ID is null

	--Active areas is null
	update c
	set SelectedToMaintain = 1
	from Credential c
	join UserProfile u
	on c.MemberId = u.MemberId
	join Certification ct
	on c.CertificationId = ct.CertificationId
	where exists
		--Check latest 90 record's active areas
		(select 1
		from [Abim.Reporting].pub.regis r1
		--if null, just set everything to 1
		where cast(r1.INTERNAL_ID as int) = u.AbimId
		and r1.active_areas is null
		and r1.EXAM = '90'
		and r1.REG_DATE = 
			(select max(r2.REG_DATE)
			from [Abim.Reporting].pub.regis r2
			where r2.internal_id = r1.internal_id
			and r2.exam = '90')) 

	--Active areas is not null
	update c
	set SelectedToMaintain = 1
	from Credential c
	join UserProfile u
	on c.MemberId = u.MemberId
	join Certification ct
	on c.CertificationId = ct.CertificationId
	join dbo.Exam e
	on e.CertificationGuid = ct.CertificationGuid
	where exists
		--Check latest 90 record's active areas
		(select 1
		from [Abim.Reporting].pub.regis r1
		where cast(r1.INTERNAL_ID as int) = u.AbimId
		--exam selected in latest 90 record
		and r1.active_areas like '%' + e.ExamCode + '%'
		and r1.EXAM = '90'
		and r1.REG_DATE = 
			(select max(r2.REG_DATE)
			from [Abim.Reporting].pub.regis r2
			where r2.internal_id = r1.internal_id
			and r2.exam = '90')) 

	
end


--Updated Selected to Maintain based on rule in task 103952

update c
set SelectedToMaintain = 1
from UserProfile u
join Credential c
on u.MemberId = c.MemberId
join Certification ct
on c.CertificationId = ct.CertificationId
	where exists
		--Check latest 90 record's active areas
		(select 1
		from [Abim.Reporting].pub.regis r1
		where cast(r1.INTERNAL_ID as int) = u.AbimId
		--FPHM
		and r1.focusarea = 'A0'
		and r1.EXAM = '90'
		and r1.REG_DATE = 
			(select max(r2.REG_DATE)
			from [Abim.Reporting].pub.regis r2
			where r2.internal_id = r1.internal_id
			and r2.exam = '90'))
and ct.Code = 'HOSP'

--IM and FPHM cannot be selected at the same time
update c
set SelectedToMaintain = 0
from UserProfile u
join Credential c
on u.MemberId = c.MemberId
join Certification ct
on c.CertificationId = ct.CertificationId
and ct.Code = 'IM'
and exists
	(select 1
	from Credential c2
	join Certification ct2
	on c2.CertificationId = ct2.CertificationId
	and ct2.Code = 'HOSP'
	and c2.SelectedToMaintain = 1
	and c2.MemberId = c.MemberId)


/*ForcedTo10Year:
	Set to 'N' (0)
*/

update ct
set ForcedPathway = 0
from Credential ct
join Certification c
on ct.CertificationId = c.CertificationId
join Source s
on c.SourceId = s.SourceId
where ForcedPathway is null
and s.Code = 'ABIM'

/*LookbackDate:
	Set to last lookback date in oracle, should be 12/31/17
*/

update ct
set LookbackDate = '12/31/2017'
from Credential ct
join Certification c
on ct.CertificationId = c.CertificationId
join Source s
on c.SourceId = s.SourceId
where LookbackDate is null
and s.Code = 'ABIM'

/*SkippedExamLookbackDate:
	Set to last lookback date in oracle, should be 12/31/17
*/

update ct
set SkippedExamLookbackDate = '12/31/2017'
from Credential ct
join Certification c
on ct.CertificationId = c.CertificationId
join Source s
on c.SourceId = s.SourceId
where SkippedExamLookbackDate is null
and s.Code = 'ABIM'