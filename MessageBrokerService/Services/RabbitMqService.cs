using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageBrokerService.Services;

public interface IRabbitMqService
{
    void PublishMessage<T>(string queueName, T message);
    void ConsumeMessages(string queueName, Func<string, Task> handler);
    bool IsConnected { get; }
}

public class RabbitMqService : IRabbitMqService, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMqService> _logger;

    public bool IsConnected => _connection?.IsOpen == true;

    public RabbitMqService(IConfiguration config, ILogger<RabbitMqService> logger)
    {
        _logger = logger;
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
                Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
                UserName = config["RabbitMQ:Username"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _logger.LogInformation("Connected to RabbitMQ");
        }
        catch (Exception ex)
        {
            _logger.LogWarning("RabbitMQ not available: {msg}. Running without broker.", ex.Message);
        }
    }

    public void PublishMessage<T>(string queueName, T message)
    {
        if (_channel == null) return;

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var props = _channel.CreateBasicProperties();
        props.Persistent = true;
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: props, body: body);
    }

    public void ConsumeMessages(string queueName, Func<string, Task> handler)
    {
        if (_channel == null) return;

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) =>
        {
            var body = Encoding.UTF8.GetString(ea.Body.ToArray());
            await handler(body);
            _channel.BasicAck(ea.DeliveryTag, false);
        };
        _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
