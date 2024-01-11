using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using MonitoringComService.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MonitoringComService.Services
{
    public class RabbitMQConsumerService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMQConsumerService(string connectionString, IServiceScopeFactory scopeFactory, ILogger<RabbitMQConsumerService> logger)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void StartConsuming(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    try
                    {
                        var deviceService = scope.ServiceProvider.GetRequiredService<DeviceService>();
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        _logger.LogInformation($"Received message from {queueName}: {message}");

                        switch (queueName)
                        {
                            case "device_create":
                                await deviceService.ProcessCreateDeviceMessage(message);
                                break;
                            case "device_update":
                                await deviceService.ProcessUpdateDeviceMessage(message);
                                break;
                            case "device_delete":
                                await deviceService.ProcessDeleteDeviceMessage(message);
                                break;
                            case "EnergyConsumptionData":
                                var measurementService = scope.ServiceProvider.GetRequiredService<MeasurementService>();
                                await measurementService.ProcessMeasurementData(message);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from queue {queueName}", queueName);
                    }
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            _logger.LogInformation($"Started consuming messages from queue {queueName}");
        }
    }
}
