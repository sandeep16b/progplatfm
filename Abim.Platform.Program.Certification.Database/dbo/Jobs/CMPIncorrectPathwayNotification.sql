DECLARE
    @Title      VARCHAR(100),
    @JobName    VARCHAR(100),
	@DatabaseName VARCHAR(100),
    @Command    VARCHAR(MAX),
    @ReturnCode INT;
SET @Title = '[DB Notifications] CMP with Incorrect Pathway';
SET @JobName = REPLACE(REPLACE(@Title, '[DB Notifications]', '$(DbPrefix)[DB Notifications]'),'_blank_','');
SET @DatabaseName = REPLACE('$(DbPrefix)Certification','_blank_','');
SET @ReturnCode = 0;
SET @Command = 
'
USE $(DbPrefix)Certification
DECLARE @sql VARCHAR(8000)
SET @sql = ''
	Declare	@EmailTo VARCHAR(500),
	@Subject VARCHAR(50)
	SET @EmailTo = ''''Jarret Bauer <JBauer@ABIM.ORG>;Michael Yagley <myagley@abim.org>;Alan Hummel <ahummel@abim.org>''''
	SET @Subject = ''''IsInCMP set with Incorrect Pathway''''
	DECLARE @server sysname, @Severity INT = 1
	
	SELECT @server = cast(serverproperty(''''servername'''') as sysname)
	DECLARE @SeverityDescription VARCHAR(20)
	SET @SeverityDescription = ''''CRITICAL''''
	DECLARE @SeverityColor VARCHAR(20)
	SET @SeverityColor = ''''RED''''
	DECLARE @abimId varchar(6)
	DECLARE @certificationName varchar(100)
	DECLARE @credentialGuid uniqueidentifier
	DECLARE @pathway varchar(100)
	DECLARE @isInCMP varchar(10)
	DECLARE @tmpSQL VARCHAR(max) = '''''''' 
	DECLARE @cnt int = 0
	DECLARE db_cursor CURSOR FOR  
	select AbimId, c.Name as CertificationName, cd.Pathway, cd.CredentialGuid, CASE cd.IsInCMP when 1 then ''''True'''' else ''''False'''' end as IsInCMP
	from $(DbPrefix)Certification..Credential cd with (nolock)
	join $(DbPrefix)Certification..Certification c with (nolock)
	on cd.CertificationId = c.CertificationId
	join $(DbPrefix)Certification..UserProfile u with (nolock)
	on cd.MemberId = u.MemberId
	where cd.IsInCMP = 1 and cd.Pathway <> ''''1Year''''
	union all
	select AbimId, c.Name as CertificationName, cd.Pathway, cd.CredentialGuid, CASE cd.IsInCMP when 1 then ''''True'''' else ''''False'''' end as IsInCMP
	from $(DbPrefix)Certification..Credential cd with (nolock)
	join $(DbPrefix)Certification..Certification c with (nolock)
	on cd.CertificationId = c.CertificationId
	join $(DbPrefix)Certification..UserProfile u with (nolock)
	on cd.MemberId = u.MemberId
	where cd.IsInCMP = 0 and cd.Pathway = ''''1Year''''
	order by AbimId
	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @abimId, @certificationName, @pathway,@credentialGuid,@isInCMP   
	WHILE @@FETCH_STATUS = 0   
	BEGIN   
		   set @cnt = @cnt + 1
		   set @tmpSQL = @tmpSQL + ''''<tr>
				<td align="left">'''' + @abimId + ''''</td>
				<td align="left">'''' + @certificationName + ''''</td>
				<td align="left">'''' + cast(@credentialGuid as varchar(300)) + ''''</td>
				<td align="left">'''' + @pathway + ''''</td>
				<td align="left">'''' + @isInCMP + ''''</td>
			</tr>''''
		   FETCH NEXT FROM db_cursor INTO @abimId, @certificationName, @pathway   ,@credentialGuid,@isInCMP   
	END   
	CLOSE db_cursor   
	DEALLOCATE db_cursor
	if @cnt > 0
	BEGIN
		DECLARE @body1 nvarchar(max)
		SET @body1 = 
		''''<html>
		<head></head>
		<body style="background: #ffffff; color: #003399"> 
			<table border="1" align="center"  cellpadding="3"  width="500">
				<tr>
					<td align="center" colspan="5" style="font-size:24;color:#003399"><b>Incorrect pathway for credentials in CMP</b></td>
				</tr>
				<tr>
					<td align="center">ABIM ID</td>
					<td align="center">Certification Name</td>
					<td align="center">Credential Guid</td>
					<td align="center">Current Pathway</td>
					<td align="center">Is In CMP?</td>
				</tr>
				'''' + @tmpSQL + ''''
			</table>
		</body>
		</html>''''
		DECLARE @subject1 nvarchar(70)
		SET @subject1 = @server + '''' '''' + @SeverityDescription + '''' '''' + @Subject
		EXEC msdb.dbo.sp_send_dbmail @recipients=@EmailTo,
			@subject = @subject1,
			@body = @body1,
			@body_format = ''''HTML'''',
			@profile_name = ''''ABIM Mail'''',
			@importance = ''''High'''';
	END 
		
		''
EXEC (@sql);
';

SET @Command = REPLACE(@Command,'_blank_','');

BEGIN TRANSACTION;
IF NOT EXISTS
    (
        SELECT name FROM msdb.dbo.syscategories WHERE name = N'CertificationJobs' AND category_class = 1
    )
BEGIN
    EXEC @ReturnCode = msdb.dbo.sp_add_category
        @class = N'JOB',
        @type = N'LOCAL',
        @name = N'CertificationJobs';
    IF (@@ERROR <> 0 OR @ReturnCode <> 0)
        GOTO QuitWithRollback;
END;
IF EXISTS (SELECT job_id FROM msdb.dbo.sysjobs WHERE name = @JobName)
BEGIN
    EXEC @ReturnCode = msdb.dbo.sp_delete_job
        @job_name = @JobName,
        @delete_unused_schedule = 1;
    IF (@@ERROR <> 0 OR @ReturnCode <> 0)
        GOTO QuitWithRollback;
END;
DECLARE @jobId BINARY(16);
EXEC @ReturnCode = msdb.dbo.sp_add_job
    @job_name = @JobName,
    @enabled = 1,
    @notify_level_eventlog = 0,
    @notify_level_email = 0,
    @notify_level_netsend = 0,
    @notify_level_page = 0,
    @delete_level = 0,
    @description = N'This will notify IT of credentials where IsInCMP = 1 but Pathway <> 1Year',
    @category_name = N'CertificationJobs',
    @job_id = @jobId OUTPUT;
IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep
    @job_id = @jobId,
    @step_name = N'Send email',
    @step_id = 1,
    @cmdexec_success_code = 0,
    @on_success_action = 1,
    @on_success_step_id = 0,
    @on_fail_action = 2,
    @on_fail_step_id = 0,
    @retry_attempts = 0,
    @retry_interval = 0,
    @os_run_priority = 0,
    @subsystem = N'TSQL',
    @command = @Command,
    @database_name = @DatabaseName,
    @flags = 0;
IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;
EXEC @ReturnCode = msdb.dbo.sp_update_job
    @job_id = @jobId,
    @start_step_id = 1;
IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule
    @job_id = @jobId,
    @name = 'Every day at 10am',
    @enabled = 1,
    @freq_type = 4,
    @freq_interval = 1,
    @freq_subday_type = 1,
    @freq_subday_interval = 0,
    @freq_relative_interval = 0,
    @freq_recurrence_factor = 0,
    @active_start_date = 20220421,
    @active_end_date = 99991231,
    @active_start_time = 100000,
    @active_end_time = 235959;
IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver
    @job_id = @jobId,
    @server_name = N'(local)';
IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;
COMMIT TRANSACTION;
GOTO EndSave;
QuitWithRollback:
IF (@@TRANCOUNT > 0)
    ROLLBACK TRANSACTION;
EndSave:
GO