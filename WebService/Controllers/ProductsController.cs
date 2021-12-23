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
            public Product(int id, string name)
            {
                Id = id;
                Name = name;
            }
            public int Id { get; }
            public string Name { get; }
        }

        public ProductsController(ILogger<ProductsController> logger, VaultWrapper vault)
        {
            _logger = logger;
            _vault = vault;
        }

        // GET /api/Products
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
                string sql = "SELECT * FROM products";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            products.Add(new Product(reader.GetInt32(0), reader.GetString(1)));
                        }
                    }
                }
            };

            return products;
        }
    }
}
