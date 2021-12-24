using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebService.DB;
using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger _logger;
        private VaultWrapper _vault;
        private Database _database;

        public ProductsController(ILogger<ProductsController> logger, VaultWrapper vault, Database database)
        {
            _logger   = logger;
            _vault    = vault;
            _database = database;
        }

        // GET /api/Products
        [HttpGet]
        public IEnumerable<DB.Product> GetProducts()
        {
            _logger.LogInformation("fetching products from database: started");

            IEnumerable<DB.Product> products = _database.GetProducts();

            _logger.LogInformation("fetching products from database: done");

            return products;
        }
    }
}
