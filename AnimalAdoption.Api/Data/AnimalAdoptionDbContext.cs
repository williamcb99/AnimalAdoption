using Microsoft.EntityFrameworkCore;

public class AnimalAdoptionDbContext : DbContext
{
    public AnimalAdoptionDbContext(DbContextOptions<AnimalAdoptionDbContext> options) : base(options) { }

    public DbSet<Animal> Animals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}