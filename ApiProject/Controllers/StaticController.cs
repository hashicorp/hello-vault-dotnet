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
    public class ProjectsController : ControllerBase
    {
        public const string endpoint = "http://127.0.0.1:1717/api";
        [HttpGet]
        public string GetAPIInfo()
        {
            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(endpoint);
            apiRequest.Headers["x-api-key"] = "my-secret-key";

            HttpWebResponse apiResponse = (HttpWebResponse)apiRequest.GetResponse();

            StreamReader streamResponse = new StreamReader(apiResponse.GetResponseStream());
            string stringResponse = streamResponse.ReadToEnd();
            return stringResponse;
        }
    }
}