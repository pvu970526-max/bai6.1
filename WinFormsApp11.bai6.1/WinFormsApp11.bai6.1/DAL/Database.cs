using Microsoft.Data.SqlClient;

namespace WinFormsApp11.bai6._1.DAL;

public static class Database
{
    public const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SalesDB;Integrated Security=True;TrustServerCertificate=True;";

    public static SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
