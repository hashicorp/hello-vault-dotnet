using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace WebService.Database
{
    public class DatabaseClient : IDisposable
    {
        private SqlConnection _connection;

        public DatabaseClient(DatabaseSettings settings, string username, string password)
        {
            _connection = new SqlConnection(BuildConnectionString(settings, username, password));
            _connection.Open();
        }

        public IEnumerable<Product> GetProducts()
        {
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
