using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Requestor
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req, FunctionContext Context)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            var (operationId, parentId) = GetOperationIdAndParentId(Context);

            var activity = new Activity("Parent");
            var guid = "00-"+parentId+"-15f670e22f6dbece-00";
            activity.SetParentId(guid);
            activity.Start();
            


            


            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
       

            var one = parentId;

           
            var t = DoBusiness();
            activity.Stop();
            _logger.LogInformation("Requestor "+one);
            response.WriteString("Welcome to Azure Functions subbux!");

            return response;
        }

        public string DoBusiness()
        {


            Console.WriteLine("Hello, World!");
            //var activity = new Activity("Main");
            //activity.Start();
            var url = "http://localhost:4091/api/Function1";
            var person = new Person("John Doe", "gardener");

            //activity.Id = "00-b68901b63f1436a15ad24e12f47745af-a442624e8ebd1d3f-00";
            //activity.Context.TraceId = "1c227caf77df71af761a651bfe4135a6";
            var json = JsonConvert.SerializeObject(person);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            var response = client.PostAsync(url, data).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            
            //activity.Stop();
            return result;


        }









        private (string, string) GetOperationIdAndParentId(FunctionContext executionContext)
        {
           // executionContext.TraceContext.TraceParent[0] = "subbu";
            var traceParent = executionContext.TraceContext.TraceParent;
            if (string.IsNullOrEmpty(traceParent))
            {
                return ("", "");

            }
            var partofTraceParent = traceParent.Split('-');
            if (partofTraceParent.Length != 4)
            {
                return ("", "");

            }


            return (partofTraceParent[0], partofTraceParent[1]);

        }




    }
}
