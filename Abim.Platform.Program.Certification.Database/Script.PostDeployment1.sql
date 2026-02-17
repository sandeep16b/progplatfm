/*
Post-Deployment Script To Fill Lookup Tables
*/

:r ".\dbo\Jobs\CMPIncorrectPathwayNotification.sql"
GO

:r ".\dbo\Jobs\IssuancesCreatedYesterdayNotification.sql"
GO

-- CertificationType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('FocusPractice','Focus Practice',SYSDATETIME(), SYSDATETIME(),'System','System'),
('General','General',SYSDATETIME(), SYSDATETIME(),'System','System'),
('JointAgreement','Joint Agreement',SYSDATETIME(), SYSDATETIME(),'System','System'),
('OtherBoard','Other Board',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Primary','Primary',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Recertification','Recertification',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Subspecialty','Sub-Specialty',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.CertificationType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- MaintenanceStatusType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Maintained','Maintained',SYSDATETIME(), SYSDATETIME(),'System','System'),
('NotMaintained','Not Maintained',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.MaintenanceStatusType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


  -- DurationType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Continuous','Continuous',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Lifetime','Lifetime',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Timelimited','Time limited',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.DurationType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

-- OccurrenceType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Initial','Initial',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Recertification','Recertification',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.OccurrenceType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- MaintenanceRequirementType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('NotRequired','Not Required',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Required','Required',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.MaintenanceRequirementType  as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- PathwayType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('1Year','1 Year',SYSDATETIME(), SYSDATETIME(),'System','System'),
('2Year','2 Year',SYSDATETIME(), SYSDATETIME(),'System','System'),
('10Year','10 Year',SYSDATETIME(), SYSDATETIME(),'System','System'),
('LNG','Longitudinal',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.PathwayType  as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy)
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;


-- IssuanceStatusType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Active','Active',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Expired','Expired',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Inactive','Inactive',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Revoked','Revoked',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Surrendered','Surrendered',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Suspended','Suspended',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Cancelled','Cancelled',SYSDATETIME(), NULL,'System',NULL)
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.IssuanceStatusType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- CredentialType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('General','General',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Subspecialty','Sub-Specialty',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.CredentialType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- CredentialDateType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('AssessmentMetDate','Assessment Met Date',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ExamDueDate','Exam Due Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('DisplayExamDueDate','Display Exam Due Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('KCIExamDueDate','KCI Exam Due Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('MOCExamDueDate','MOC Exam Due Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('ExpirationDate','Expiration Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('GracePeriodStartDate','Grace Period Start Date',SYSDATETIME(), SYSDATETIME(),'System','System'), 
('GracePeriodEndDate','Grace Period End Date',SYSDATETIME(), SYSDATETIME(),'System','System') 
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge  dbo.CredentialDateType as t
using  cte_data as s
on            1=1 and t.Value = s.Value
when matched then
       update set
       Description = s.Description
when not matched by target then
     insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- LookbackReasonType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('TwoYear','Two Year',SYSDATETIME(), SYSDATETIME(),'System','System'),
('FiveYear','Five Year',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Attestation','Attestation',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Assessment','Assessment',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Certification','Certification',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.LookbackReasonType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- LookbackActionType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('FailurePoint','Failure Point',SYSDATETIME(), SYSDATETIME(),'System','System'),
('RestorationPoint','Restoration Point',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.LookbackActionType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


-- LookbackStatusType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('CredentialStatus','Credential Status',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ParticipationStatus','Participation Status',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.LookbackStatusType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

	
-- LookbackDateType
 ;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('TwoYearStart','Two Year Start',SYSDATETIME(), SYSDATETIME(),'System','System'),
('TwoYearEnd','Two Year End',SYSDATETIME(), SYSDATETIME(),'System','System'),
('FiveYearStart','Five Year Start',SYSDATETIME(), SYSDATETIME(),'System','System'),
('FiveYearEnd','Five Year End',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	dbo.LookbackDateType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);


--CAN WE DROP THE CODE COLUMN? OR GET BUSINESS TO ASSIGN CODES?

--Source
 ;with cte_data(Code,SourceGuid,ShortName,Name,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('ABIM', 'A0151381-6E8E-E711-810D-005056AB3791', 'ABIM','American Board of Internal Medicine',SYSDATETIME(), SYSDATETIME(),'System','System'),
('Other', 'A1151381-6E8E-E711-810D-005056AB3791', 'Other','Unknown Board',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABAI', 'A2151381-6E8E-E711-810D-005056AB3791','Allergy & Immunology', 'American Board of Allergy & Immunology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABA', 'A3151381-6E8E-E711-810D-005056AB3791', 'Anesthesiology','American Board of Anesthesiology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABCRS', 'A4151381-6E8E-E711-810D-005056AB3791', 'Colon & Rectal Surgery','American Board of Colon & Rectal Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABDERM', 'A5151381-6E8E-E711-810D-005056AB3791','Dermatology', 'American Board of Dermatology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABEM', 'A6151381-6E8E-E711-810D-005056AB3791', 'Emergency Medicine','American Board of Emergency Medicine',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABFM', 'A7151381-6E8E-E711-810D-005056AB3791','Family Medicine', 'American Board of Family Medicine',SYSDATETIME(), SYSDATETIME(),'System','System'),
('OTHER7', 'A8151381-6E8E-E711-810D-005056AB3791', 'Internal Medicine','American Board of Internal Medicine (Other Board)',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABMGG', 'A9151381-6E8E-E711-810D-005056AB3791', 'Medical Genetics and Genomics','American Board of Medical Genetics and Genomics',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABNS', 'AA151381-6E8E-E711-810D-005056AB3791', 'Neurological Surgery','American Board of Neurological Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABNM', 'AB151381-6E8E-E711-810D-005056AB3791', 'Nuclear Medicine','American Board of Nuclear Medicine',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABOG', 'AC151381-6E8E-E711-810D-005056AB3791', 'Obstetrics & Gynecology','American Board of Obstetrics & Gynecology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABOP', 'AD151381-6E8E-E711-810D-005056AB3791', 'Ophthalmology','American Board of Ophthalmology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABOS', 'AE151381-6E8E-E711-810D-005056AB3791', 'Orthopaedic Surgery','American Board of Orthopaedic Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABOHNS', 'AF151381-6E8E-E711-810D-005056AB3791', 'Otolaryngology - Head and Neck Surgery','American Board of Otolaryngology – Head and Neck Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABPA', 'B0151381-6E8E-E711-810D-005056AB3791', 'Pathology','American Board of Pathology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABP', 'B1151381-6E8E-E711-810D-005056AB3791', 'Pediatrics','American Board of Pediatrics',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABPMR', 'B2151381-6E8E-E711-810D-005056AB3791', 'Physical Medicine & Rehabilitation','American Board of Physical Medicine & Rehabilitation',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABPS', 'B3151381-6E8E-E711-810D-005056AB3791', 'Plastic Surgery','American Board of Plastic Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABPM', 'B4151381-6E8E-E711-810D-005056AB3791', 'Preventive Medicine','American Board of Preventive Medicine',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABPN', 'B5151381-6E8E-E711-810D-005056AB3791', 'Psychiatry & Neurology','American Board of Psychiatry & Neurology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABR', 'B6151381-6E8E-E711-810D-005056AB3791', 'Radiology','American Board of Radiology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABS', 'B7151381-6E8E-E711-810D-005056AB3791', 'Surgery','American Board of Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABTS', 'B8151381-6E8E-E711-810D-005056AB3791', 'Thoracic Surgery','American Board of Thoracic Surgery',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABUR', 'B9151381-6E8E-E711-810D-005056AB3791', 'Urology','American Board of Urology',SYSDATETIME(), SYSDATETIME(),'System','System'),
('CBNC', '53159427-1E78-4924-A72D-CB773E0A833E', 'Nuclear Cardiology','Certification Board oF Nuclear Cardiology',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Code,SourceGuid,ShortName,Name,Created,CreatedBy,Modified,ModifiedBy))
merge	dbo.Source as t
using	cte_data as s
on		 t.Code = s.Code
when matched then
	update set
	SourceGuid = s.SourceGuid,
	Name = s.Name,
	ShortName = s.ShortName
when not matched by target then
	insert(Code,SourceGuid,ShortName,Name,Created,CreatedBy,Modified,ModifiedBy)
	values(s.Code,s.SourceGuid,s.ShortName,s.Name,s.Created,s.CreatedBy,s.Modified,s.ModifiedBy);



-- Certification (ABIM source only)
 ;with cte_data(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, IsCertificateRetired, CertificateRetiredDate, Created, CreatedBy, SourceId)
as (select *, (select SourceId 
			  from dbo.Source 
			  where code = 'ABIM') as SourceId
from (values
('ACHD', '0271AD17-9920-E711-8101-005056AB0196', 'Adult Congenital Heart Disease', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('ADOL', '0171AD17-9920-E711-8101-005056AB0196', 'Adolescent Medicine', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('AHFTC', '0371AD17-9920-E711-8101-005056AB0197', 'Advanced Heart Failure and Transplant Cardiology', 1, 3, 'Subspecialty', 0, NULL,  sysdatetime(), 'System'),
('CARD', '0571AD17-9920-E711-8101-005056AB0198', 'Cardiovascular Disease', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('CCEP', '0671AD17-9920-E711-8101-005056AB0198', 'Clinical Cardiac Electrophysiology', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('CRIT', '0871AD17-9920-E711-8101-005056AB0199', 'Critical Care Medicine', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('ENDO', '1071AD17-9920-E711-8101-005056AB0200', 'Endocrinology, Diabetes and Metabolism', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('GAST', '1271AD17-9920-E711-8101-005056AB0201', 'Gastroenterology', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('GERI', '1371AD17-9920-E711-8101-005056AB0202', 'Geriatric Medicine', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('HEMA', '1471AD17-9920-E711-8101-005056AB0202', 'Hematology', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('HOSP', '1171AD17-9920-E711-8101-005056AB0201', 'Focused Practice in Hospital Medicine', 0, 3, 'FocusPractice', 1, CAST('2023-12-21' AS DATETIME), sysdatetime(), 'System'),
('HPM', '1571AD17-9920-E711-8101-005056AB0203', 'Hospice and Palliative Medicine', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('ICARD', '1871AD17-9920-E711-8101-005056AB0204', 'Interventional Cardiology', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('ID', '1671AD17-9920-E711-8101-005056AB0203', 'Infectious Disease', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('IM', '1771AD17-9920-E711-8101-005056AB0204', 'Internal Medicine', 0, 3, 'Primary', 0, NULL, sysdatetime(), 'System'),
('NEPH', '2071AD17-9920-E711-8101-005056AB0205', 'Nephrology', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('ONCO', '1971AD17-9920-E711-8101-005056AB0205', 'Medical Oncology', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('PTHEP', '2171AD17-9920-E711-8101-005056AB0206', 'Pediatric Transplant Hepatology', 1, 3, 'Subspecialty', 1, CAST('2025-12-01' AS DATETIME), sysdatetime(), 'System'),
('PULM', '2271AD17-9920-E711-8101-005056AB0206', 'Pulmonary Disease', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('RHEUM', '2371AD17-9920-E711-8101-005056AB0207', 'Rheumatology', 0, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('SLEEP', '2471AD17-9920-E711-8101-005056AB0207', 'Sleep Medicine', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('SPORT', '2571AD17-9920-E711-8101-005056AB0208', 'Sports Medicine', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('THEP', '2671AD17-9920-E711-8101-005056AB0208', 'Transplant Hepatology', 1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System'),
('NCC', 'B6AD945A-D9C2-EA11-8136-005056AB4ABF','Neurocritical Care',1, 3, 'Subspecialty', 0, NULL, sysdatetime(), 'System')
)c(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, IsCertificateRetired, CertificateRetiredDate, Created, CreatedBy))
merge	dbo.Certification as t
using	cte_data as s
on		t.Code = s.Code
		and t.SourceId = s.SourceId
when matched then
	update set
	Name = s.Name,
	CertificationGuid = s.CertificationGuid,
	AddedQualification = s.AddedQualification,
	ConsecutiveAttempt = s.ConsecutiveAttempt,
	Type = s.Type,
	IsCertificateRetired = s.IsCertificateRetired,
	CertificateRetiredDate = s.CertificateRetiredDate
when not matched by target then
	insert(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, IsCertificateRetired, CertificateRetiredDate, Created, CreatedBy, SourceId)
	values(s.Code, s.CertificationGuid, s.Name, s.AddedQualification, s.ConsecutiveAttempt, s.Type, s.IsCertificateRetired, s.CertificateRetiredDate, s.Created, s.CreatedBy, s.SourceId);

-- ActionType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
(1,'Add',SYSDATETIME(), SYSDATETIME(),'System','System'),
(2,'Update',SYSDATETIME(), SYSDATETIME(),'System','System'),
(3,'Cancel',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	Cosponsored.ActionType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

-- CertMOCType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
(1,'Cert',SYSDATETIME(), SYSDATETIME(),'System','System'),
(2,'MOC',SYSDATETIME(), SYSDATETIME(),'System','System'),
(3,'KCI',SYSDATETIME(), SYSDATETIME(),'System','System'),
 (4,'LKA Only',SYSDATETIME(),SYSDATETIME(),'System','System'),
(6,'Only MOC',SYSDATETIME(),SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	Cosponsored.CertMOCType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

-- FileProcessStatusType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('NewFile','New file to be processed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('SuccessfullyDownloaded','Successfully downloaded the file',SYSDATETIME(), SYSDATETIME(),'System','System'),
('NotDownloaded','File not downloaded',SYSDATETIME(), SYSDATETIME(),'System','System'),
('SuccessfullyParsed','Successfully parsed the file',SYSDATETIME(), SYSDATETIME(),'System','System'),
('NotParsed','File not parsed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('SuccessfullyCompleted','Successfully completed process',SYSDATETIME(), SYSDATETIME(),'System','System'),
('CompletedwithErrors','Completed process with errors',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	Cosponsored.FileProcessStatusType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

-- FileDetailProcessStatusType
;with cte_data(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
as (select * from (values
('Initiated','Process Initiated',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ValidationFailed','Validation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ValidationSuccess','Validation successfully completed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ProfileCreationFailed','Create new profile API failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABIMInitialCertCredentialCreationFailed','ABIM initial cert credentail creation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABIMInitialCertIssuanceCreationFailed','ABIM initial cert credentail creation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('BoardPrimaryCertCredentialCreationFailed','Cosponsored Board primary cert credential creation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('BoardPrimaryCertIssuanceCreationFailed','Cosponsored Board primary cert credential creation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('ABIMFPHMCredentialCreationFailed','ABIM FPHM credentail creation failed',SYSDATETIME(), SYSDATETIME(),'System','System'),
('SendWelcomeEmailFailed','Send Welcome Email Failed',SYSDATETIME(),SYSDATETIME(),'System','System'),
('SendSecurityCodeEmailFailed','Send Security Code Email Failed',SYSDATETIME(),SYSDATETIME(),'System','System'),
('Success','Successfully completed process',SYSDATETIME(), SYSDATETIME(),'System','System'),
('DuplicateProfile'	,'Duplicate Profile found',SYSDATETIME(), SYSDATETIME(),'System','System'),
('UserRestrictionInsertFailed'	,'User Restriction Insertion Failed for CoSponsered Cancellation',SYSDATETIME(), SYSDATETIME(),'System','System')
)c(Value,Description,Created,Modified,CreatedBy,ModifiedBy))
merge	Cosponsored.FileDetailProcessStatusType as t
using	cte_data as s
on		1=1 and t.Value = s.Value
when matched then
	update set
	Description = s.Description
when not matched by target then
	insert(Value,Description,Created,Modified,CreatedBy,ModifiedBy)
	values(s.Value,s.Description,s.Created,s.Modified,s.CreatedBy,s.ModifiedBy);

-- Board
;with cte_data(BoardCode,BoardName,CreatedBy) 
as (select [Code] as BoardCode,[Name] as BoardName,SYSTEM_USER
	from dbo.Source 
	where Code in ('ABPMR','ABS','ABR','ABOHNS','ABA','ABPN','ABEM','ABP','ABFM','ABOG','ABPM')
   )
merge	Cosponsored.Board as t
using	cte_data as s
on		1=1 and t.Code = s.BoardCode
when matched then
	update set
	[Name] = s.BoardName,
	Modified = SYSDATETIME(),
	ModifiedBy = SYSTEM_USER
when not matched by target then
	insert(Code,[Name],CreatedBy)
	values(s.BoardCode,s.BoardName,s.CreatedBy);

--Update BaseCertificationId from those that have added qualification and underlying exam
Declare @CertificationId int

select @CertificationId = CertificationId
from dbo.Certification
where Code = 'CARD'

update Certification
set BaseCertificationId = @CertificationId
where Code in ('ACHD','AHFTC','CCEP','ICARD')

select @CertificationId = CertificationId
from dbo.Certification
where Code = 'GAST'

update Certification
set BaseCertificationId = @CertificationId
where Code  = 'THEP'

select @CertificationId = CertificationId
from dbo.Certification
where Code = 'CRIT'

update Certification
set BaseCertificationId = @CertificationId
where Code  = 'NCC'


-- Certification (Other source only)
 ;with cte_data(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, Created, CreatedBy, SourceId)
as (select *, (select SourceId 
			  from dbo.Source 
			  where code = 'Other') as SourceId
from (values
('ALLG', '0471AD17-9920-E711-8101-005056AB0197', 'Allergy and Immunology', 0, 3, 'JointAgreement', sysdatetime(), 'System'),
('CLI', '0771AD17-9920-E711-8101-005056AB0199', 'Clinical and Laboratory Immunology', 1, 3, 'Subspecialty', sysdatetime(), 'System'),
('DLI', '0971AD17-9920-E711-8101-005056AB0200', 'Diagnostic and Laboratory Immunology', 1, 3, 'Subspecialty', sysdatetime(), 'System')
)c(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, Created, CreatedBy))
merge	dbo.Certification as t
using	cte_data as s
on		t.Code = s.Code
		and t.SourceId = s.SourceId
when matched then
	update set
	Name = s.Name,
	CertificationGuid = s.CertificationGuid,
	AddedQualification = s.AddedQualification,
	ConsecutiveAttempt = s.ConsecutiveAttempt,
	Type = s.Type
when not matched by target then
	insert(Code, CertificationGuid, Name, AddedQualification, ConsecutiveAttempt, Type, Created, CreatedBy, SourceId)
	values(s.Code, s.CertificationGuid, s.Name, s.AddedQualification, s.ConsecutiveAttempt, s.Type, s.Created, s.CreatedBy, s.SourceId);

--Update BaseCertificationId
update Certification
set BaseCertificationId = 
	(select CertificationId
	from Certification
	where Name = 'Cardiovascular Disease'
	and SourceId = (Select SourceId
					from Source
					where Code = 'ABIM'))
where 
	Name in ('Adult Congenital Heart Disease'
			,'Advanced Heart Failure and Transplant Cardiology'
			,'Clinical Cardiac Electrophysiology'
			,'Interventional Cardiology')
	and 
	SourceId = (Select SourceId
				from Source
				where Code = 'ABIM')

update Certification
set BaseCertificationId = 
	(select CertificationId
	from Certification
	where Name = 'Gastroenterology'
	and SourceId = (Select SourceId
					from Source
					where Code = 'ABIM'))
where 
	Name in ('Transplant Hepatology')
	and 
	SourceId = (Select SourceId
				from Source
				where Code = 'ABIM')


--The code blocks below rely on DbPrefix variable from Octopus
Declare @sql nvarchar(1000),
@objectid int,
@dbprefix varchar(10),
@env varchar(10)

Set @dbprefix = replace('$(DbPrefix)', '_blank_', '')
Set @env =  replace('$(Environment)', '_blank_', '')

--Make sure guids are in sync between Certification and Registration
set @sql = 'update e
			set CertificationGuid = c.CertificationGuid
			from [' + @dbprefix + 'Registration].[dbo].[Exam] e
			join [' + @dbprefix + 'Certification].dbo.[Certification] c
			on e.Name = c.Name
			and c.SourceId = (select SourceId from [' + @dbprefix + 'Certification].dbo.Source where Name = ''ABIM'')'

Execute sp_executesql @sql


--CREATE IDENTITY VIEW IN EACH PLATFORM DATABASE (Other than Identity)
--This was added to help out with data migration for specific tenants
select @objectid = object_id('dbo.UserProfile', 'V')

if @objectid is not null
begin
	Set @sql = 'drop view dbo.UserProfile'
	Execute sp_executesql @sql
end

set @sql = 'create view dbo.UserProfile as 
select 
	p.value as [AbimId]
	, u.UserAccountsGuid as [MemberID]
	, u.UserAccountsId as [ProfileKey]
	, u.email as [EmailAddress]
	, u.LastName 
	, u.FirstName 
	, u.MiddleName
	, u.Salutation
	, u.Suffix
	, u.MaidenName
	, u.Gender
	, u.BirthDate
	, u.Created as [CreatedDate]
from 
	' +  @dbprefix + 'IdentityData'  + '.dbo.UserAccounts u with (nolock)
join 
	' +  @dbprefix + 'IdentityData'  + '.dbo.ProfileProperty p
on 
	p.UserAccountsId = u.UserAccountsId
and 
	p.Type = ''http://schemas.abim.org/2016/identifier/abim'''

Execute sp_executesql @sql


--DATA MIGRATION VIEWS

--Source
select @objectid = object_id('map.OtherCertification', 'V')

if @objectid is not null
begin
	Set @sql = 'drop view map.OtherCertification'
	Execute sp_executesql @sql
end

set @sql = 'create view map.OtherCertification as 
select 
	Null as BaseCertificationId,
	s.SourceId as SourceId,
	0 as AddedQualification,
	''OtherBoard'' as Type,
	REPLACE(o.description, ''*'', '''') as Name,
	cast(cast(o.code as int) as varchar(8))  as Code,
	getdate() as Created,
	''jbauer'' as CreatedBy
from
	[Abim.reporting].pub.OC_CERT_CODE_LV o
join
	dbo.Source s
on
	o.BOARD_NAME = s.Name '

Execute sp_executesql @sql

-- Credential_Issuance View 

select @objectid = object_id('dbo.Credential_Issuance', 'V')

if @objectid is not null
begin
	Set @sql = 'drop view dbo.Credential_Issuance'
	Execute sp_executesql @sql
end

set @sql = 'create view dbo.Credential_Issuance as 
select 
	credIssue.ExpirationDate
    ,cred.MemberId
	,cred.CertificationId
	,cert.Code CertCode
	,credIssue.Duration  
    ,credIssue.Occurrence
	,credIssue.IssuanceDate
	,credIssue.IssuanceStatus 
	,credIssue.UnderReview
	,credIssue.EffectiveDate
	,src.Code SourceCode
	,cert.Name CertName
from 
	' + @dbprefix + 'Certification.dbo.Credential cred with (nolock)
join 
	' + @dbprefix + 'Certification.dbo.Issuance credIssue 

	ON cred.credentialid = credIssue.credentialid 	

join 
	' + @dbprefix + 'Certification.dbo.Certification cert
	ON cred.CertificationId = cert.CertificationId 	
join 
	' + @dbprefix + 'Certification.dbo.Source src
	ON cert.SourceId = src.SourceId
'

Execute sp_executesql @sql

--Create Certification synonyms
if  object_id('dbo.Exam','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[Exam] FOR [' + @dbprefix + 'Registration].[dbo].[Exam]'
	Execute sp_executesql @sql
end

if  object_id('dbo.Administration','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[Administration] FOR [' + @dbprefix + 'Registration].[dbo].[Administration]'
	Execute sp_executesql @sql
end

if  object_id('dbo.Registration','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[Registration] FOR [' + @dbprefix + 'Registration].[dbo].[Registration]'
	Execute sp_executesql @sql
end


if  object_id('dbo.SeatRegistration','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[SeatRegistration] FOR [' + @dbprefix + 'Registration].[dbo].[SeatRegistration]'
	Execute sp_executesql @sql
END
--229806
if  object_id('dbo.Attestation','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[Attestation] FOR [' + @dbprefix + 'Attestation].[dbo].[Attestation]'
	Execute sp_executesql @sql
END

if  object_id('dbo.AttestationInstance','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [dbo].[AttestationInstance] FOR [' + @dbprefix + 'Attestation].[dbo].[AttestationInstance]'
	Execute sp_executesql @sql
END
---------
if  object_id('IdentityData.UserAccounts','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[UserAccounts] FOR [' + @dbprefix + 'IdentityData].[dbo].[UserAccounts]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.ProfileProperty','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[ProfileProperty] FOR [' + @dbprefix + 'IdentityData].[dbo].[ProfileProperty]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.Addresses','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[Addresses] FOR [' + @dbprefix + 'IdentityData].[dbo].[Addresses]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.Region','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[Region] FOR [' + @dbprefix + 'IdentityData].[dbo].[Region]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.Country','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[Country] FOR [' + @dbprefix + 'IdentityData].[dbo].[Country]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.RestrictionFeature','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[RestrictionFeature] FOR [' + @dbprefix + 'IdentityData].[dbo].[RestrictionFeature]'
	Execute sp_executesql @sql
END

if  object_id('IdentityData.UserAccountsRestriction','SN') is null
begin
	Set @sql = 'CREATE SYNONYM [IdentityData].[UserAccountsRestriction] FOR [' + @dbprefix + 'IdentityData].[dbo].[UserAccountsRestriction]'
	Execute sp_executesql @sql
END
