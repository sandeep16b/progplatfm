CREATE TABLE [dbo].[CertificationType] (
	--Primary Key
    [Value]										NVARCHAR (50)							NOT NULL,
    --All other fields (sorted alphabetically)
	[Description]								NVARCHAR (255)								NOT NULL,
	--System Audit Fields
	[Created]				DATETIME DEFAULT(CURRENT_TIMESTAMP)	 NOT NULL,
	[CreatedBy]				NVARCHAR (255)							 NOT NULL,
	[Modified]				DATETIME								 NULL,
	[ModifiedBy]			NVARCHAR (255)							 NULL,
    CONSTRAINT [PK_CertificationType] PRIMARY KEY CLUSTERED ([Value] ASC)
);

Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Areas a diplomate can earn a Credential in. Contains both ABIM and Other Board Certifications', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'CertificationType'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value representing the type of Certification',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CertificationType',
    @level2type = N'COLUMN',
    @level2name = N'Value'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Description of the CertificationType value',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'CertificationType',
    @level2type = N'COLUMN',
    @level2name = N'Description'