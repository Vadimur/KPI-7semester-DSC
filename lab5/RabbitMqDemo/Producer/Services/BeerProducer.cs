using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared;
using System;
using System.Text;

namespace Producer.Services
{
    public class BeerProducer : IDisposable
    {
        private readonly ILogger<BeerProducer> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public  BeerProducer(ILogger<BeerProducer> logger)
        {
            _logger = logger;

            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: SharedConstants.Queue,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false);
        }

        public void Produce<T>(T @object)
        {
            var payload = JsonConvert.SerializeObject(@object);
            var body = Encoding.UTF8.GetBytes(payload);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true; // make messages persistent

            _channel.BasicPublish(exchange: SharedConstants.Exchange,
                                  routingKey: SharedConstants.RoutingKey,
                                  basicProperties: properties,
                                  body: body);

            _logger.LogInformation($"Beer was passed: {payload}");
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
