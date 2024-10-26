
using lion_force_be.Models;
using lion_force_be.Models.Relations;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.DBContext;

public sealed class DbContextLF : DbContext
{
  public DbContextLF(DbContextOptions<DbContextLF> options) : base(options) { }
  public DbSet<Academy> Academies { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<Belt> Belts { get; set; }
  public DbSet<Service> Services { get; set; }
  public DbSet<Price> Prices { get; set; }
  public DbSet<UserService> UserServices { get; set; }
  protected override void OnModelCreating(ModelBuilder builder)
  {

    builder.Entity<UserService>(tb =>
    {
      tb.HasKey(us => new { us.UserId, us.ServiceId, us.PaymentDate });
      tb.HasOne(us => us.User).WithMany(u => u.UserServices).HasForeignKey(us => us.UserId);
      tb.HasOne(us => us.Service).WithMany(s => s.UserServices).HasForeignKey(us => us.ServiceId);
    });
  }
}