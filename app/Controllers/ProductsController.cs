using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using app.Vault;
using app.Models;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private VaultWrapper _vault;

        public ProductsController( VaultWrapper vault )
        {
            _vault = vault;
        }

        // POST /api/Payments
        [HttpGet]
        public IEnumerable<Product> GetProducts()
        {
            // Get 
            List<Product> someProducts = new List<Product>();

            return someProducts;
        }
    }
}
