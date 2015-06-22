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