using System;
using System.Data.SqlClient;
using System.Configuration;

public static class DBAccess
{

    public static string ConnectionString
    {
        get
        {
            return ConfigurationManager.ConnectionStrings["RentalDB"].ConnectionString;
        }
    }


    public static SqlConnection GetConnection()
    {
        SqlConnection conn = new SqlConnection(ConnectionString);
        conn.Open(); 
        return conn;
    }
}