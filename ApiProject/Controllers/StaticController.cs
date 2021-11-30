using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaticController : ControllerBase
    {
        private const string secureApiEndpoint = "http://127.0.0.1:1717/api";

        [HttpGet]
        public string GetApiInfo()
        {
            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(secureApiEndpoint);
            apiRequest.Headers["x-api-key"] = "my-secret-key";

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            StreamReader streamResponse = new StreamReader(apiResponse.GetResponseStream());
            string stringResponse = streamResponse.ReadToEnd();
            return stringResponse;
        }
    }
}