-- 
--	SQL scripts will be run if no matching GUID is found in SQLBatchControl table
--
--	select newid()
--	
declare @sentinelvalue uniqueidentifier
set @sentinelvalue = '0470CA97-7DD8-48BC-A6A2-8312D62220C5'

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
if object_id('Test01') is not null
	drop table Test01
create table Test01 (
	id int identity(1,1) primary key,
	descrip varchar(100) null,
	insert_date datetime default(getutcdate())
	)
