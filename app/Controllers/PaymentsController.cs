using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using app.Vault;

namespace app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private const string secureServiceEndpoint = "http://secure-service/api";

        private VaultWrapper _vault;

        public PaymentsController( VaultWrapper vault )
        {
            _vault = vault;
        }

        // POST /api/Payments
        [HttpPost]
        public string CreatePayment()
        {
            // fetch the secret api key from Vault
            string apiKey = _vault.GetSecretApiKey();

            HttpWebRequest request = WebRequest.Create( secureServiceEndpoint ) as HttpWebRequest;

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
