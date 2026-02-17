CREATE TABLE [dbo].[CredentialDateType]
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
    CONSTRAINT [PK_CredentialDateType]	PRIMARY KEY CLUSTERED ([Value] ASC)
);
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'A lookup table containing a list of credential dates that are tracked in the CredentialDateLog table', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'CredentialDateType'