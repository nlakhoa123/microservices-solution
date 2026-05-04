using MessageBrokerService.Models;
using Microsoft.EntityFrameworkCore;

namespace MessageBrokerService.Data;

public class MessageDbContext : DbContext
{
    public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options) { }
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
}
