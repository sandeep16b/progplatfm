CREATE TABLE [dbo].[MaintenanceRequirementType] (
	--Primary Key
    [Value]									NVARCHAR (50)							NOT NULL,
    --All other fields (sorted alphabetically)
	[Description]								NVARCHAR (255)								NOT NULL,
	--System Audit Fields
	[Created]				DATETIME DEFAULT(CURRENT_TIMESTAMP)	 NOT NULL,
	[CreatedBy]				NVARCHAR (255)							 NOT NULL,
	[Modified]				DATETIME								 NULL,
	[ModifiedBy]			NVARCHAR (255)							 NULL,
    CONSTRAINT [PK_MaintenanceRequirementType]	PRIMARY KEY CLUSTERED ([Value] ASC)
);

GO
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'A lookup table used to indicate if an Issuance is required to be maintained or not', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'MaintenanceRequirementType'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The value',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MaintenanceRequirementType',
    @level2type = N'COLUMN',
    @level2name = N'Value'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The description',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'MaintenanceRequirementType',
    @level2type = N'COLUMN',
    @level2name = N'Description'