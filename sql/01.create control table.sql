--
--	Begin: sql script
--

--	create table
if object_id('SqlBatchControl') is not null
	drop table SqlBatchControl
create table SqlBatchControl (
	id int identity(1,1) primary key,
	filename varchar(max) not null,
	insert_date datetime not null default (getutcdate())
)
GO




