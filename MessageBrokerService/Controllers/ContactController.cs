using MessageBrokerService.Data;
using MessageBrokerService.DTOs;
using MessageBrokerService.Models;
using MessageBrokerService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MessageBrokerService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly MessageDbContext _db;
    private readonly IRabbitMqService _rabbit;

    public ContactController(MessageDbContext db, IRabbitMqService rabbit)
    {
        _db = db;
        _rabbit = rabbit;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendContactDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Body))
            return BadRequest(new { message = "Name, email, and message body are required." });

        var msg = new ContactMessage
        {
            Name = dto.Name,
            Email = dto.Email,
            Subject = dto.Subject,
            Body = dto.Body,
            CustomerId = dto.CustomerId,
            Status = _rabbit.IsConnected ? "Queued" : "Sent"
        };

        _db.ContactMessages.Add(msg);
        await _db.SaveChangesAsync();

        // Publish to RabbitMQ queue
        if (_rabbit.IsConnected)
        {
            _rabbit.PublishMessage("contact_messages", msg);
        }
        else
        {
            msg.Status = "Sent";
            msg.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return Ok(new ContactMessageDto(msg.Id, msg.Name, msg.Email, msg.Subject, msg.Body, msg.CustomerId, msg.Status, msg.CreatedAt));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var msgs = await _db.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new ContactMessageDto(m.Id, m.Name, m.Email, m.Subject, m.Body, m.CustomerId, m.Status, m.CreatedAt))
            .ToListAsync();
        return Ok(msgs);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomer(int customerId)
    {
        var msgs = await _db.ContactMessages
            .Where(m => m.CustomerId == customerId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new ContactMessageDto(m.Id, m.Name, m.Email, m.Subject, m.Body, m.CustomerId, m.Status, m.CreatedAt))
            .ToListAsync();
        return Ok(msgs);
    }

    [HttpGet("status")]
    public IActionResult Status() =>
        Ok(new { rabbitMqConnected = _rabbit.IsConnected, message = "Message Broker Service running" });
}
