-- 
--	SQL scripts will be run if no matching GUID is found in SQLBatchControl table
--
--	select newid()
--	
declare @sentinelvalue uniqueidentifier
set @sentinelvalue = '8C5C9FBA-571F-496F-95B6-44EE142E7475'

if exists(select * from [INFORMATION_SCHEMA].[TABLES] where TABLE_NAME = 'SqlBatchControl')
	begin
	if exists (select id from SqlBatchControl where id = @sentinelvalue)
		return 
	end

insert SqlBatchControl (id)
values (@sentinelvalue)
/* =====================================================================================================
	Include the above in every file. Make sure the @sentinelvalue is a new GUID unique to each sql file.
   ===================================================================================================== */

--
--	Begin: sql script
--
insert Test01 (descrip)
select top 2 TABLE_NAME from [INFORMATION_SCHEMA].[TABLES]
GO

if object_id('testproc') is not null
	drop procedure testproc
go
create procedure testproc as
--	exec testproc
select * from Test01
go