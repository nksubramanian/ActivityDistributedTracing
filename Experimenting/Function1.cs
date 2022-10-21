using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Experimenting
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
            req.Headers.TryGetValues("traceparent", out var headerValue);

            
            var (operationId, parentId) = GetOperationIdAndParentId(Context);

            _logger.LogInformation("Traceparent at HTTP receiver is " + headerValue.FirstOrDefault());
            _logger.LogInformation("The operation Id of Http receiver "+ parentId);
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }


        private (string, string) GetOperationIdAndParentId(FunctionContext executionContext)
        {
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
