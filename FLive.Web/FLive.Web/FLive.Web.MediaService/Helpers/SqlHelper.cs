using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FLive.Web.MediaService.Helpers
{
    public static class SqlHelper
    {
        public static DataTable ExecuteStatement(string query)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["FliveDBConnection"].ConnectionString;
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            DataTable dataTable = new DataTable();

            try
            {
                connection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection);
                sqlDataAdapter.SelectCommand.CommandTimeout = 500;
                sqlDataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public static int ExecuteUpdate(string query)
        {
           //  string connectionString = ConfigurationManager.ConnectionStrings["FliveDBConnection"].ToString();
           string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            int recordCount = 0;
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                recordCount = cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return recordCount;
        }
    }
}