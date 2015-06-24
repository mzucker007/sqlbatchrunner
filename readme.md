# SqlBatchRunner

This is a simple utility to apply sql batches to a database and track which batches have been already applied.

SQL Server only.

## Usage

A control table named dbo.SqlBatchControl will be created in the database if it does not yet exist.

Save *.sql files to a folder.

Edit connection string in SqlBatchRunner.exe.config

Edit directory path for *.sql files in SqlBatchRunner.exe.config (optional). 

Execute as follows:
```
SqlBatchRunner.exe [path-to-sql-files]
```

Optional. If you copy SqlBatchRunner.exe and SqlBatchRunner.exe.config into the folder with the sql files, it will execute the sql files in that folder. Otherwise, you can pass the path-to-sql files as an argument in the command line, or set it in the config file.





