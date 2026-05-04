using System.Text.Json;
using MessageBrokerService.Data;
using MessageBrokerService.Models;

namespace MessageBrokerService.Services;

public class ContactMessageWorker : BackgroundService
{
    private readonly IRabbitMqService _rabbit;
    private readonly IServiceProvider _services;
    private readonly ILogger<ContactMessageWorker> _logger;

    public ContactMessageWorker(IRabbitMqService rabbit, IServiceProvider services, ILogger<ContactMessageWorker> logger)
    {
        _rabbit = rabbit;
        _services = services;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_rabbit.IsConnected)
        {
            _logger.LogWarning("RabbitMQ not available. Worker skipped.");
            return Task.CompletedTask;
        }

        _rabbit.ConsumeMessages("contact_messages", async (json) =>
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MessageDbContext>();

            var msg = JsonSerializer.Deserialize<ContactMessage>(json);
            if (msg != null)
            {
                var existing = await db.ContactMessages.FindAsync(msg.Id);
                if (existing != null)
                {
                    existing.Status = "Sent";
                    existing.ProcessedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Contact message {id} processed.", msg.Id);
                }
            }
        });

        return Task.CompletedTask;
    }
}
