CREATE TABLE [dbo].[Credential] (
	--Primary Key
    [CredentialId]			INT IDENTITY(1,1)		NOT NULL,
	--Natural Key
	[MemberId]				UNIQUEIDENTIFIER		NOT NULL,
    [CertificationId]		INT						NOT NULL, 
	--Non-clustered key(s)
	[CredentialGuid]		UNIQUEIDENTIFIER		NOT NULL DEFAULT newsequentialid(),
	--All other fields (sorted alphabetically)
	[AssessmentMet]			BIT						NOT NULL DEFAULT 0,
	[AssessmentMetDate]		DATETIME2				NULL,
	[ConsecutiveKCIPassRequired] BIT				NOT NULL DEFAULT 0,
	[ExamDueDate]			DATETIME2				NULL,
	[ExamFailCount]			INT						NOT NULL DEFAULT 0,
	[DisplayExamDueDate]	DATETIME2				NULL, 
	[ForcedPathway]			BIT						NOT NULL DEFAULT 0,
	[GracePeriodEndDate]	DATETIME2				NULL,
	[GracePeriodStartDate]	DATETIME2				NULL,
	[GrandfatherMOCPrintDate] DATETIME2				NULL,
    [IsActive]				BIT						NOT NULL,
	[KCIExamDueDate]		DATETIME2				NULL, 
	[LookbackDate]			DATETIME2				NULL, 
	[MOCExamDueDate]		DATETIME2				NULL, 
	[Pathway]				NVARCHAR (50)			NOT NULL DEFAULT '10Year',
	[ReAttestationDueDate]	DATETIME2				NULL,
	[SelectedToMaintain]	BIT						NOT NULL DEFAULT 1,
	[SkippedExamLookbackDate] DATETIME2				NULL,
    [Type]					NVARCHAR (50)			NOT NULL,
	[WithdrawnDate]			DATETIME2				NULL,
	[IsInCMP]				BIT						NOT NULL DEFAULT 0,
	[CMPEnrollmentDate]		DATETIME2				NULL,
	[CMPUnenrollmentDate]	DATETIME2				NULL,
	[IsCosponsored]			BIT						NOT NULL DEFAULT 0,
	[OnBehalfBoardCode]		NVARCHAR(50)			NULL,
	[OnBehalfBoardName]		NVARCHAR(200)			NULL,
	--System Audit Fields
	[Created]				DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,
    CONSTRAINT [PK_Credential]			      PRIMARY KEY CLUSTERED   ([CredentialId] ASC),
    CONSTRAINT [FK_Credential_Certification]  FOREIGN KEY             ([CertificationId]) REFERENCES [dbo].[Certification] ([CertificationId]),
    CONSTRAINT [FK_Credential_PathwayType]    FOREIGN KEY             ([Pathway])		  REFERENCES [dbo].[PathwayType] ([Value]),
	CONSTRAINT [FK_Credential_CredentialType] FOREIGN KEY             ([Type])		      REFERENCES [dbo].[CredentialType] ([Value]), 
    CONSTRAINT [NK_Credential] UNIQUE ([MemberId],[CertificationId]),
    
    --CONSTRAINT [UK_Credential_MemberId] UNIQUE ([MemberId])
	
);
GO

CREATE UNIQUE NONCLUSTERED INDEX [NCK_Credential] ON [dbo].[Credential] ([CredentialGuid])
GO
CREATE NONCLUSTERED INDEX [IK_Credential_MemberId] ON [dbo].[Credential] ([MemberId])
GO
CREATE NONCLUSTERED INDEX [IK_Credential_Certification] ON [dbo].[Credential] ([CertificationId]) INCLUDE ([CredentialId], [MemberId])
GO
CREATE NONCLUSTERED INDEX [IK_Credential_PathwayType] ON [dbo].[Credential] ([Pathway])
GO
CREATE NONCLUSTERED INDEX [IK_Credential_CredentialType] ON [dbo].[Credential] ([Type])
GO
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Distinct list of areas a diplomate is certified in', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'Credential'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'CredentialId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Member (diplomate) who earned this Credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'MemberId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique Identifier Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'CredentialGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity value of the Certification record that corresponds to this Credential (what area the diplomate certified in)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'CertificationId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Next date the diplomate is required to take and pass the recertification exam',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'ExamDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Counter indicating how many times in a row the diplomate has failed the exam',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'ExamFailCount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'If a diplomate has issues on the 2Year pathway, they may be forced to the 10Year.',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'ForcedPathway'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'End date of the diplomate''s grace period for this credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'GracePeriodEndDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Start date of the diplomate''s grace period for this credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'GracePeriodStartDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the certificate of appreciation was printed for a grandfather who voluntarily recertified',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'GrandfatherMOCPrintDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if this Credential is active (if so it will have an underlying Active issuance)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'IsActive'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date of the next lookback for this Credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'LookbackDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Current pathway the diplomate is on for this Credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'Pathway'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Due date for the diplomate to submit their reattestation',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'ReAttestationDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if the doctor has met the exam requirement for the current cycle of this credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'AssessmentMet'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the doctor passed the exam for the current cycle of this credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'AssessmentMetDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Due date displayed to doctors in the portal (can differ from the actual exam due date depending on pathway and number of consecutive failures)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'ConsecutiveKCIPassRequired'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Due date displayed to doctors in the portal (can differ from the actual exam due date depending on pathway and number of consecutive failures)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'DisplayExamDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Exam due date for credentials on the 2 year pathway (not visible to doctors or used in any rules, just stored for tracking purposes)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'KCIExamDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Exam due date for credentials on the 10 year pathway (not visible to doctors or used in any rules, just stored for tracking purposes)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'MOCExamDueDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The doctor has the ability to toggle the credential between "selected" and "not selected". This also indicates which credential of an IM/FPHM pair is the selected one.',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'SelectedToMaintain'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the credential is a subspecialty or not',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'Type'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The business has the ability to set a doctor''s credential (issuance) to Suspended, Surrendered or Revoked. This date indicates when that will take effect',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'WithdrawnDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if this Credential is Cosponsored',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'IsCosponsored'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Code for the Cosponsored Board',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'OnBehalfBoardCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name for the Cosponsored Board',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Credential',
    @level2type = N'COLUMN',
    @level2name = N'OnBehalfBoardName'
GO