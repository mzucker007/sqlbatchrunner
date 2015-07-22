using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace SqlBatchRunner
{
    class SqlRunner
    {
        private String connectionString;

        private bool isUnattendedModeEnabled;

        public SqlRunner(String connectionString)
        {
            this.connectionString = connectionString;
            this.isUnattendedModeEnabled = true;
        }

        public void EnableManualMode()
        {
            isUnattendedModeEnabled = false;
        }

        public void Run(String folderPath)
        {
            DataTable filesPreviouslyRun = readControlTable();

            if (filesPreviouslyRun != null)
            {
                var folderInfo = new DirectoryInfo(folderPath);
                var sqlFiles = folderInfo.EnumerateFiles("*.sql").OrderBy(f => f.Name);

                foreach (var fileojb in sqlFiles)
                {
                    var fileContent = File.ReadAllText(fileojb.FullName);

                    //  calculate checksum of file contents
                    var cksum = createCkSum(fileContent);

                    Console.Write("{0} : {1}", fileojb.Name, cksum);

                    if (filesPreviouslyRun.AsEnumerable().Any(row => cksum == row.Field<String>("CheckSum")))
                    {
                        Console.WriteLine(" - Previously executed");
                    }
                    else
                    {
                        Console.WriteLine(" - Running...");
                        runSql(fileojb.Name, fileContent, cksum);
                    }
                }
            }
            return;
        }

        void runSql(String fileName, String fileContent, String cksum)
        {
            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand("query", con);

            try
            {
                con.Open();

                if (isUnattendedModeEnabled || ConfirmToContinue(" Run batch: " + fileName))
                {
                    foreach (var query in Queries(fileContent))
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                }

                //  log the filename in table
                if (isUnattendedModeEnabled || ConfirmToContinue(" Update tracking table"))
                {
                    cmd.CommandText = String.Format("insert into [dbo].[SqlBatchControl] (OriginalFileName, CheckSum, Connection) values ('{0}', '{1}', '{2}')", fileName, cksum, con.Database);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                Console.WriteLine(cmd.CommandText);
                throw;
            }
            finally
            {
                con.Close();
            }
            return;
        }

        private bool ConfirmToContinue(string v)
        {
            bool check = false;
            ConsoleKeyInfo ck;
            do
            {
                Console.Write("\r{0}? (y/n)  \b", v);
                ck = Console.ReadKey();
                check = !((ck.Key == ConsoleKey.Y) || (ck.Key == ConsoleKey.N));
            } while (check);

            Console.WriteLine();

            return ck.Key == ConsoleKey.Y;
        }

        public bool createControlTable()
        {
            bool result;
            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand(@"if object_id(N'dbo.SqlBatchControl') is null 
                                        create table dbo.SqlBatchControl ( 
	                                    OriginalFileName varchar(max) not null, 
                                        CheckSum varchar(max) not null,
                                        Connection varchar(max) not null,
                                        UtcDateRun datetime not null default (getutcdate()) )", con);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                result = true;
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        DataTable readControlTable()
        {
            DataTable fileDataTable = null;

            if (createControlTable())
            {
                var con = new SqlConnection(connectionString);
                var cmd = new SqlCommand("select OriginalFileName, CheckSum from SqlBatchControl", con);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    fileDataTable = new DataTable();
                    fileDataTable.Load(reader);
                }
                finally
                {
                    con.Close();
                }
            }
            return fileDataTable;
        }

        static String createCkSum(String filetext)
        {
            byte[] filetextBytes = Encoding.UTF8.GetBytes(filetext);

            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(filetextBytes)).Replace("-", string.Empty);
            }
        }

        IEnumerable<string> Queries(string queryfile)
        {
            List<string> queries = new List<string>();

            using (var reader = new StringReader(queryfile))
            {
                StringBuilder qb = new StringBuilder();
                string line = null;

                do
                {
                    line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Trim().Equals("go", StringComparison.OrdinalIgnoreCase))
                        {
                            if (qb.Length > 0)
                            {
                                queries.Add(qb.ToString());
                                qb.Clear();
                            }
                        }
                        else
                            qb.AppendLine(line);
                    }
                } while (line != null);

                if (qb.Length > 0)
                    queries.Add(qb.ToString());
            }

            return queries;
        }
    }
}
