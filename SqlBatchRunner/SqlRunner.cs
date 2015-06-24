using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;

namespace SqlBatchRunner
{
    static class SqlRunner
    {
        private static String connectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public static int Run(String folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            FileInfo[] sqlFiles = folderInfo.GetFiles("*.sql");
            sqlFiles.OrderBy(f => f.Name);
            DataTable filesPreviouslyRun = readControlTable();

            foreach (var fileojb in sqlFiles)
            {
                var fileContent = File.ReadAllText(fileojb.FullName);

                //  calculate checksum of file contents
                var cksum = createCkSum(fileContent);

                if (filesPreviouslyRun.AsEnumerable().Any(row => cksum == row.Field<String>("CheckSum")))
                {
                    Console.WriteLine("Previously executed: {0} with checksum: {1}", fileojb.Name, cksum);
                }
                else
                {
                    runSql(fileojb.Name, fileContent, cksum);
                }
            }
            return 0;
        }

        static int runSql(String fileName, String fileContent, String cksum)
        {
            Console.WriteLine("Running: {0}", fileName);
            fileContent = fileContent.Replace("GO", "go").Replace("Go", "go");
            var sqlqueries = fileContent.Split(new[] { "go" }, StringSplitOptions.RemoveEmptyEntries);

            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand("query", con);

            try
            {
                con.Open();
                foreach (var query in sqlqueries)
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }

                //  log the filename in table
                cmd.CommandText = String.Format("insert SqlBatchControl (OriginalFileName, CheckSum, Connection) values ('{0}', '{1}', '{2}')", fileName, cksum, con.Database);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return 0;
        }

        public static bool createControlTable()
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
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        static DataTable readControlTable()
        {
            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand("select OriginalFileName, CheckSum from SqlBatchControl", con);
            var fileDataTable = new DataTable();

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                fileDataTable.Load(reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
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
    }
}
