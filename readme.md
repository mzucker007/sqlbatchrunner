# SqlBatchRunner

This is a simple utility to apply sql batches to a database and track which batches have been already applied.

SQL Server only.

## Usage

Save *.sql files to a folder.

Be sure to include a GUID in the sql file to uniquely identify the sql batch.
```
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
```

Execute as follows:
```
SqlBatchRunner.exe [path-to-sql-files]
```

Optional. If you copy SqlBatchRunner.exe and SqlBatchRunner.exe.config into the folder with the sql files, it will execute the sql files in that folder.





