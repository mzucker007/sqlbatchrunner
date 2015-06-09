-- 
--	SQL scripts will be run if no matching GUID is found in SQLBatchControl table
--
--	select newid()
--	
declare @sentinelvalue uniqueidentifier
set @sentinelvalue = '6C31E43A-992D-4516-AFE7-B1B8A5722809'

if exists(select * from [INFORMATION_SCHEMA].[TABLES] where TABLE_NAME = 'SqlBatchControl')
	begin
	if exists (select id from SqlBatchControl where id = @sentinelvalue)
		return 
	end

/* =====================================================================================================
	Include the above in every file. Make sure the @sentinelvalue is a new GUID unique to each sql file.
   ===================================================================================================== */

--
--	Begin: sql script
--

--	create table
if object_id('SqlBatchControl') is not null
	drop table SqlBatchControl
create table SqlBatchControl (
	id uniqueidentifier primary key,
	insert_date datetime default (getutcdate())
)
GO

-- need to insert this after table created
declare @sentinelvalue uniqueidentifier
set @sentinelvalue = '6C31E43A-992D-4516-AFE7-B1B8A5722809'

if exists (select id from SqlBatchControl where id = @sentinelvalue)
begin
	return 
end
--	insert
insert SqlBatchControl (id)
values ('6C31E43A-992D-4516-AFE7-B1B8A5722809')




