using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private const string secureApiEndpoint = "http://127.0.0.1:1717/api";
        private readonly IConfiguration _config;

        public PaymentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public string CreatePayment()
        {
            string apiKey = _config.GetValue<string>("api:apikey");

            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(secureApiEndpoint);
            apiRequest.Headers["x-api-key"] = apiKey;

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            StreamReader streamResponse = new StreamReader(apiResponse.GetResponseStream());

            string stringResponse = streamResponse.ReadToEnd();
            return stringResponse;
        }
    }
}