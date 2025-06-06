
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using OrderAPI.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderAPI.Services
{
    public class AzureServiceBusPublisher : IAzureServiceBusPublisher
    {
        private readonly string _connectionString;
        private readonly string _queueName;

        public AzureServiceBusPublisher(IConfiguration configuration)
        {
            _connectionString = configuration["AzureServiceBus:ConnectionString"];
            _queueName = configuration["AzureServiceBus:QueueName"];
        }

        public async Task SendMessageAsync(Order order)
        {
            var client = new ServiceBusClient(_connectionString);
            var sender = client.CreateSender(_queueName);

            var json = JsonSerializer.Serialize(order);
            var message = new ServiceBusMessage(json);

            await sender.SendMessageAsync(message);
        }
    }
}
