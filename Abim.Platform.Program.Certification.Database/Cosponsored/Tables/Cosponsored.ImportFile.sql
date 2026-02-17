CREATE TABLE [Cosponsored].[ImportFile] (
	--Primary Key
    [ImportFileId]       INT IDENTITY(1,1)			NOT NULL,
    --All other fields 
	[FileName]				NVARCHAR(255)			NOT NULL,
	[BoardCode]				NVARCHAR(50)			NOT NULL,
	[ErrorDescription]		NVARCHAR(255)			NULL,
	[DownloadedDate]		DATETIME				NOT NULL,
    [ProcessedDate]			DATETIME				NULL,
	[ProcessStatus]			VARCHAR(50)				NOT NULL	DEFAULT('NewFile'),
	[IsNotificationSent]	BIT						NOT NULL	DEFAULT(0),
    --System Audit Fields
	[Created]				DATETIME				NOT NULL	DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)			NOT NULL,
	[Modified]				DATETIME				NULL,
	[ModifiedBy]			NVARCHAR (255)			NULL,
    CONSTRAINT [PK_ImportFile]							PRIMARY KEY CLUSTERED	([ImportFileId] ASC),
	CONSTRAINT [FK_ImportFile_FileProcessStatusType]	FOREIGN KEY				([ProcessStatus])	REFERENCES	[Cosponsored].[FileProcessStatusType]	([Value])
);
GO

  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Processed File info of files recieved from Cosponsored Boards', 
  @level0type = N'Schema', 
  @level0name = 'Cosponsored', 
  @level1type = N'Table', 
  @level1name = 'ImportFile'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFile',
    @level2type = N'COLUMN',
    @level2name = N'ImportFileId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Name of the processed file that was recieved from Cosponsored Board',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFile',
    @level2type = N'COLUMN',
    @level2name = N'FileName'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The Board Code used by the Vendor',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFile',
    @level2type = N'COLUMN',
    @level2name = N'BoardCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description of Error recieved while processing the file',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFile',
    @level2type = N'COLUMN',
    @level2name = N'ErrorDescription'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Date the file recieved from Cosponsored Board was processed',
    @level0type = N'SCHEMA',
    @level0name = N'Cosponsored',
    @level1type = N'TABLE',
    @level1name = N'ImportFile',
    @level2type = N'COLUMN',
    @level2name = N'ProcessedDate'
GO