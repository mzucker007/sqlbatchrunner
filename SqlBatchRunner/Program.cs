﻿using System;
using System.Configuration;

namespace SqlBatchRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            var result = 0;

            if (args.Length == 1)
            {
                /**
                 * This will run *.sql files found in directory passed in via command line.
                 */

                //  create SqlBatchControl table if it doesn't exist
                //SqlRunner.createControlTable();

                //  execute sql found in target folder
                Console.WriteLine("Executing SQL found in {0}", args[0]);
                //SqlRunner.Run(args[0]);
            }
            else if (args.Length == 0)
            {
                //  If no command line arguments, first check the directory in the App.Config.
                //  Otherwise, just execute the *.sql found in the current directory.
                String directoryName = ConfigurationManager.AppSettings["DirectoryName"] != "" ? ConfigurationManager.AppSettings["DirectoryName"] : Environment.CurrentDirectory;
                String connectionString = ConfigurationManager.AppSettings["ConnectionString"];

                //  create SqlBatchControl table if it doesn't exist
                var runner = new SqlRunner(connectionString);

                //  execute sql found folder local to execution context
                Console.WriteLine("Executing SQL found in {0}", directoryName);
                runner.Run(directoryName);
            }
            else if (args.Length == 2)
            {
                var directoryPath = args[0];
                var xmlFile = args[1];

                var c = new ConfigScanner(xmlFile);

                c.ProcessDirectory(directoryPath);
            }
            else
            {
                Console.WriteLine("Expecting only one argument, the path to sql files.");
            }

            return result;
        }
    }
}
