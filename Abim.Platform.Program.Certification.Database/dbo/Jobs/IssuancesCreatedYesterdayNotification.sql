DECLARE
    @Title      VARCHAR(100),
    @JobName    VARCHAR(100),
	@DatabaseName VARCHAR(100),
    @Command    VARCHAR(MAX),
    @ReturnCode INT;

SET @Title = '[DB Notifications] Issuances Created Yesterday';
SET @JobName = REPLACE(REPLACE(@Title, '[DB Notifications]', '$(DbPrefix)[DB Notifications]'),'_blank_','');
SET @DatabaseName = REPLACE('$(DbPrefix)Certification','_blank_','');
SET @ReturnCode = 0;
SET @Command = 
'USE $(DbPrefix)Certification

Declare	@EmailTo VARCHAR(500),
	@Subject VARCHAR(50)

	SET @EmailTo = ''Jarret Bauer <JBauer@ABIM.ORG>;Alex Reznitchenko <AReznitchenko@ABIM.ORG>; Ramesh Kaliappan <RKaliappan@ABIM.ORG>; Natalie R. Trahey <NTrahey@ABIM.ORG>; Florence Mickens <FMickens@ABIM.ORG>; Tatiana Bohush <TBohush@ABIM.ORG>;Michael Yagley <MYagley@ABIM.ORG>''--;Michael Yagley <myagley@abim.org>;Alan Hummel <ahummel@abim.org>''
	SET @Subject = ''Issuances Created Yesterday''

	DECLARE @server sysname, @Severity INT = 1
	
	SELECT @server = cast(serverproperty(''servername'') as sysname)

	DECLARE @SeverityDescription VARCHAR(20)
	SET @SeverityDescription = ''CRITICAL''

	DECLARE @SeverityColor VARCHAR(20)
	SET @SeverityColor = ''RED''

	DECLARE @abimId varchar(6)
	DECLARE @certificationName varchar(100)
	DECLARE @IsCosponsored varchar(20)
	DECLARE @credentialGuid uniqueidentifier
	DECLARE @IssuanceDate varchar(20)
	DECLARE @ExamDueDate varchar(20)
	DECLARE @RecentAdministrationDate varchar(20)
	DECLARE @RecentExamResult varchar(20)
	DECLARE @RecentExamType varchar(20)
	DECLARE @LKAEnrollmentStatus varchar(20)
	DECLARE @LKAEnrollmentDate varchar(20)
	DECLARE @CMPTestDate varchar(20)
	DECLARE @CMPTestResult varchar(20)
	DECLARE @CMPOnHold varchar(20)
	DECLARE @IsInCMP varchar(20)
	DECLARE @Pathway varchar(20)
	DECLARE @tmpSQL VARCHAR(max) = '''' 
	DECLARE @cnt int = 0

	set transaction isolation level read uncommitted

	DECLARE db_cursor CURSOR FOR  
	select u.AbimId, c.Name as Discipline, cd.CredentialGuid, CONVERT(varchar,i.IssuanceDate,101) as IssuanceDate, CONVERT(varchar,cd.ExamDueDate,101) as ExamDueDate
	,isnull((select top 1 FORMAT(ad.AdministrationDate,''MM/dd/yyyy'') as AdministrationDate
	from $(DbPrefix)Registration.dbo.Registration (nolock) reg 
	inner join $(DbPrefix)Registration.dbo.Administration (nolock) ad on reg.AdministrationId=ad.AdministrationId
	inner join $(DbPrefix)Registration.dbo.Exam (nolock) ex on ad.ExamId=ex.ExamId 
	where reg.MemberId=cd.MemberId and ex.CertificationGuid=c.CertificationGuid
	order by ad.AdministrationDate desc), '''') as ''Most Recent ABIM AdministrationDate'',
----------------------------------------------------------------------------------------------------
isnull((select top 1 ExamResult
	from $(DbPrefix)Registration.dbo.Registration (nolock) reg 
	inner join $(DbPrefix)Registration.dbo.Administration (nolock) ad on reg.AdministrationId=ad.AdministrationId
	inner join $(DbPrefix)Registration.dbo.Exam (nolock) ex on ad.ExamId=ex.ExamId 
	where reg.MemberId=cd.MemberId and ex.CertificationGuid=c.CertificationGuid
	order by ad.AdministrationDate desc), '''') as ''Most Recent ExamResult''
-------------------------------------------------------------------------------------------------
,isnull((select top 1 ex.ExamType
	from $(DbPrefix)Registration.dbo.Registration (nolock) reg 
	inner join $(DbPrefix)Registration.dbo.Administration (nolock) ad on reg.AdministrationId=ad.AdministrationId
	inner join $(DbPrefix)Registration.dbo.Exam (nolock) ex on ad.ExamId=ex.ExamId 
	where reg.MemberId=cd.MemberId and ex.CertificationGuid=c.CertificationGuid
	order by ad.AdministrationDate desc), '''') as ''Most Recent ExamType''
-------------------------------------------------------------------------------------------------
,ISNULL((SELECT top 1
lg.[EnrollmentStatus]
  FROM $(DbPrefix)Registration.[Longitudinal].[LongitudinalEnrollment]  lg with (nolock)
  inner join $(DbPrefix)Registration.[Longitudinal].[LongitudinalParticipation] lgPart with (nolock) on lgPart.[EnrollmentId]=lg.[EnrollmentId]
  where lg.CredentialGuid=cd.CredentialGuid and lg.MemberId=cd.MemberId
  order by lg.[EnrollmentDate] desc ),'''') as ''LgEnrollmentStatus''
--------------------------------------------------------------------------------------------------
,ISNULL((SELECT top 1
FORMAT(lg.[EnrollmentDate],''MM/dd/yyyy'') 
  FROM $(DbPrefix)Registration.[Longitudinal].[LongitudinalEnrollment]  lg with (nolock)
  inner join $(DbPrefix)Registration.[Longitudinal].[LongitudinalParticipation] lgPart with (nolock) on lgPart.[EnrollmentId]=lg.[EnrollmentId]
  where lg.CredentialGuid=cd.CredentialGuid and lg.MemberId=cd.MemberId
  order by lg.[EnrollmentDate] desc ),'''') as ''LgEnrollmentDate''
,ISNULL((select top 1 FORMAT(cmpr.TestDate,''MM/dd/yyyy'') 
	from $(DbPrefix)Registration..CMPRegistration (nolock) cmpr 
	inner join $(DbPrefix)Registration..CMPExam (nolock) cmpe on cmpr.CMPExamId=cmpe.CMPExamId
	where cmpr.MemberId=cd.MemberId and cmpe.CertificationGuid=c.CertificationGuid
	order by cmpr.TestDate desc), '''') as ''Most Recent CMP Test Date''
,ISNULL((select top 1 cmper.Value
	from $(DbPrefix)Registration..CMPRegistration (nolock) cmpr 
	inner join $(DbPrefix)Registration..CMPExam (nolock) cmpe on cmpr.CMPExamId=cmpe.CMPExamId
	inner join $(DbPrefix)Registration..CMPExamResult cmper on cmpr.CMPExamResultId = cmper.CMPExamResultId
	where cmpr.MemberId=cd.MemberId and cmpe.CertificationGuid=c.CertificationGuid
	order by cmpr.TestDate desc), '''') as ''Most Recent CMP Test Result''
,ISNULL((select top 1 case cmpr.OnHold when 1 then ''TRUE'' else ''FALSE'' end
	from $(DbPrefix)Registration..CMPRegistration (nolock) cmpr 
	inner join $(DbPrefix)Registration..CMPExam (nolock) cmpe on cmpr.CMPExamId=cmpe.CMPExamId
	inner join $(DbPrefix)Registration..CMPExamResult cmper on cmpr.CMPExamResultId = cmper.CMPExamResultId
	where cmpr.MemberId=cd.MemberId and cmpe.CertificationGuid=c.CertificationGuid
	order by cmpr.TestDate desc), '''') as ''Most Recent CMP On Hold''
-------------------------------------------------------------------------------------------------
, CASE cd.IsInCMP when 1 then ''TRUE'' else ''FALSE'' end as IsInCMP
, cd.Pathway
, case cd.IsCosponsored when 1 then ''TRUE'' else ''FALSE'' end as IsCosponsored
	from $(DbPrefix)Certification..Credential cd
	join $(DbPrefix)Certification..Certification c
	on cd.CertificationId = c.CertificationId
	join $(DbPrefix)Certification..Issuance i 
	on cd.CredentialId = i.CredentialId
	join $(DbPrefix)Certification..UserProfile u
	on cd.MemberId = u.MemberId
	where cast(i.Created as date) = dateadd(day, -1, cast(getdate() as date))
	and i.SourceId = 1
	order by u.AbimId, c.Name

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @abimId, @certificationName, @CredentialGuid,@IssuanceDate,@ExamDueDate, @RecentAdministrationDate,@RecentExamResult,@RecentExamType,@LKAEnrollmentStatus,@LKAEnrollmentDate,@CMPTestDate,@CMPTestResult,@CMPOnHold,@IsInCMP,@Pathway, @IsCosponsored

	WHILE @@FETCH_STATUS = 0   
	BEGIN   
		   set @cnt = @cnt + 1
		   set @tmpSQL = @tmpSQL + ''<tr>
				<td align="left">'' + @abimId + ''</td>
				<td align="left">'' + @certificationName + ''</td>
				<td align="left">'' + cast(@credentialGuid as varchar(300)) + ''</td>
				<td align="left">'' + @IsCosponsored + ''</td>
				<td align="left">'' + @IssuanceDate + ''</td>
				<td align="left">'' + @ExamDueDate + ''</td>
				<td align="left">'' + @RecentAdministrationDate + ''</td>
				<td align="left">'' + @RecentExamResult + ''</td>
				<td align="left">'' + @RecentExamType + ''</td>
				<td align="left">'' + @LKAEnrollmentStatus + ''</td>
				<td align="left">'' + @LKAEnrollmentDate + ''</td>
				<td align="left">'' + @CMPTestDate + ''</td>
				<td align="left">'' + @CMPTestResult + ''</td>
				<td align="left">'' + @CMPOnHold + ''</td>
				<td align="left">'' + @IsInCMP + ''</td>
				<td align="left">'' + @Pathway + ''</td>

			</tr>''

		   FETCH NEXT FROM db_cursor INTO @abimId, @certificationName, @CredentialGuid,@IssuanceDate,@ExamDueDate, @RecentAdministrationDate,@RecentExamResult,@RecentExamType,@LKAEnrollmentStatus,@LKAEnrollmentDate,@CMPTestDate,@CMPTestResult,@CMPOnHold,@IsInCMP,@Pathway, @IsCosponsored
	END   

	CLOSE db_cursor   
	DEALLOCATE db_cursor

	if @cnt > 0
	BEGIN
		DECLARE @body1 nvarchar(max)
		SET @body1 = 
		''<html>
		<head></head>
		<body style="background: #ffffff; color: #003399"> 
			<table border="1" align="center"  cellpadding="3"  width="500">
				<tr>
					<td align="center" colspan="16" style="font-size:24;color:#003399"><b>Issuances Created Yesterday (Total Count: '' + cast(@cnt as varchar(10)) + '')</b></td>
				</tr>
				<tr>
					<td align="center">ABIM ID</td>
					<td align="center">Discipline Name</td>
					<td align="center">Credential Guid</td>
					<td align="center">Is Cosponsored?</td>
					<td align="center">Issuance Date</td>
					<td align="center">Exam Due Date</td>
					<td align="center">Recent Admin Date</td>
					<td align="center">Recent Exam Result</td>
					<td align="center">Recent Exam Type</td>
					<td align="center">LKA Enrollment Status</td>
					<td align="center">LKA Enrollment Date</td>
					<td align="center">Recent CMP Test Date</td>
					<td align="center">Recent CMP Result</td>
					<td align="center">Recent CMP On Hold?</td>
					<td align="center">In CMP?</td>
					<td align="center">Pathway</td>
				</tr>
				'' + @tmpSQL + ''
			</table>
		</body>
		</html>''

		DECLARE @subject1 nvarchar(70)
		SET @subject1 = @Subject
		EXEC msdb.dbo.sp_send_dbmail @recipients=@EmailTo,
			@subject = @subject1,
			@body = @body1,
			@body_format = ''HTML'',
			@profile_name = ''ABIM Mail'',
			@importance = ''High'';
	END

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
    @description = N'This Job will send email for Issuance created yesterday.',
    @category_name = N'CertificationJobs',
    --@owner_login_name = N'ESA',
    @job_id = @jobId OUTPUT;

IF (@@ERROR <> 0 OR @ReturnCode <> 0)
    GOTO QuitWithRollback;

EXEC @ReturnCode = msdb.dbo.sp_add_jobstep
    @job_id = @jobId,
    @step_name = N'Send issuances created yesterday',
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
    @name = 'Daily at 9am',
    @enabled=1, 
	@freq_type=4, 
	@freq_interval=1, 
	@freq_subday_type=1, 
	@freq_subday_interval=0, 
	@freq_relative_interval=0, 
	@freq_recurrence_factor=0, 
	@active_start_date=20230206, 
	@active_end_date=99991231, 
	@active_start_time=90000, 
	@active_end_time=235959;

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
