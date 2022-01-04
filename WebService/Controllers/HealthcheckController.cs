using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Route("[controller]")]
    public class HealthcheckController : ControllerBase
    {
        // GET /Healthcheck
        [HttpGet]
        public string GetHealthcheck()
        {
            return "OK";
        }
    }
}
