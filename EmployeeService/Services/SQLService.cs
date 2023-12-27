using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EmployeeService.Services
{

    public static class SQLService
    {
        private static readonly string connectionString = "Data Source=(local);Initial Catalog=Emploee;User ID=sa;Password=pass@word1;";


        public static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static void ExecuteNonQuery(SqlConnection connection, string query)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Query completed successfully.");
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during query execution: " + ex.Message);
                throw new Exception("Error during query execution: " + ex.Message);
            }

        }

        public static DataTable ExecuteReader(SqlConnection connection, string query)
        {

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();

                var dt = new DataTable();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
                Console.WriteLine("Query completed successfully.");
                return dt;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error during query execution: " + ex.Message);
                throw new Exception("Error during query execution: " + ex.Message);
            }
        }

    }
}