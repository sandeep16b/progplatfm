CREATE TABLE [dbo].[Source] (
	--Primary Key
    [SourceId]      INT IDENTITY(1,1) NOT NULL,
	--Natural Key
	[Code]          NVARCHAR(10)      NOT NULL, 
	--Non-clustered key(s)
	[SourceGuid]    UNIQUEIDENTIFIER  NOT NULL DEFAULT newsequentialid(),
	--All other fields (sorted alphabetically)
    [Name]          NVARCHAR (255)    NOT NULL,
	[ShortName]		NVARCHAR(50)	  NULL,
	--System Audit Fields
	[Created]		DATETIME          NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]	    NVARCHAR (255)    NOT NULL,
	[Modified]	    DATETIME          NULL,
	[ModifiedBy]	NVARCHAR (255)    NULL,
    CONSTRAINT [PK_Source]	  PRIMARY KEY CLUSTERED ([SourceId] ASC), 
    CONSTRAINT [NK_Source_Code] UNIQUE ([Code])
);

GO

CREATE UNIQUE NONCLUSTERED INDEX [NCK_Source] ON [dbo].[Source] ([SourceGuid])
GO

 EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'List of organizations offering Credentials in areas of Certification', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'Source'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Source',
    @level2type = N'COLUMN',
    @level2name = N'SourceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Code of the Source record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Source',
    @level2type = N'COLUMN',
    @level2name = N'Code'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique Identifier Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Source',
    @level2type = N'COLUMN',
    @level2name = N'SourceGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description of the Source record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Source',
    @level2type = N'COLUMN',
    @level2name = N'Name'