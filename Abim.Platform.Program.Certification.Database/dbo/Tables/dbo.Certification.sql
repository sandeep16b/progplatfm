
CREATE TABLE [dbo].[Certification] (
	--Primary Key
    [CertificationId]       INT IDENTITY(1,1)     NOT NULL,
	--Natural Key
	[SourceId]              INT                   NOT NULL,
	[Code]                  NVARCHAR (50)         NOT NULL,
	--Non-clustered key(s)
	[CertificationGuid]     UNIQUEIDENTIFIER      NOT NULL DEFAULT newsequentialid(),
    --All other fields (sorted alphabetically)
	[AddedQualification]    BIT                   NOT NULL,
	[BaseCertificationId]   INT                   NULL, 
	[ConsecutiveAttempt]	INT					  NULL,
    [Name]                  NVARCHAR (255)        NOT NULL,
    [Type]                  NVARCHAR (50)         NOT NULL,
    --System Audit Fields
	[Created]				DATETIME              NOT NULL DEFAULT(CURRENT_TIMESTAMP),
	[CreatedBy]				NVARCHAR (255)        NOT NULL,
	[Modified]				DATETIME              NULL,
	[ModifiedBy]			NVARCHAR (255)        NULL,
    CONSTRAINT [PK_Certification]			        PRIMARY KEY CLUSTERED ([CertificationId] ASC),
    CONSTRAINT [FK_Certification_BaseCertification] FOREIGN KEY ([BaseCertificationId])		REFERENCES [dbo].[Certification] ([CertificationId]),
	CONSTRAINT [FK_Certification_CertficationType]	FOREIGN KEY ([Type])			        REFERENCES [dbo].[CertificationType] ([Value]),
	CONSTRAINT [FK_Certification_Source]			FOREIGN KEY ([SourceId])				REFERENCES [dbo].[Source] ([SourceId]), 
    CONSTRAINT [NK_Certification] UNIQUE ([SourceId], [Code]), 

);


GO

CREATE UNIQUE NONCLUSTERED INDEX [NCK_Certification] ON [dbo].[Certification] ([CertificationGuid])
GO
CREATE NONCLUSTERED INDEX [IK_Certification_BaseCertification] ON [dbo].[Certification] ([BaseCertificationId])
GO
CREATE NONCLUSTERED INDEX [IK_Certification_CertificationType] ON [dbo].[Certification] ([Type])
GO
CREATE NONCLUSTERED INDEX [IK_Certification_Source] ON [dbo].[Certification] ([SourceId])
GO

Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Areas a diplomate can earn a Credential in. Contains both ABIM and Other Board Certifications', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'Certification'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Identity Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'CertificationId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Describes the organization that is responsible for granting this certification (values must be in dbo.Source table)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'SourceId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Discipline abbreviation for the base exam of this certification',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'Code'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique Identifier Value for Record',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'CertificationGuid'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Flag indicating that this certificate has an underlying required subspecialty cert (BaseCertificationId is not null and indicates which cert is required)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'AddedQualification'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Aka Tertiary Cert, this indicates which other certificate the diplomate is required to obtain before being able to earn this particular certificate (some certs have IM has required, but will not be "AddedQualification")',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'BaseCertificationId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Numeric value indicating how many consecutive Failed attempts are allowed before the diplomate must skip an administration',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'ConsecutiveAttempt'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value describing the discipline for this certificate',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Value describing the type of certificate - subspecialty, primary, etc (values must be in dbo.CertificateType)',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Certification',
    @level2type = N'COLUMN',
    @level2name = N'Type'