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
        private static string[] CRUDStarters = new string[] { "SELECT", "INSERT", "UPDATE", "DELETE" };

        private static string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

        public static int Run(string folderPath)
        {
            var folderInfo = new DirectoryInfo(folderPath);
            FileInfo[] sqlFiles = folderInfo.GetFiles("*.sql");
            string[] filesPreviouslyRun = readControlTable();
            foreach (var fileojb in sqlFiles)
            {
                if (Array.IndexOf(filesPreviouslyRun, fileojb.Name) > -1)
                {
                    Console.WriteLine("Previously executed: {0}", fileojb.Name);
                }
                else
                {
                    runSql(fileojb);
                }
            }
            return 0;
        }

        static int runSql(FileInfo fileojb)
        {
            Console.WriteLine("Running: {0}", fileojb.Name);
            // Covers funky characters
            var fileContent = File.ReadAllText(fileojb.FullName, Encoding.UTF7);

            var sqlqueries = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None );

            var con = new SqlConnection(connectionString);
            con.Open();
            SqlTransaction trans = con.BeginTransaction();

            try
            {
                var cmd = new SqlCommand("query", con, trans); 
                string currentQuery = string.Empty;
                foreach (var str in sqlqueries)
                {
                    if (str.Trim().ToUpper().CompareTo("GO") == 0 && currentQuery.Length > 0)
                    {
                        cmd.CommandText = currentQuery;
                        cmd.ExecuteNonQuery();
                        currentQuery = string.Empty;
                    }
                    else
                        currentQuery += str + Environment.NewLine;
                }

                if (currentQuery.Length > 0)
                {
                    cmd.CommandText = currentQuery;
                    cmd.ExecuteNonQuery();
                    currentQuery = string.Empty;
                }
                //  log the filename in table
                cmd.CommandText = string.Format("insert SqlBatchControl (filename) values ('{0}')", fileojb.Name);
                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                trans.Rollback();
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
	                                    id int identity(1,1) primary key, 
	                                    filename varchar(max) not null, 
                                        insert_date datetime not null default (getutcdate()) )", con);

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

        static string[] readControlTable()
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
