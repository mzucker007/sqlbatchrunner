using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Collections;

namespace SqlBatchRunner
{
    static class SqlRunner
    {
        private static String connectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public static int Run(String folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            FileInfo[] sqlFiles = folderInfo.GetFiles("*.sql");
            String[] filesPreviouslyRun = readControlTable();
            foreach (var fileojb in sqlFiles)
            {
                if (Array.IndexOf(filesPreviouslyRun, fileojb.FullName) > -1)
                {
                    Console.WriteLine("Previously executed: {0}", fileojb.FullName);
                }
                else
                {
                    runSql(fileojb.FullName);
                }
            }
            return 0;
        }

        static int runSql(String fileName) {
            Console.WriteLine("Running: {0}", fileName);
            var fileContent = File.ReadAllText(fileName);
            fileContent = fileContent.Replace("GO", "go");
            var sqlqueries = fileContent.Split(new[] { "go" }, StringSplitOptions.RemoveEmptyEntries);

            //var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
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
                cmd.CommandText = String.Format("insert SqlBatchControl (filename) values ('{0}')", fileName);
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

        static String[] readControlTable()
        {
            List<String> filesPreviouslyRun = new List<string>();
            var con = new SqlConnection(connectionString);
            var cmd = new SqlCommand("select filename from SqlBatchControl", con);

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    filesPreviouslyRun.Add(reader["fileName"].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return filesPreviouslyRun.ToArray();
        }
    }
}
