using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebService.Vault;

namespace WebService.Controllers
{
    public class PaymentsControllerSettings
    {
        public string SecureServiceEndpoint { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ILogger _logger;
        private VaultWrapper _vault;
        private readonly string _secureServiceEndpoint;

        public PaymentsController(ILogger<PaymentsController> logger, VaultWrapper vault, PaymentsControllerSettings settings)
        {
            _logger = logger;
            _vault = vault;
            _secureServiceEndpoint = settings.SecureServiceEndpoint;
        }

        // POST /api/Payments
        [HttpPost]
        public string CreatePayment()
        {
            // fetch the secret api key from Vault
            _logger.LogInformation("retrieving api key from Vault: started");

            string apiKey = _vault.GetSecretApiKey();

            _logger.LogInformation("retrieving api key from Vault: done");

            HttpWebRequest request = WebRequest.Create(_secureServiceEndpoint) as HttpWebRequest;

            // add the secret api key to the request header
            request.Headers[ "x-api-key" ] = apiKey;

            // forward the response back to the caller
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader sr = new StreamReader( response.GetResponseStream()))
                {
                    _logger.LogInformation($"sent request to { _secureServiceEndpoint } with api key and received response");

                    string stringResponse = sr.ReadToEnd();
                    return stringResponse;
                }
            }
        }
    }
}
