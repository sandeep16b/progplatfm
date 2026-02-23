CREATE TABLE [Cosponsored].[ImportFileDetail] (
	--Primary Key
    [ImportFileDetailId]       INT IDENTITY(1,1)		NOT NULL,
    --Foreign Key 
	[ImportFileId]				INT						NOT NULL,
	--Fields not found in data file
	[ProfileExists]				BIT						NOT NULL DEFAULT(0),
	[AbimId]					INT						NULL,
	[MemberId]					UNIQUEIDENTIFIER		NULL,
    [SSN]						NVARCHAR(255)			NULL,
	[ErrorDescription]			NVARCHAR(500)			NULL,
	--All other fields (Sorted Alpabetically)
	[Action]					INT						NULL,
	[AbmsId]					NVARCHAR(50)			NOT NULL DEFAULT(''),
	[AdaStatus]					NVARCHAR(50)			NOT NULL DEFAULT('2'),
	[BirthCountryCode]			NVARCHAR(128)			NULL,
	[BirthCountryName]			NVARCHAR(255)			NULL,
	[CertMOC]					INT						NOT NULL,
	[City]						NVARCHAR(255)			NOT NULL DEFAULT(''),
	[ClientID]					NVARCHAR(255)			NOT NULL DEFAULT(''),
	[CountryCode]				NVARCHAR(50)			NULL,
	[CountryName]				NVARCHAR(255)			NOT NULL DEFAULT(''),
	[DateLastUpdate]			DATETIME				NULL,
	[DateOfBirth]				DATETIME				NOT NULL,
	[District]					NVARCHAR(255)			NULL,
	[Email]						NVARCHAR(255)			NOT NULL DEFAULT(''),
	[ExamDueDate]				DATETIME				NOT NULL,
	[ExamName]					NVARCHAR(255)			NOT NULL DEFAULT(''),
	[FamilyName]				NVARCHAR(255)			NOT NULL DEFAULT(''),
	[FaxNumber]					NVARCHAR(255)			NULL,
	[FirstName]					NVARCHAR(255)			NOT NULL DEFAULT(''),
	[FormattedName]				NVARCHAR(255)			NULL,
	[Gender]					NVARCHAR(50)			NOT NULL DEFAULT(''),
	[GenerationIdentifier]		NVARCHAR(50)			NULL,
	[MiddleName]				NVARCHAR(255)			NULL,
	[Organization]				NVARCHAR(255)			NULL,
	[Phone]						NVARCHAR(255)			NOT NULL DEFAULT(''),
	[PostalCode]				NVARCHAR(50)			NOT NULL DEFAULT(''),
	[ProcessStatus]				VARCHAR(50)				NOT NULL DEFAULT('Initiated'),
	[Region]					NVARCHAR(255)			NULL,
	[StateOrProvince]			NVARCHAR(255)			NOT NULL DEFAULT(''),
	[StreetAddress1]			NVARCHAR(255)			NOT NULL DEFAULT(''),
	[StreetAddress2]			NVARCHAR(255)			NULL,
	[StreetAddress3]			NVARCHAR(255)			NULL,
	[StreetAddress4]			NVARCHAR(255)			NULL,
	[Title]						NVARCHAR(255)			NULL,
	[TypeOfMedicalSchool]		NVARCHAR(50)			NOT NULL,
	[WelcomeEmailSent]			BIT						NOT NULL DEFAULT(0),
	[SecurityCodeEmailSent]		BIT						NOT NULL DEFAULT(0),
	[RealSSNExists]				INT						NULL,
	[PartialTaxId]				VARCHAR(5)				NULL,
    --System Audit Fields
	[Created]					DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]					NVARCHAR (255)			NOT NULL,
	[Modified]					DATETIME				NULL,
	[ModifiedBy]				NVARCHAR (255)			NULL,
    CONSTRAINT [PK_ImportFileDetail]								PRIMARY KEY CLUSTERED	([ImportFileDetailId] ASC),
    CONSTRAINT [FK_ImportFileDetail_ImportFile]						FOREIGN KEY				([ImportFileId])			REFERENCES [Cosponsored].[ImportFile]	([ImportFileId]),
	CONSTRAINT [FK_ImportFileDetail_ActionType]						FOREIGN KEY				([Action])					REFERENCES [Cosponsored].[ActionType]	([Value]),
	CONSTRAINT [FK_ImportFileDetail_CertMOCType]					FOREIGN KEY				([CertMOC])					REFERENCES [Cosponsored].[CertMOCType]	([Value]),
	CONSTRAINT [FK_ImportFileDetail_FileDetailProcessStatusType]	FOREIGN KEY				([ProcessStatus])			REFERENCES [Cosponsored].[FileDetailProcessStatusType]	([Value])
);
GO

  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Data processed from files recieved from Cosponsored Boards', 
  @level0type = N'Schema', 
  @level0name = 'Cosponsored', 
  @level1type = N'Table', 
  @level1name = 'ImportFileDetail'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ImportFileDetailId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity value for the imported file that was recieved from the Cosponsored Boards',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ImportFileId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the Abim Profile exists',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ProfileExists'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique ID for Abim Profile',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'AbimId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique Identifier for Abim Profile',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'MemberId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Dummy SSN for each Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'SSN'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Errors returned as part of the data load for the specific record',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ErrorDescription'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Action performed during data load (1=Add, 2=Update, 3=Cancel)',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Action'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'ABMS ID for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'AbmsId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Ada accommodation status of the Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'AdaStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Birth Country Code for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'BirthCountryCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Birth Country Name for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'BirthCountryName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates the Exam Type',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'CertMOC'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'City for Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'City'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Board Number',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ClientId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Country Code of Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'CountryCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Country Name of Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'CountryName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date Last Update for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'DateLastUpdate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date of Birth for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'DateOfBirth'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'District of Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'District'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Email Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Email'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Exam Due Date for Area indicated in Registration node',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ExamDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Exam Name for Registration',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'ExamName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Last Name of Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'FamilyName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Fax Number',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'FaxNumber'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'First Name of Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'FirstName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Formatted Full Name of Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'FormattedName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Gender of Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Gender'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Suffix of Candidate Name',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'GenerationIdentifier'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Middle Name of Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'MiddleName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Organization Name for Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Organization'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Phone Number',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Phone'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Postal Code for Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'PostalCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Region for Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Region'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'State for Address',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'StateOrProvince'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Address Line 1',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'StreetAddress1'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Address Line 2',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'StreetAddress2'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Address Line 3',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'StreetAddress3'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Address Line 4',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'StreetAddress4'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Title',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'Title'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Type of Medical School for Candidate',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'TypeOfMedicalSchool'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the Welcome Email is sent to physician',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'WelcomeEmailSent'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the Security Code Email is sent to physician',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFileDetail',
    @level2type = N'COLUMN',
    @level2name = N'SecurityCodeEmailSent'
GO