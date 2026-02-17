CREATE TABLE [dbo].[LookbackMember]
(
	[MemberId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY
);

Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'System table used to trigger year end lookback for specific individuals, outside of the scheduled yearly run of year end lookback	', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LookbackMember'

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Unique identifier of the diplomate to run year end lookback against',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackMember',
    @level2type = N'COLUMN',
    @level2name = N'MemberId'