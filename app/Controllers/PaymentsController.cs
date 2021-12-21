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

        VaultWrapper _vault;

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

            HttpWebRequest request = ( HttpWebRequest ) WebRequest.Create( secureServiceEndpoint );

            // add the secret api key to the request header
            request.Headers[ "x-api-key" ] = apiKey;

            // forward the response back to the caller
            using ( HttpWebResponse response = ( HttpWebResponse ) request.GetResponse() )
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
