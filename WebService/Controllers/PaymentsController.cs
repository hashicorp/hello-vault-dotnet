using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using WebService.Vault;

namespace WebService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private VaultWrapper _vault;
        private readonly string _secureServiceAddress;

        public PaymentsController( VaultWrapper vault, string secureServiceAddress )
        {
            _vault = vault;
            _secureServiceAddress = secureServiceAddress;
        }

        // POST /api/Payments
        [HttpPost]
        public string CreatePayment()
        {
            // fetch the secret api key from Vault
            string apiKey = _vault.GetSecretApiKey();

            HttpWebRequest request = WebRequest.Create( _secureServiceAddress ) as HttpWebRequest;

            // add the secret api key to the request header
            request.Headers[ "x-api-key" ] = apiKey;

            // forward the response back to the caller
            using ( HttpWebResponse response = request.GetResponse() as HttpWebResponse )
            {
                using ( StreamReader sr = new StreamReader( response.GetResponseStream() ) )
                {
                    string stringResponse = sr.ReadToEnd();
                    return stringResponse;
                }
            }
        }
    }
}
