using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SE.CBMS.GBP.Business
{
    public class ServiceBusSenderModel
    {
        readonly string AzureSBConnectionString;
        public ServiceBusSenderModel(string SBConnectionString)
        {
            this.AzureSBConnectionString = SBConnectionString;
        }
        public async Task<string> SendMessageAsync(string sbPayload, string QueueName)
        {
            string message;
            ServiceBusClient client;

            // the sender used to publish messages to the queue
            Azure.Messaging.ServiceBus.ServiceBusSender sender;

            // TODO: Replace the <NAMESPACE-CONNECTION-STRING> and <QUEUE-NAME> placeholders
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            client = new ServiceBusClient(this.AzureSBConnectionString, clientOptions);
            sender = client.CreateSender(QueueName);

            ServiceBusMessage msg = new ServiceBusMessage(sbPayload);

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessageAsync(msg);
                message = "Success";

            }
            catch
            {
                message = "Failure";
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                //await client.DisposeAsync();
            }
            return message;
        }
    }
}
