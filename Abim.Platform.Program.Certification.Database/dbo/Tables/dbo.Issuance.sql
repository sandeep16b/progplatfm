
CREATE TABLE [dbo].[Issuance] (
	--Primary Key
	[IssuanceId]                     INT IDENTITY(1,1)           NOT NULL,
	[IssuanceGuid]                   UNIQUEIDENTIFIER    DEFAULT(NEWSEQUENTIALID()) NOT NULL,
	--Natural Key
    [CredentialId]					 INT                         NOT NULL, 
	[IssuanceDate]					 DATETIME                    NOT NULL,
	[EffectiveDate]					 DATETIME                    NOT NULL,
	--All other fields (sorted alphabetically)
    [DeselectionEffectiveDate]       DATETIME                    NULL, 
    [DeSelectionProcessedDate]        DATETIME                    NULL, 
    [DeselectionSubmittedDate]       DATETIME                    NULL, 
    [Duration]						 NVARCHAR (50)               NOT NULL,
	[ExpirationDate]				 DATETIME                    NULL,
	[ExpiredDate]					 DATETIME					 NULL, 
    [IssuanceStatus]				 NVARCHAR (50)               NOT NULL,
	[MaintenanceRequirement]         NVARCHAR (50)               NOT NULL,
    [MaintenanceStatus]				 NVARCHAR (50)               NOT NULL,
	[Occurrence]                     NVARCHAR (50)               NOT NULL,
	[RegistrationGuid]                 UNIQUEIDENTIFIER            NULL,
	[ScheduledUpdate]				 DATETIME                    NULL,
	[SourceId]						 INT                         NOT NULL,
    [UnderReview]                    BIT                         NULL,
	--System Audit Fields
	[Created]                        DATETIME                    NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]                      NVARCHAR (255)              NOT NULL,
	[Modified]                       DATETIME                    NULL,
	[ModifiedBy]                     NVARCHAR (255)              NULL,
    CONSTRAINT [PK_Issuance]		               PRIMARY KEY CLUSTERED ([IssuanceId] ASC),
    CONSTRAINT [FK_Issuance_Credential]                      FOREIGN KEY ([CredentialId])               REFERENCES [dbo].[Credential] ([CredentialId]),
	CONSTRAINT [FK_Issuance_Source]                          FOREIGN KEY (SourceId)                     REFERENCES [dbo].[Source] (SourceId),
	CONSTRAINT [FK_Issuance_DurationType]					 FOREIGN KEY ([Duration])				    REFERENCES [dbo].[DurationType] ([Value]),
    CONSTRAINT [FK_Issuance_IssuanceStatusType]				 FOREIGN KEY ([IssuanceStatus])				REFERENCES [dbo].[IssuanceStatusType] ([Value]),
    CONSTRAINT [FK_Issuance_MaintenanceRequirementType]		 FOREIGN KEY ([MaintenanceRequirement])	    REFERENCES [dbo].[MaintenanceRequirementType] ([Value]),
    CONSTRAINT [FK_Issuance_MaintenanceStatusType]			 FOREIGN KEY ([MaintenanceStatus])		    REFERENCES [dbo].[MaintenanceStatusType] ([Value]),
    CONSTRAINT [FK_Issuance_OccurrenceType]					 FOREIGN KEY ([Occurrence])				    REFERENCES [dbo].[OccurrenceType] ([Value]), 
    CONSTRAINT [NK_Issuance] UNIQUE ([CredentialId],[IssuanceDate],[EffectiveDate])    
);

GO

CREATE NONCLUSTERED INDEX [IK_Issuance_Credential] ON [dbo].[Issuance] ([CredentialId])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_Source] ON [dbo].[Issuance] ([SourceId])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_DurationType] ON [dbo].[Issuance] ([Duration]) INCLUDE ([CredentialId],[IssuanceDate])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_IssuanceStatusType] ON [dbo].[Issuance] ([IssuanceStatus])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_MaintenanceRequirementType] ON [dbo].[Issuance] ([MaintenanceRequirement])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_MaintenanceStatusType] ON [dbo].[Issuance] ([MaintenanceStatus])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_OccurrenceType] ON [dbo].[Issuance] ([Occurrence])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_Issuancedate] ON [dbo].[Issuance] ([IssuanceDate]) INCLUDE ([IssuanceId],[CredentialId],[Occurrence])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_DeselectionEffectiveDate] ON [dbo].[Issuance] ([DeselectionEffectiveDate])
GO
CREATE NONCLUSTERED INDEX [IK_Issuance_DeselectionProcessedDate] ON [dbo].[Issuance] ([DeSelectionProcessedDate])
GO
CREATE NONCLUSTERED INDEX [NK_Issuance2] ON [dbo].[Issuance] ([Duration],[IssuanceStatus],[SourceId],[ExpirationDate])
INCLUDE ([CredentialId])
GO

  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Contains a record for each time a diplomate certifies, or recertifies, in a particular area. 	', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'Issuance'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'IssuanceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity value of the Credential record that corresponds to this Issuance (Credential contains the MemberId)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'CredentialId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date deselection was scheduled to go into effect',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'DeselectionEffectiveDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date deselection was processed',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'DeselectionProcessedDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date deselection was submitted',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'DeselectionSubmittedDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date of issuance',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'IssuanceDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the issuance becomes effective',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'EffectiveDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value indicating what type of issuance this is (TimeLimited, Continuous, Lifetime)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'Duration'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the issuance expires',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'ExpirationDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Current status of this issuance',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'IssuanceStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the diplomate is required to maintain this issuance/credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'MaintenanceRequirement'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates if the diplomate is currently meeting all requirements for this issuance/credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'MaintenanceStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value indicating whether this is an Initial or Recertification issuance',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'Occurrence'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifying value of the Registration record that resulted in this Issuance (for future use)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'RegistrationGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Next date this issuance (and related Credential) will be updated via a lookback process',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'ScheduledUpdate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Describes the organization that is responsible for granting this certification (values must be in dbo.Source table)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'SourceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value indicating if this issuance (and related Credential) is Under Review',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'UnderReview'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Actual expired date of the issuance ',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'ExpiredDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The issuance external identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Issuance',
    @level2type = N'COLUMN',
    @level2name = N'IssuanceGuid'