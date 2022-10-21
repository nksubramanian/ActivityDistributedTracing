using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;

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
            //MetaDataWrapper is the object being sent by the receiver here


            string traceparent="there is nothing";
            string correlationid = "there is nothing";
            string messageid = "there is nothing";
            try
            {
               

                if (Context.BindingContext.BindingData.TryGetValue("BasicProperties", out var basicProperties))
                {

                    var dynamicObject = JsonConvert.DeserializeObject<dynamic>(basicProperties.ToString())!;

                    messageid = dynamicObject.MessageId;
                    correlationid = dynamicObject.CorrelationId;
                    var headers = dynamicObject.Headers;
                    traceparent = headers.traceparent;
             


                }

            }
            catch(Exception ex)
            {

            }

          
             _logger.LogInformation($"The traceparent of RabbitMQ is : {traceparent}");
            _logger.LogInformation($"The correlation id is : {correlationid}");
            _logger.LogInformation($"The messageid is : {messageid}");


            var telemetryRequest = new RequestTelemetry();
            //telemetryRequest.Context.Operation.Id = "00-70a3d58f5f30dfde49d04dcd97bf4ff0-15f670e22f6dbece-00";
            var telemetryClient = new TelemetryClient();
            var currentOperation = telemetryClient.StartOperation(telemetryRequest);
            try
            {

                var activity = new Activity("Parent");
                var guid = traceparent;
                activity.SetParentId(guid);
                activity.Start();
                _logger.LogInformation($"The string received is : " + myQueueItem);
                var (operationId, parentId) = GetOperationIdAndParentId(Context);
                _logger.LogInformation($"The operation Id of RabbitMq Listener is : {parentId}");

                // amqps://gmtxfped:q9q2CrgirKMatFilFe-qlM4cMzGa4hii@beaver.rmq.cloudamqp.com/gmtxfped

                activity.Stop();

            }
            catch (Exception e)
            {
                telemetryClient.TrackException(e);
            }
            finally
            {
                telemetryClient.StopOperation(currentOperation);
            }


            
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
