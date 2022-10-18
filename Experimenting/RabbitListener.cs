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
        public void Run([RabbitMQTrigger("myqueue", ConnectionStringSetting = "amqp://guest:guest@localhost:15672/")] string myQueueItem)
        {

            // amqps://gmtxfped:q9q2CrgirKMatFilFe-qlM4cMzGa4hii@beaver.rmq.cloudamqp.com/gmtxfped
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
