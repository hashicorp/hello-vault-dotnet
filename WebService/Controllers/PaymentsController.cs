using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger _logger;
        private VaultWrapper _vault;
        private readonly string _secureServiceAddress;

        public PaymentsController(ILogger<PaymentsController> logger, VaultWrapper vault, string secureServiceAddress)
        {
            _logger = logger;
            _vault = vault;
            _secureServiceAddress = secureServiceAddress;
        }

        // POST /api/Payments
        [HttpPost]
        public string CreatePayment()
        {
            // fetch the secret api key from Vault
            _logger.LogInformation("retrieving api key from Vault: started");

            string apiKey = _vault.GetSecretApiKey();

            _logger.LogInformation("retrieving api key from Vault: done");

            HttpWebRequest request = WebRequest.Create(_secureServiceAddress) as HttpWebRequest;

            // add the secret api key to the request header
            request.Headers[ "x-api-key" ] = apiKey;

            // forward the response back to the caller
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader( response.GetResponseStream()))
                {
                    string stringResponse = sr.ReadToEnd();
                    return stringResponse;
                }
            }
        }
    }
}
