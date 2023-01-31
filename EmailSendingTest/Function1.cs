using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using System.Net;
using System.Text;

namespace EmailSendingTest
{
    public static class Function1
    {
        [FunctionName("SendEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = "Failed";

            HttpClient httpClient = new HttpClient();
            try
            {
                string requestStr = Environment.GetEnvironmentVariable("DuoCircleConnectionString");
                var content = new StringContent("{\r\n  \r\n    \"sandboxMode\": false,\r\n    \"messages\": [\r\n      {\r\n        \"sender\": {\r\n          \"email\": \"rtlesf_mobility@sertest.shell.com\",\r\n          \"name\": \"Emerald Admin\"\r\n        },\r\n        \"from\": {\r\n          \"email\": \"rtlesf_mobility@sertest.shell.com\",\r\n          \"name\": \"Emerald Admin\"\r\n        },\r\n        \"to\": [\r\n          {\r\n            \"email\": \"paul.ponnudurai@cgi.com\",\r\n            \"name\": \"Paul\"\r\n          }\r\n        ],\r\n        \"subject\": \"Test\",\r\n        \"text\": \"Hello.Welcome\"\r\n      }\r\n    ]\r\n  }\r\n\r\n\r\n");
                httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                //This is the key section you were missing
                //Environment.GetEnvironmentVariable("DuoCircleConnectionString");
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("DuoCircleCredentials"));
               httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + System.Convert.ToBase64String(plainTextBytes));


                var task = httpClient.PostAsJsonAsync(requestStr, content);
                 responseMessage = await task.Result.Content.ReadAsStringAsync();

                log.LogInformation("responseMessage"+ responseMessage);
            }
            catch (Exception ex)
            {
                log.LogInformation("Exception" + ex);
                var ee = ex; ;
            }
           
            return new OkObjectResult(responseMessage);
        }
    }
}
