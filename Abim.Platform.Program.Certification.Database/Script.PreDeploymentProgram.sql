/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


-----------------------------------------------------------------------
-----------------------------------------------------------------------
-------------------------DISABLE SQL CDC------------------------------
-----------------------------------------------------------------------
-----------------------------------------------------------------------
-----------------------------------------------------------------------

--still have to enable just incase it was disabled
--EXEC sys.sp_cdc_enable_db

--DECLARE @c_Table SYSNAME 
--, @c_Schema SYSNAME 
--, @c_SQL NVARCHAR(1000) 
--, @c as CURSOR 

--SET @c = CURSOR FOR  

-- SELECT  
-- [name] as TableName 
-- ,schema_name([schema_id]) as SchemaName 
-- FROM sys.tables 
-- WHERE [type] = 'U' 
-- AND [schema_id] = schema_id('dbo')
-- AND [name] NOT LIKE 'attrep%' 
-- and [name] NOT LIKE 'QRTZ%'
-- and [name] NOT IN ('__MigrationHistory', '__RefactorLog', 'sysdiagrams')
-- AND is_ms_shipped = 0 
-- AND is_tracked_by_cdc = 1

--OPEN @c; 

-- FETCH NEXT FROM @c INTO @c_Table, @c_Schema; 
--  WHILE @@FETCH_STATUS = 0 

-- BEGIN 
--  SET @c_SQL = 'EXEC sys.sp_cdc_disable_table    
--     @source_schema = N'''+@c_Schema+''' 
--    ,@source_name = N'''+@c_Table+''' 
--    ,@capture_instance = N''' + 'All' + ''';'       

--  EXEC (@c_SQL) 

--  FETCH NEXT FROM @c INTO @c_Table, @c_Schema; 

-- END 
 
--CLOSE @c; 

--DEALLOCATE @c;