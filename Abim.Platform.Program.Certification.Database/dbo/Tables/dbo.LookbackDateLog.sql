CREATE TABLE [dbo].[LookbackDateLog]
(
	--Primary Key
	[LookbackDateLogId]            INT IDENTITY(1,1)           NOT NULL,
	--Natural Key
	[ChangedDate]					 DATETIME2(7)                NOT NULL,
    [MemberGuid]					 UNIQUEIDENTIFIER            NOT NULL, 
	[LookbackDate]				     NVARCHAR(50)				 NOT NULL,
	--All other fields (sorted alphabetically)
	[NewValue]						 DATETIME2(7)                NULL,
	[OldValue]						 DATETIME2(7)                NULL,
	--System Audit Fields
	[Created]				DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,

    CONSTRAINT [PK_LookbackDateLog]			      PRIMARY KEY CLUSTERED   ([LookbackDateLogId] ASC),
    CONSTRAINT [FK_LookbackDateLog_LookbackDate]    FOREIGN KEY             ([LookbackDate])		  REFERENCES [dbo].[LookbackDateType] ([Value]),
    CONSTRAINT [NK_LookbackDateLog] UNIQUE ([MemberGuid],[LookbackDate],[ChangedDate]),
)
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Log table used to store any changes made to lookback dates (dates that are tracked found in LookbackDateType table)	', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LookbackDateLog'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The LoobackDateLog identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackDateLogId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date which the credential date value changed in the system',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateLog',
    @level2type = N'COLUMN',
    @level2name = N'ChangedDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'External identifier (guid) of the diplomate that this date change is associated with',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateLog',
    @level2type = N'COLUMN',
    @level2name = N'MemberGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value indicating which lookback date was changed',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The new value for this lookback date field',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateLog',
    @level2type = N'COLUMN',
    @level2name = N'NewValue'