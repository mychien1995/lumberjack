using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Lumberjack.Server.Services
{
    public interface IDbConnectionFactory
    {
        SqlConnection GetConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connString = configuration.GetConnectionString("Default");
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connString);
            if (connection.State != ConnectionState.Open) connection.Open();
            return connection;
        }
    }
}
