
CREATE PROCEDURE dbo.usp_get_Certificateprint_List
(
	@i_CertGuidList VARCHAR(8000), 
	@i_AbimId VARCHAR(50),
	@i_InitialCert bit, 
	@i_ReCertification BIT,
	@i_startDate DATETIME, 
	@i_endDate DATETIME
)
AS
BEGIN

--DECLARE @i_CertGuidList VARCHAR(8000), @i_InitialCert bit, @i_ReCertification BIT,
--	@i_startDate DATETIME, @i_endDate DATETIME

--SET 	@i_CertGuidList = '2071AD17-9920-E711-8101-005056AB0205,1971AD17-9920-E711-8101-005056AB0205'
--SET @i_AbimId = NULL
--SET @i_InitialCert = 1
--SET @i_ReCertification = 1
--SET @i_startDate = '2019-12-01'
--SET @i_endDate = '2020-12-31'

DECLARE @ErrorCode INT;

DECLARE @CertType VARCHAR(50);

IF EXISTS (SELECT 1 FROM dbo.Certification c WITH (NOLOCK) 
			INNER JOIN dbo.Source s WITH (NOLOCK) ON s.SourceId = c.SourceId
			INNER JOIN dbo.Split(@i_CertGuidList,',') AS list ON c.CertificationGuid = list.[data] 
			WHERE c.Name = 'Focused Practice in Hospital Medicine'
			AND s.Code = 'ABIM')
BEGIN
		SET @i_ReCertification = 1;
END

SELECT @CertType = CASE 
						WHEN @i_InitialCert=1 AND @i_ReCertification=1 THEN 'Initial,Recertification'
						WHEN @i_InitialCert=1 AND @i_ReCertification=0 THEN 'Initial'
						WHEN @i_InitialCert=0 AND @i_ReCertification=1 THEN 'Recertification'
						ELSE '' 
					END

;with diplomate as
(
SELECT DISTINCT
	up.AbimId
	,up.MemberID
	,ua.LastName AS LastName
	,ua.FirstName AS FirstName
	,ua.MiddleName AS MiddleName
	,ua.Suffix AS NameExtent
	,ua.Email
	,ua.Username
	,up.ProfileKey
FROM IdentityData.UserProfile up WITH (NOLOCK)
INNER JOIN IdentityData.UserAccounts ua WITH (NOLOCK) ON ua.[Key] = up.ProfileKey
WHERE
-- Standard Exclusions (X(Blocked), D(Deceased), R(Retired) and Inactive)
	ua.IsActive = 1 -- Exclude Inactive
AND ua.IsRetired = 0 -- Exlude R(Retired)
AND ua.IsDeceased = 0 -- Exclude D(Deceased)
AND ua.Email IS NOT NULL -- Has Email Address
AND (up.AbimId = @i_AbimId OR @i_AbimId IS NULL)
-- Exlcude diplomates with X(Blocked) status
AND NOT EXISTS (
				SELECT 1
				FROM IdentityData.UserAccountsRestriction uar WITH (NOLOCK)
				INNER JOIN IdentityData.RestrictionFeature rf WITH (NOLOCK) ON rf.RestrictionFeatureId = uar.RestrictionFeatureId
				WHERE uar.UserAccountsId = ua.[Key]
				AND rf.FeatureName = 'Blocked'
			   )
-- Exclude Revoked, Suspended and Surrendered Certs
AND NOT EXISTS (
				SELECT 1
				FROM dbo.[Credential] cr1 WITH (NOLOCK)
				INNER JOIN dbo.Issuance i1 WITH (NOLOCK) ON i1.CredentialId = cr1.CredentialId
				WHERE cr1.MemberId = up.MemberID
				AND i1.IssuanceStatus IN ('Revoked','Suspended','Surrendered')
			   )
-- Exclude ABIM Internal 'Test IDs'
AND NOT EXISTS 
		(
		SELECT 1 
		FROM IdentityData.UserClaims uc2 WITH (NOLOCK) 
		WHERE 
			uc2.ParentKey = ua.[Key]
			AND uc2.[Type] = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
			AND uc2.Value = 'Demo-Account'
		)
AND NOT EXISTS 
		(
		SELECT 1
		FROM IdentityData.UserAccounts ua1 WITH (NOLOCK)
		WHERE ua1.[ID] = ua.[ID]
		AND ((ua1.FirstName like 'Test%') OR (ua1.FirstName like 'TDD') OR (ua1.LastName like 'Test%') ) --OR (ua1.Email like '%abim.org')
		)
)
,addr as
(
select 
	adr.StreetAddress1,
	adr.StreetAddress2,
	adr.StreetAddress3,
	adr.StreetAddress4,
	adr.City,
	--CASE WHEN ISNULL(con.Code,'US') = 'US' THEN rgn.Code ELSE 'ZZ' END as [State],
	adr.PostalCode,
	con.Code as CountryCode,
	rgn.Code as [State],
	CASE WHEN ISNULL(con.Code,'US') = 'US' THEN '' ELSE con.[Name] END AS CountryName,
	adr.IsPrimary,
	adr.Profile_Key
from IdentityData.Addresses adr with (nolock)
left outer join IdentityData.Country con with (nolock)
on con.Code = adr.CountryId
left outer join IdentityData.Region rgn with (nolock)
on rgn.RegionId = adr.RegionId
and rgn.CountryId = con.Code
WHERE adr.IsPrimary = 1
)
-- Criteria specific to Adolescent Medicine Intial Certification Exam
,Certification as
(
SELECT DISTINCT	
	ct1.Name AS ExamName,
	ct1.CertificationGuid,
	IIF(i2.Occurrence='Initial','Certification','Recertification') AS CATEGORY,
	i2.Occurrence AS ExamType,
	d.AbimId,
	d.MemberID,
	CONCAT(d.FirstName,' ',ISNULL(d.MiddleName,''),' ', d.LastName,' ',ISNULL(d.NameExtent,'')) AS FullName,
	d.Email,
	i2.IssuanceDate AS CertificateDate,
	CONCAT('S',CONVERT(Varchar(2),IIF(MONTH(i2.IssuanceDate) >= 7,(YEAR(i2.IssuanceDate) % 100),(YEAR(i2.IssuanceDate) % 100)-1)),'Y', CONVERT(VARCHAR(2),(YEAR(i2.IssuanceDate) % 100))) AS Signaturedate,
	IIF(i2.MaintenanceStatus='Maintained',1,0) AS Maintain,
	1 AS OriginalCert,
	0 AS Duplicates,
	addr.StreetAddress1 AS ADDR1,
	addr.StreetAddress2 AS ADDR2,
	addr.StreetAddress3 AS ADDR3,
	addr.City,
	addr.State,
	addr.PostalCode,
	addr.CountryName,
	addr.CountryCode,
	'513711565054' AS GLCode,
	'CER-03'  IOPCode,
	CASE  
	 WHEN ct1.Name = 'Internal Medicine' AND ct1.Type='Primary' THEN 1 
	 WHEN YEAR(i2.EffectiveDate) >=2014 AND ISNULL(i2.MaintenanceStatus,'') <> 'Maintained' THEN 2
	 ELSE 3 END AS LetterVersion,

	 CASE WHEN cr2.Type = 'Subspecialty' AND i2.IssuanceDate >= '01-Jul-2014' THEN IIF(i2.MaintenanceStatus='Maintained','L','N') 
	 ELSE IIF(i2.MaintenanceStatus='Maintained','Y','N') END AS NO_EndDate
FROM dbo.[Credential] cr2 WITH (NOLOCK)
INNER JOIN dbo.Issuance i2 WITH (NOLOCK) ON i2.CredentialId = cr2.CredentialId
INNER JOIN dbo.Certification ct1 WITH (NOLOCK) ON ct1.CertificationId = cr2.CertificationId
INNER JOIN dbo.Source s WITH (NOLOCK) ON s.SourceId = ct1.SourceId
INNER JOIN dbo.Split(@i_CertGuidList,',') AS list ON ct1.CertificationGuid = list.[data]
INNER JOIN diplomate d ON d.MemberID = cr2.MemberId
INNER JOIN addr ON addr.Profile_Key = d.ProfileKey
WHERE i2.IssuanceDate BETWEEN @i_startDate AND @i_endDate 
AND s.Code = 'ABIM'   
AND 
(i2.MaintenanceStatus !='Maintained'
	OR 
	NOT EXISTS (SELECT 1
					FROM dbo.[Credential] cr3 WITH (NOLOCK)
					INNER JOIN dbo.Issuance i3 WITH (NOLOCK) ON i3.CredentialId = cr3.CredentialId
					WHERE cr2.MemberId=cr3.MemberId
					AND cr2.CertificationId = cr3.CertificationId
					AND i2.Occurrence = i3.Occurrence
					AND i3.IssuanceDate < i2.IssuanceDate
					AND ((i3.MaintenanceStatus ='Maintained') OR (i3.MaintenanceStatus=i2.MaintenanceStatus))
					)
	)
)

SELECT DISTINCT
 	c.FullName,
	c.AbimId AS CandidateId,
	c.MemberID,
	c.ExamName AS Specialty,
	IIF(c.ExamName='Focused Practice in Hospital Medicine','F',IIF(c.ExamType='Initial','C','P')) AS ExamType,
	c.CertificationGuid,
	'' AS ExamCode,
	IIF(c.ExamName='Focused Practice in Hospital Medicine','Focus Practice',c.CATEGORY) AS CATEGORY,
	c.OriginalCert AS Original,
	c.Duplicates,
	c.Maintain,
	c.ADDR1,
	c.ADDR2,
	c.ADDR3,
	c.City,
	c.State,
	c.PostalCode AS Zip,
	c.CountryName AS Country,
	c.CertificateDate,
	c.Signaturedate,
	c.NO_EndDate,
	c.LetterVersion,
	c.Email,
	c.CountryCode,
	c.GLCode,
	c.IOPCode
FROM Certification c
INNER JOIN dbo.Split(@CertType,',') AS CertList ON c.ExamType = CertList.[data]

END