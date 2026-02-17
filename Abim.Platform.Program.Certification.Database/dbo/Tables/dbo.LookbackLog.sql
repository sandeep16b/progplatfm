
CREATE TABLE [dbo].[LookbackLog]
(
	--Primary Key
	[LookbackLogId]                  INT IDENTITY(1,1)           NOT NULL,
	--Natural Key
    [CredentialGuid]				 UNIQUEIDENTIFIER            NOT NULL, 
	[LookbackAction]			     NVARCHAR(50)				 NOT NULL,
	[LookbackLogDate]				 DATETIME                    NOT NULL,
	[LookbackReason]			     NVARCHAR(50)				 NOT NULL,
	[LookbackStatus]			     NVARCHAR(50)				 NOT NULL,
	--All other fields (sorted alphabetically)
	[IsPendingAction]				 BIT						 NOT NULL DEFAULT 0,
	--System Audit Fields
	[Created]				DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,

    CONSTRAINT [PK_LookbackLog]			      PRIMARY KEY CLUSTERED   ([LookbackLogId] ASC),
    CONSTRAINT [FK_LookbackLog_Credential]  FOREIGN KEY             ([CredentialGuid]) REFERENCES [dbo].[Credential] ([CredentialGuid]),
    CONSTRAINT [FK_LookbackLog_LookbackAction]    FOREIGN KEY             ([LookbackAction])		  REFERENCES [dbo].[LookbackActionType] ([Value]),
	CONSTRAINT [FK_LookbackLog_LookbackReason] FOREIGN KEY             ([LookbackReason])		      REFERENCES [dbo].[LookbackReasonType] ([Value]), 
	CONSTRAINT [FK_LookbackLog_LookbackStatus] FOREIGN KEY             ([LookbackStatus])		      REFERENCES [dbo].[LookbackStatusType] ([Value]), 

)
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Log table used to store all actions taking during each year end lookback run (implemented February 2019)', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LookbackLog'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The LoobackLog identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackLogId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'External identifier (guid) of the credential that is associated with this log record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'CredentialGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Action taken against the credential (failure or restoration) **feature not yet in use',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackAction'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date of which this year end lookback action was taken against this credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackLogDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates which rule, or rules, were found to be not met, causing year end lookback to set the credential in a negative state',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackReason'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Indicates which status on the credential was affected by the year end lookback rule not being met (credential or participation)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'LookbackStatus'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating if this action is still pending due to the diplomate having more time to meet the rule **feature not yet in use',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackLog',
    @level2type = N'COLUMN',
    @level2name = N'IsPendingAction'