using funeraisefunerais.Models;
using Microsoft.EntityFrameworkCore;

namespace funeraisefunerais.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
}
