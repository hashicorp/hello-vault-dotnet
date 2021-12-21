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
        private const string secureApiEndpoint = "http://secure-service/api";

        VaultWrapper _vault;

        public PaymentsController( VaultWrapper vault )
        {
            _vault = vault;
        }

        [HttpPost]
        public string CreatePayment()
        {
            string apiKey = _vault.GetSecretApiKey();

            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(secureApiEndpoint);
            apiRequest.Headers["x-api-key"] = apiKey;

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            StreamReader streamResponse = new StreamReader(apiResponse.GetResponseStream());
            string stringResponse = streamResponse.ReadToEnd();
            return stringResponse;
        }
    }
}
