CREATE TABLE [dbo].[LookbackActionType]
(
	--Primary Key
    [Value]									NVARCHAR (50)							NOT NULL,
    --All other fields (sorted alphabetically)
	[Description]								NVARCHAR (255)								NOT NULL,
	--System Audit Fields
	[Created]				DATETIME DEFAULT(CURRENT_TIMESTAMP)	 NOT NULL,
	[CreatedBy]				NVARCHAR (255)							 NOT NULL,
	[Modified]				DATETIME								 NULL,
	[ModifiedBy]			NVARCHAR (255)							 NULL,
    CONSTRAINT [PK_LookbackActionType]	PRIMARY KEY CLUSTERED ([Value] ASC)
);
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'A lookup table containing actions that can be taken during year end lookback', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LookbackActionType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The value',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackActionType',
    @level2type = N'COLUMN',
    @level2name = N'Value'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The description',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackActionType',
    @level2type = N'COLUMN',
    @level2name = N'Description'