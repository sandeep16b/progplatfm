insert into dbo.Certification (BaseCertificationId, SourceId, AddedQualification, Type, Name, Code,  Created, CreatedBy)
select 
	Null as BaseCertificationId,
	s.SourceId as SourceId,
	0 as AddedQualification,
	'OtherBoard' as Type,
	REPLACE(o.description, '*', '') as Name,
	cast(cast(o.code as int) as varchar(8))  as Code,
	getdate() as Created,
	'jbauer' as CreatedBy
from
	[Abim.reporting].pub.OC_CERT_CODE_LV o
join
	dbo.Source s
on
	'American Board of ' + o.BOARD_NAME = s.Name 
	and
	s.Code <> 'ABIM'


	