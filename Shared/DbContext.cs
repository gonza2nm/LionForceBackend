using lion_force_be.Models;
using lion_force_be.Models.Relations;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.DBContext;

public sealed class DbContextLF : DbContext
{
  public DbContextLF(DbContextOptions<DbContextLF> options) : base(options) { }
  public required DbSet<Academy> Academies { get; set; }
  public required DbSet<User> Users { get; set; }
  public required DbSet<Role> Roles { get; set; }
  public required DbSet<Belt> Belts { get; set; }
  public required DbSet<Service> Services { get; set; }
  public required DbSet<Price> Prices { get; set; }
  public required DbSet<UserService> UserServices { get; set; }
  public required DbSet<Invoice> Invoices { get; set; }
  protected override void OnModelCreating(ModelBuilder builder)
  {

    builder.Entity<UserService>(tb =>
    {
      tb.HasKey(us => new { us.UserId, us.ServiceId });
      tb.Property(us => us.Active).HasDefaultValue(true);
      tb.HasOne(us => us.User).WithMany(u => u.UserServices).HasForeignKey(us => us.UserId);
      tb.HasOne(us => us.Service).WithMany(s => s.UserServices).HasForeignKey(us => us.ServiceId);
      tb.HasMany(us => us.Invoices).WithOne(i => i.UserService).HasForeignKey(i => new { i.UserId, i.ServiceId }).OnDelete(DeleteBehavior.NoAction);
    });
    builder.Entity<Role>(tb =>
    {
      tb.Property(r => r.Name).HasColumnType("varchar(15)");
      tb.HasData(
        new Role { Id = 1, Name = "Admin" },
        new Role { Id = 2, Name = "Supervisor" },
        new Role { Id = 3, Name = "Instructor" },
        new Role { Id = 4, Name = "Student" }
      );
    });
    builder.Entity<Belt>(tb =>
    {
      tb.Property(b => b.BeltRank).HasColumnType("varchar(15)");
      tb.Property(b => b.Degree).HasColumnType("varchar(2)");
      tb.HasData(
        new Belt { Id = 1, BeltRank = "Blanco" },
        new Belt { Id = 2, BeltRank = "Naranja", Decided = false },
        new Belt { Id = 3, BeltRank = "Naranja", Decided = true },
        new Belt { Id = 4, BeltRank = "Amarillo", Decided = false },
        new Belt { Id = 5, BeltRank = "Amarillo", Decided = true },
        new Belt { Id = 6, BeltRank = "Camuflado", Decided = false },
        new Belt { Id = 7, BeltRank = "Camuflado", Decided = true },
        new Belt { Id = 8, BeltRank = "Verde", Decided = false },
        new Belt { Id = 9, BeltRank = "Verde", Decided = true },
        new Belt { Id = 10, BeltRank = "Violeta", Decided = false },
        new Belt { Id = 11, BeltRank = "Violeta", Decided = true },
        new Belt { Id = 12, BeltRank = "Azul", Decided = false },
        new Belt { Id = 13, BeltRank = "Azul", Decided = true },
        new Belt { Id = 14, BeltRank = "Marron", Decided = false },
        new Belt { Id = 15, BeltRank = "Marron", Decided = true },
        new Belt { Id = 16, BeltRank = "Rojo", Decided = false },
        new Belt { Id = 17, BeltRank = "Rojo", Decided = true },
        new Belt { Id = 18, BeltRank = "Rojo negro", Decided = true },
        new Belt { Id = 19, BeltRank = "Negro", Degree = "1", Decided = true },
        new Belt { Id = 20, BeltRank = "Negro", Degree = "2", Decided = true },
        new Belt { Id = 21, BeltRank = "Negro", Degree = "3", Decided = true },
        new Belt { Id = 22, BeltRank = "Negro", Degree = "4", Decided = true },
        new Belt { Id = 23, BeltRank = "Negro", Degree = "5", Decided = true },
        new Belt { Id = 24, BeltRank = "Negro", Degree = "6", Decided = true },
        new Belt { Id = 25, BeltRank = "Negro", Degree = "7", Decided = true },
        new Belt { Id = 26, BeltRank = "Negro", Degree = "8", Decided = true },
        new Belt { Id = 27, BeltRank = "Negro", Degree = "9", Decided = true }
      );
    });
    builder.Entity<User>(tb =>
    {
      tb.Property(u => u.Name).HasColumnType("varchar(20)");
      tb.Property(u => u.LastName).HasColumnType("varchar(20)");
      tb.HasIndex(u => u.DNI).IsUnique();
      tb.Property(u => u.BirthDate).HasColumnType("datetime(0)");
      tb.HasOne(u => u.Belt).WithMany(b => b.Users).HasForeignKey(u => u.BeltId);
      tb.HasOne(u => u.Role).WithMany(r => r.Users).HasForeignKey(u => u.RoleId);
      tb.HasOne(u => u.Academy).WithMany(a => a.Users).HasForeignKey(u => u.AcademyId);
      tb.HasMany(u => u.UserServices).WithOne(us => us.User).HasForeignKey(us => us.UserId).OnDelete(DeleteBehavior.Cascade);
      tb.HasOne(u => u.Belt).WithMany(b => b.Users).HasForeignKey(u => u.BeltId);
    });

    builder.Entity<Service>(tb =>
    {
      tb.Property(s => s.Name).HasColumnType("varchar(30)");
      tb.Property(s => s.Details).HasColumnType("varchar(200)");
      tb.HasMany(s => s.Prices).WithOne(p => p.Service).HasForeignKey(p => p.ServiceId).OnDelete(DeleteBehavior.Cascade);
      tb.HasMany(s => s.UserServices).WithOne(usser => usser.Service).HasForeignKey(usser => usser.ServiceId).OnDelete(DeleteBehavior.Restrict);
      tb.HasOne(s => s.Academy).WithMany(a => a.Services).HasForeignKey(s => s.AcademyId);
    });
    builder.Entity<Price>(tb =>
    {
      tb.Property(p => p.FromDate).HasColumnType("datetime(0)");
      tb.Property(p => p.UntilDate).HasColumnType("datetime(0)");
      tb.Property(p => p.Value).HasPrecision(18, 2);
      tb.HasOne(p => p.Service).WithMany(s => s.Prices).HasForeignKey(p => p.ServiceId);
    });

    builder.Entity<Academy>(tb =>
    {
      tb.Property(a => a.Name).HasColumnType("varchar(50)");
      tb.HasMany(a => a.Users).WithOne(u => u.Academy).HasForeignKey(u => u.AcademyId);
      tb.HasData(
        new Academy { Id = 1, Name = "Lion Force" }
      );
    });
    builder.Entity<Invoice>(tb =>
    {
      tb.Property(i => i.PaymentDate).HasColumnType("datetime(0)");
      tb.Property(i => i.DueDate).HasColumnType("datetime(0)");
      tb.Property(i => i.Paid).HasDefaultValue(false);
      tb.Property(i => i.PaymentDate).HasDefaultValue(null);
      tb.HasOne(i => i.UserService).WithMany(us => us.Invoices)
        .HasForeignKey(i => new { i.UserId, i.ServiceId })
        .OnDelete(DeleteBehavior.Restrict); // No borra las facturas
    });
  }
}