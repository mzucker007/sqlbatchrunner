# SqlBatchRunner

This is a simple utility to apply sql batches to a database and track which batches have been already applied.

SQL Server only.

## Usage

Save *.sql files to a folder.

Edit connection string in SqlBatchRunner.exe.config

Execute as follows:
```
SqlBatchRunner.exe [path-to-sql-files]
```

Optional. If you copy SqlBatchRunner.exe and SqlBatchRunner.exe.config into the folder with the sql files, it will execute the sql files in that folder.





