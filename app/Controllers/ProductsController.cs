using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using app.Vault;
using app.Models;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private VaultWrapper _vault;

        public ProductsController(VaultWrapper vault, IConfiguration configuration)
        {
            _configuration = configuration;
            _vault = vault;
        }

        // Get /api/Products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            string connectionString = _vault.GetDbConnectionString();
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
            List<Product> someProducts = new List<Product>(){};
            return someProducts;
        }
    }
}
