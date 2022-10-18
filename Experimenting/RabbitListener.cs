using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Experimenting
{
    public class RabbitListener
    {
        private readonly ILogger _logger;

        public RabbitListener(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RabbitListener>();
        }

        [Function("RabbitListener")]
        public void Run([RabbitMQTrigger("myqueue", ConnectionStringSetting = "amqp://guest:guest@localhost:15672/")] string myQueueItem, FunctionContext Context)
        {
            var (operationId, parentId) = GetOperationIdAndParentId(Context);
            _logger.LogInformation($"The operation Id is : {parentId}");
            // amqps://gmtxfped:q9q2CrgirKMatFilFe-qlM4cMzGa4hii@beaver.rmq.cloudamqp.com/gmtxfped
           
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
