using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Text;

namespace Consumer
{
    public class BeerConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private Action<Beer> _callback;

        public BeerConsumer()
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: SharedConstants.Queue,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false);
        }

        public void Consume(Action<Beer> callback)
        {
            _callback = callback;
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceive;

            _channel.BasicConsume(queue: SharedConstants.Queue,
                                 autoAck: false,
                                 consumer: consumer);            
        }

        private void OnReceive(object sender, BasicDeliverEventArgs args)
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Beer beer = JsonConvert.DeserializeObject<Beer>(message);
            _callback(beer);

            _channel.BasicAck(args.DeliveryTag, false);
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
