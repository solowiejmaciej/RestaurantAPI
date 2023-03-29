using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities;

public class RestaurantDBContext : DbContext
{
    public RestaurantDBContext(DbContextOptions<RestaurantDBContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>() // ustawiamy properiesy  danego pola
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(25);
        modelBuilder.Entity<Dish>()
            .Property(d => d.Name)
            .IsRequired();
        modelBuilder.Entity<Address>()
            .Property(d => d.City)
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<Address>()
            .Property(d => d.Street)
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();
        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();
    }
}