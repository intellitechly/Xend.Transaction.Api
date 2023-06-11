using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System;
using Xend.Transaction.Api.Contracts;

namespace Xend.Transaction.Api.Services
{
    public class EventBus : IEventBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "eventsExchange";

        public EventBus(string connectionString)
        {
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic);
        }

        public void Publish<TEvent>(TEvent @event)
        {
            var routingKey = GetRoutingKey<TEvent>();
            var properties = _channel.CreateBasicProperties();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

            _channel.BasicPublish(ExchangeName, routingKey, properties, body);
        }

        public void Subscribe<TEvent>(IEventHandler<TEvent> eventHandler)
        {
            var queueName = typeof(TEvent).FullName;
            var routingKey = GetRoutingKey<TEvent>();

            _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(queueName, ExchangeName, routingKey, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                var @event = JsonConvert.DeserializeObject<TEvent>(message);
                eventHandler.Handle(@event);
            };

            _channel.BasicConsume(queueName, autoAck: true, consumer: consumer);
        }

        private static string GetRoutingKey<TEvent>()
        {
            // Provide a logic to determine the routing key based on the event type
            // This can be based on event properties or conventions specific to your application
            // For simplicity, this example uses the event type name as the routing key
            return typeof(TEvent).Name;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

}
