using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("[controller]")]
    public class HealthcheckController : ControllerBase
    {
        // GET /api/Products
        [HttpGet]
        public string GetHealthcheck()
        {
            return "OK";
        }
    }
}