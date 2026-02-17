CREATE TABLE [dbo].[CredentialDateLog]
(
	--Primary Key
	[CredentialDateLogId]            INT IDENTITY(1,1)           NOT NULL,
	--Natural Key
	[ChangedDate]					 DATETIME2(7)                NOT NULL,
    [CredentialGuid]				 UNIQUEIDENTIFIER            NOT NULL, 
	[CredentialDate]			     NVARCHAR(50)				 NOT NULL,
	--All other fields (sorted alphabetically)
	[NewValue]						 DATETIME2(7)                NULL,
	[OldValue]						 DATETIME2(7)                NULL,
	--System Audit Fields
	[Created]				DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,

    CONSTRAINT [PK_CredentialDateLog]			      PRIMARY KEY CLUSTERED   ([CredentialDateLogId] ASC),
    CONSTRAINT [FK_CredentialDateLog_Credential]  FOREIGN KEY             ([CredentialGuid]) REFERENCES [dbo].[Credential] ([CredentialGuid]),
    CONSTRAINT [FK_CredentialDateLog_CredentialDate]    FOREIGN KEY             ([CredentialDate])		  REFERENCES [dbo].[CredentialDateType] ([Value]),
    CONSTRAINT [NK_CredentialDateLog] UNIQUE ([CredentialGuid],[CredentialDate],[ChangedDate]),
)
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Log table used to store any changes made to credential level dates (dates that are tracked found in CredentialDateType table)', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'CredentialDateLog'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The CredentialDateLog identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'CredentialDateLogId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The date/time that this credential date value changed',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'ChangedDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'External identifier (guid) of the credential that changed (can be used to join to the Credential table)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'CredentialGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates which date was changed (acceptable values found in CredentialDateType table)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'CredentialDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The new value for this credential date field',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'NewValue'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The prior value for this credential date field',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CredentialDateLog',
    @level2type = N'COLUMN',
    @level2name = N'OldValue'