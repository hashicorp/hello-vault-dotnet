using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebService.Database;
using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly VaultWrapper _vault;
        private readonly DatabaseClient _database;

        public ProductsController(ILogger<ProductsController> logger, VaultWrapper vault, DatabaseClient database)
        {
            _logger   = logger;
            _vault    = vault;
            _database = database;
        }

        // GET /api/Products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            _logger.LogInformation("fetching products from database: started");

            IEnumerable<Product> products = _database.GetProducts();

            _logger.LogInformation("fetching products from database: done");

            return products;
        }
    }
}
