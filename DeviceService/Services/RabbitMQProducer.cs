using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace DeviceService.Services
{
    public class RabbitMQProducer
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQProducer(string connectionUri)
        {
            var factory = new ConnectionFactory();

            factory.Uri = new Uri(connectionUri);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "device_create", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "device_update", durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: "device_delete", durable: true, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishMessage<T>(string queueName, T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
        }
    }
}
