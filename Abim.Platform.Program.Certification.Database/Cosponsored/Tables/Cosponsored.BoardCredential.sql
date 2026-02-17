CREATE TABLE [Cosponsored].[BoardCredential] (
	--Primary Key
    [BoardCredentialId]		INT IDENTITY(1,1)		NOT NULL,
    --All other fields 
	[BoardCode]				NVARCHAR(50)			NOT NULL,
	[BoardName]				NVARCHAR(255)			NOT NULL,
	[ExamName]				NVARCHAR(255)			NOT NULL,
	[ExamType]				NVARCHAR(255)			NOT NULL,
    [BoardPrimaryCertName]	NVARCHAR(255)			NULL,
	[BoardPrimaryCertGuid]	UNIQUEIDENTIFIER		NULL,
	[AbimInitialCertName]	NVARCHAR(255)			NULL,
	[AbimInitialCertGuid]	UNIQUEIDENTIFIER		NULL,
    --System Audit Fields
	[Created]				DATETIME				NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,
    CONSTRAINT [PK_BoardCredential]	PRIMARY KEY CLUSTERED ([BoardCredentialId] ASC)
);
GO

  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Processed File info of files recieved from Cosponsored Boards', 
  @level0type = N'Schema', 
  @level0name = 'Cosponsored', 
  @level1type = N'Table', 
  @level1name = 'BoardCredential'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'BoardCredentialId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The Board Code used by the Vendor',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'BoardCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The Name of the Board',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'BoardName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name of Exam',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'ExamName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Type of Exam',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'ExamType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name of Primary Cert issued by Cosponsored Board',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'BoardPrimaryCertName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier of Primary Cert issued by Cosponsored Board',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'BoardPrimaryCertGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name of Initial Cert issued by ABIM',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'AbimInitialCertName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier of Initial Cert issued by ABIM',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'BoardCredential',
    @level2type = N'COLUMN',
    @level2name = N'AbimInitialCertGuid'
GO