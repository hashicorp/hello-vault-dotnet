using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace WebService.Database
{
    public class DatabaseClient : IDisposable
    {
        private readonly ILogger _logger;
        private readonly SqlConnection _connection;

        public DatabaseClient(ILoggerFactory loggerFactory, DatabaseSettings settings, string username, string password)
        {
            _logger = loggerFactory.CreateLogger("Database");

            _logger.LogInformation($"connecting to '{ settings.DataSource }' database with username { username }: started");

            _connection = new SqlConnection(BuildConnectionString(settings, username, password));
            _connection.Open();

            _logger.LogInformation($"connecting to '{ settings.DataSource }' database with username { username }: done");
        }

        public IEnumerable<Product> GetProducts()
        {
            _logger.LogInformation("fetching products from database: started");

            const string query = "SELECT * FROM [example].[dbo].[products]";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        yield return new Product
                        {
                            Id   = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        };
                    }
                }
            }

            _logger.LogInformation("fetching products from database: done");
        }

        private string BuildConnectionString(DatabaseSettings settings, string username, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource      = settings.DataSource;
            builder.InitialCatalog  = settings.InitialCatalog;
            builder.ConnectTimeout  = settings.Timeout;
            builder.UserID          = username;
            builder.Password        = password;

            return builder.ConnectionString;
        }

        #region < implementation of IDisposable >

        private bool _disposed;

        ~DatabaseClient()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
