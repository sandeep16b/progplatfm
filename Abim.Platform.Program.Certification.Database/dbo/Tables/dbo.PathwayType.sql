CREATE TABLE [dbo].[PathwayType] (
	--Primary Key
    [Value]									    NVARCHAR (50)							NOT NULL,
    --All other fields (sorted alphabetically)
	[Description]								NVARCHAR (255)								NOT NULL,
	--System Audit Fields
	[Created]				DATETIME DEFAULT(CURRENT_TIMESTAMP)	 NOT NULL,
	[CreatedBy]				NVARCHAR (255)							 NOT NULL,
	[Modified]				DATETIME								 NULL,
	[ModifiedBy]			NVARCHAR (255)							 NULL,
    CONSTRAINT [PK_PathwayType] PRIMARY KEY CLUSTERED ([Value] ASC)
);

GO
 EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = '2 and 10 year options given to diplomates for maintaining good status', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'PathwayType'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value representing the pathway selected on the Credential',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'PathwayType',
    @level2type = N'COLUMN',
    @level2name = N'Value'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description of the PathwayType value',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'PathwayType',
    @level2type = N'COLUMN',
    @level2name = N'Description'