CREATE TABLE dbo.LookbackDateInfo
(	
       LookbackDateInfoId		INT						IDENTITY(1,1) NOT NULL,
       MemberId					uniqueidentifier		NOT NULL CONSTRAINT UK_LookbackDateInfo_MemberId UNIQUE (MemberId),
       Lookback2YearEndDate     DATETIME				,
       Lookback2YearStartDate   DATETIME				,
       Lookback5YearEndDate     DATETIME				,
       Lookback5YearStartDate   DATETIME				,
	   Created                  DATETIME                DEFAULT(GETDATE()),
       CreatedBy                NVARCHAR (255)          NOT NULL,
       Modified                 DATETIME                ,
       ModifiedBy               NVARCHAR (255)          ,

       CONSTRAINT PK_LookbackDateInfo PRIMARY KEY CLUSTERED  ( LookbackDateInfoId  ASC)

)
Go
  EXEC Sp_addextendedproperty 
  @name = N'MS_Description', 
  @value = 'Table containing lookback start and end dates for each diplomate (both 2 and 5 year)', 
  @level0type = N'Schema', 
  @level0name = 'dbo', 
  @level1type = N'Table', 
  @level1name = 'LookbackDateInfo'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The LoobackDateInfo identifier',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'LookbackDateInfoId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'External identifier (guid) of the diplomate that these lookback dates are associated with',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'MemberId'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'End date of the 2 year lookback period',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'Lookback2YearEndDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Start date of the 2 year lookback period',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'Lookback2YearStartDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'End date of the 5 year lookback period',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'Lookback5YearEndDate'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Start date of the 5 year lookback period',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'LookbackDateInfo',
    @level2type = N'COLUMN',
    @level2name = N'Lookback5YearStartDate'