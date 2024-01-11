using System;
using System.IO;
using System.Timers;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Globalization;

class Program
{
    private static int deviceId;
    private static StreamReader? reader;
    private static System.Timers.Timer? timer;

    private static DateTime currentTimestamp;
    private static int? lastMeasurement = null;
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Please provide a device ID.");
            return;
        }
        
        // Parse the device ID to an integer
        if (!int.TryParse(args[0], out deviceId))
        {
            Console.WriteLine("Invalid device ID. Please provide a valid integer.");
            return;
        }
        Console.WriteLine($"Starting data processing for device ID: {deviceId}");

        currentTimestamp = DateTime.Today;

        // Open CSV file for reading
        reader = new StreamReader("sensor.csv");

        // Set up a timer to tick every second
        timer = new System.Timers.Timer(2000);
        timer.Elapsed += OnTimedEvent;
        timer.AutoReset = true;
        timer.Enabled = true;

        Console.WriteLine("Press Enter to exit the program...");
        Console.ReadLine();
    }

    private static void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {
        string currentMeasurementStr = ReadNextMeasurement();
        if (currentMeasurementStr == null)
        {
            Console.WriteLine("No more data to read.");
            timer.Stop();
            return;
        }

        int currentMeasurement = int.Parse(currentMeasurementStr);
        int measurementToSend;

        if (lastMeasurement.HasValue)
        {
            // Send the difference if not the first value
            measurementToSend = currentMeasurement - lastMeasurement.Value;
        }
        else
        {
            // Send the current measurement if it's the first value
            measurementToSend = currentMeasurement;
        }

        lastMeasurement = currentMeasurement;

        var jsonData = JsonSerializer.Serialize(new
        {
            Timestamp = new DateTimeOffset(currentTimestamp).ToUnixTimeSeconds(),
            DeviceId = deviceId,
            MeasurementValue = measurementToSend
        });

        SendMessageToQueue(jsonData);

        // Increment the timestamp by 15 minutes
        currentTimestamp = currentTimestamp.AddMinutes(15);
    }
    private static string ReadNextMeasurement()
    {
        if (reader != null && !reader.EndOfStream)
        {
            string line = reader.ReadLine();
            double measurement = double.Parse(line, CultureInfo.InvariantCulture);
            int truncatedMeasurement = (int)measurement;
            return truncatedMeasurement.ToString();
        }
        else
        {
            return null;
        }
    }

    private static void SendMessageToQueue(string message)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://ejjgjahg:HFXjx2E2jq79wTZau8rOQO41sw1EiYeq@crow.rmq.cloudamqp.com/ejjgjahg")
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "EnergyConsumptionData",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "EnergyConsumptionData",
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine(" [x] Sent {0}", message);
        }
    }

}
