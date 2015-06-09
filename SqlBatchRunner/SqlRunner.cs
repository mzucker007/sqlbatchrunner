using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace SqlBatchRunner
{
    static class SqlRunner
    {
        public static int Run(String folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            FileInfo[] sqlFiles = folderInfo.GetFiles("*.sql");
            foreach (var fileojb in sqlFiles)
            {
                runSql(fileojb.FullName);
            }
            return 0;
        }

        static int runSql(String fileName) {
            Console.WriteLine("Running: {0}", fileName);
            var fileContent = File.ReadAllText(fileName);
            fileContent = fileContent.Replace("GO", "go");
            var sqlqueries = fileContent.Split(new[] { "go" }, StringSplitOptions.RemoveEmptyEntries);

            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
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
    }
}
