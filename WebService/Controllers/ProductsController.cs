using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        private VaultWrapper _vault;

        public struct Product
        {
            public Product(string name)
            {
                Name = name;
            }
            public string Name { get; }
        }

        public ProductsController(ILogger<ProductsController> logger, VaultWrapper vault)
        {
            _logger = logger;
            _vault = vault;
        }

        // Get /api/Products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            _logger.LogInformation("Retrieving database connection string from Vault");
            string connectionString = _vault.GetDbConnectionString();
            _logger.LogInformation("Successfully retrieved database connection string from Vault");

            List<Product> products = new List<Product>();
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                String sql = "SELECT name FROM products";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            products.Add(new Product(reader.GetString(0)));
                        }
                    }
                }
            };
            
            return products;
        }
    }
}
