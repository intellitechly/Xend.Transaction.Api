using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Xend.Transaction.Api.Contracts;

namespace Xend.Transaction.Api.Services
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "transactionsExchange";

        public MessageBroker(string connectionString)
        {
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
        }

        public void Publish<TMessage>(TMessage message)
        {
            var routingKey = GetRoutingKey<TMessage>();
            var properties = _channel.CreateBasicProperties();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.BasicPublish(ExchangeName, routingKey, properties, body);
        }

        private static string GetRoutingKey<TMessage>()
        {
            // Provide a logic to determine the routing key based on the message type
            // This can be based on message properties or conventions specific to your application
            // For simplicity, this example uses the message type name as the routing key
            return typeof(TMessage).Name;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

}
