using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private VaultWrapper _vault;

        public ProductsController(IConfiguration configuration, ILogger logger, VaultWrapper vault)
        {
            _configuration = configuration;
            _logger = logger;
            _vault = vault;
        }

        // Get /api/Products
        [HttpGet]
        public string GetProducts()
        {
            _logger.LogInformation("Retrieving database connection string from Vault");
            string connectionString = _vault.GetDbConnectionString();
            _logger.LogInformation("Successfully retrieved database connection string from Vault");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT name FROM products";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }
                
            };
            
            // Serialize to IEnumerable

            return string.Empty;
        }
    }
}
