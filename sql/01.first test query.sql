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
