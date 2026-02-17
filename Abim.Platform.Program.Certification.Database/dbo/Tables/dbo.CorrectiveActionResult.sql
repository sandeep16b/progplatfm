
CREATE TABLE [dbo].[CorrectiveActionResult](
	CorrectiveActionResultId [int] IDENTITY(1,1) NOT NULL,
	[CredentialId] [uniqueidentifier] NOT NULL,
	[MemberId] [uniqueidentifier] NOT NULL,
	[EventDate] [datetime2](7) NOT NULL,
	[PBI] [nvarchar](255) NULL,
	[MeetRule] [bit] NOT NULL,
	[ValidationResults] [nvarchar](500) NULL,
	[AdditionalResults] [varchar](max) NULL,
------------------------------------------------------
	IssuanceDate [datetime2](7) NULL,
	MaintenanceStatus [nvarchar](255) NULL,
	CredentialCategory [nvarchar](255) NULL,
------------------------------------------------------	
	[Attestation] [bit] NULL,
	ReattestationDueDate [datetime2](7) NULL,
------------------------------------------------------	
	[ExamRequirement] [bit] NULL,
	[ExamAssessmentMet] [bit] NULL,
	[PassMOCExam] [bit] NULL,
	[PassKCIExam] [bit] NULL,
	[PassCMPExam] [bit] NULL,
	MOCExamTimeRange [nvarchar](255) NULL,
	KCIExamTimeRange [nvarchar](255) NULL,
-------------------------------------------------------
	[FiveYearLookBack] [bit] NULL,
	[FiveYearLookBackRange] [nvarchar](255) NULL,
	[TotalMOCpoints] [numeric](18, 4) NULL,
	[MedicalKnowledgePoints] [numeric](18, 4) NULL,
	[Reciprocity] [bit] NULL,
	[NewSubspecialtyInitialCert] [bit] NULL,
--------------- MaintenanceStatus -------------------
	MaintenanceAnyMOCPoints [numeric](18, 4) NULL,
	MaintenanceTwoYearLookBackRange [nvarchar](255) NULL,
--System Audit Fields
	[Created] [datetime] NOT NULL DEFAULT (CURRENT_TIMESTAMP),
	[CreatedBy] [nvarchar](255) NOT NULL,
	CONSTRAINT [PK_CorrectiveActionResult] PRIMARY KEY CLUSTERED ([CorrectiveActionResultId] ASC)
);

GO
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Log table containing all actions taken against a credential by the corrective action process', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'CorrectiveActionResult'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The CorrectiveActionResult identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'CorrectiveActionResultId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The external identifier (guid) of the diplomate which corrective action was run against',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MemberId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The date corrective action was triggered',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'EventDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates which business rule was met causing corrective action to place the credential in a positive state',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MeetRule'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value representing any validation errors during execution of corrective action rules **not in use',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'ValidationResults'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Additional corrective action results specific to particular corrective action rules',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'AdditionalResults'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The date given to the new issuance created by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'IssuanceDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The partipation status assigned to the new issuance created by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MaintenanceStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value indicating what type of credential this is (grandfather, time limited, must be maintained)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'CredentialCategory'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the attestation requirement was met',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'Attestation'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the doctor reattested',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'ReattestationDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the exam requirement was found to be met by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'ExamRequirement'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the doctor met the exam requirement',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'ExamAssessmentMet'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the doctor passed the 10 year MOC exam',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'PassMOCExam'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the doctor passed the 2 year KCI exam',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'PassKCIExam'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the doctor passed the CMP Exam',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'PassCMPExam'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Start and end dates for the MOC exam due date requirement',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MOCExamTimeRange'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Start and end dates for the KCI exam due date requirement',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'KCIExamTimeRange'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the 5 year lookback requirement was found to be met by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'FiveYearLookBack'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Text value containing the date range for the 5 year lookback window',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'FiveYearLookBackRange'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Total number of MOC points earned during the 5 year lookback window',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'TotalMOCpoints'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Total number of Medical Knowledge points earned during the 5 year lookback window',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MedicalKnowledgePoints'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the reciprocity requirement was found to be met by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'Reciprocity'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the "new subpsecialty initial cert" requirement was found to be met by corrective action',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'NewSubspecialtyInitialCert'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'MOC points earned during the 2 year lookback window',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MaintenanceAnyMOCPoints'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Text value containing the date range for the 2 year lookback window',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CorrectiveActionResult',
    @level2type = N'COLUMN',
    @level2name = N'MaintenanceTwoYearLookBackRange'