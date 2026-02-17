CREATE TABLE [dbo].[IssuanceStatusType] (
	--Primary Key
    [Value]									NVARCHAR (50)							NOT NULL,
    --All other fields (sorted alphabetically)
	[Description]								NVARCHAR (255)								NOT NULL,
	--System Audit Fields
	[Created]				DATETIME DEFAULT(CURRENT_TIMESTAMP)	 NOT NULL,
	[CreatedBy]				NVARCHAR (255)							 NOT NULL,
	[Modified]				DATETIME								 NULL,
	[ModifiedBy]			NVARCHAR (255)							 NULL,
    CONSTRAINT [PK_IssuanceStatusType]	PRIMARY KEY CLUSTERED ([Value] ASC)
);

GO
 EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Indicates if the diplomate is currently maintaining that given Issuance/Credential', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'IssuanceStatusType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value representing the status of the Issuance',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'IssuanceStatusType',
    @level2type = N'COLUMN',
    @level2name = N'Value'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description of the IssuanceStatusType value',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'IssuanceStatusType',
    @level2type = N'COLUMN',
    @level2name = N'Description'