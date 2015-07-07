# SqlBatchRunner

This utility recursively scans a directory to find *.sql files, applies them to a database and tracks which ones have been already applied.

SQL Server only.

## Usage

A control table named dbo.SqlBatchControl will be created in the database if it does not yet exist.

Save *.sql files to a folder.

Create a file named config.json in the following format.

{ 
	"ConnectionString":"connect",
	"ConnectionStringXmlSearch":[
		{
			"Attribute":"value",
			"NodePath":"parameters\/setParameter[@name=\"name-as-specified-in-parameters-file\"]"
		},
		{
			"Attribute":"connectionString",
			"NodePath":"connectionStrings\/add[@name=\"name-as-specified-in-web.config\"]"
		}
	]
}

If ConnectionString isn't specified, SqlBatchRunner will expect a second parameter to specify an XML file in the format of a Web.config, or in the format of a WebDeploy xml parameters file. 

To run the tool, specify the root directory of your sql scripts and the XML file containing your connection strings.

Execute as follows:
```
SqlBatchRunner.exe [path-to-directory-containing-sql-files] [path-to-web.config]
```

Example:

SqlBatchRunner.exe c:\projects\mvcproject\dbscripts c:\projects\mvcproject\web.release.config

SqlBatchRunner accepts an optional third parameter, -m, which enables manual mode. Before executing any SQL script, SqlBatchRunner will prompt you to run the script. Whether or not you run the script, you’ll be prompted to update the tracking table. Manual mode is designed for getting an existing database updated when first implementing this tool.