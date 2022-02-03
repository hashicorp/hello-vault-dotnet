using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebService.Database;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseClient _database;

        public ProductsController(DatabaseClient database)
        {
            _database = database;
        }

        // GET /api/Products
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            return _database.GetProducts();
        }
    }
}
