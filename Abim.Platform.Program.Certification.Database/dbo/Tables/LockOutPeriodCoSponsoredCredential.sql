CREATE TABLE [dbo].[LockOutPeriodCoSponsoredCredential]
(
	[CredentialId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
);

Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'System table used to trigger lock out period for specific credentials, outside of the scheduled yearly run of lock out period	', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LockOutPeriodCoSponsoredCredential'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier of the credential to run lock out period against',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LockOutPeriodCoSponsoredCredential',
    @level2type = N'COLUMN',
    @level2name = N'CredentialId'