using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requestor
{
    public static class RabbitMqHelper
    {
        public static void InjectTraceContextIntoBasicProperties(
IBasicProperties props, string key, string value)
        {
            if (props.Headers == null)
            {
                props.Headers = new Dictionary<string, object>();
            }

            props.Headers[key] = value;
        }

        public static void AddMessagingTags(Activity activity, string queueName)
        {
            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.destination", "worker");
            activity?.SetTag("messaging.rabbitmq.routing_key", queueName);
        }

    }
}
