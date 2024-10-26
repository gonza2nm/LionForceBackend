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
        new Belt { Id = 2, BeltRank = "Naranja" },
        new Belt { Id = 3, BeltRank = "Amarillo" },
        new Belt { Id = 4, BeltRank = "Camuflado" },
        new Belt { Id = 5, BeltRank = "Verde" },
        new Belt { Id = 6, BeltRank = "Violeta" },
        new Belt { Id = 7, BeltRank = "Azul" },
        new Belt { Id = 8, BeltRank = "Marron" },
        new Belt { Id = 9, BeltRank = "Rojo" },
        new Belt { Id = 10, BeltRank = "Rojo negro" },
        new Belt { Id = 11, BeltRank = "Negro", Degree = "1" },
        new Belt { Id = 12, BeltRank = "Negro", Degree = "2" },
        new Belt { Id = 13, BeltRank = "Negro", Degree = "3" },
        new Belt { Id = 14, BeltRank = "Negro", Degree = "4" },
        new Belt { Id = 15, BeltRank = "Negro", Degree = "5" },
        new Belt { Id = 16, BeltRank = "Negro", Degree = "6" },
        new Belt { Id = 17, BeltRank = "Negro", Degree = "7" },
        new Belt { Id = 18, BeltRank = "Negro", Degree = "8" },
        new Belt { Id = 19, BeltRank = "Negro", Degree = "9" },
        new Belt { Id = 20, BeltRank = "Negro", Degree = "10" }
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
      tb.HasMany(u => u.UserServices).WithOne(us => us.User).HasForeignKey(us => us.UserId);
      tb.HasOne(u => u.Belt).WithMany(b => b.Users).HasForeignKey(u => u.BeltId);
      tb.HasData(
          new User { Id = 1, Name = "Gonzalo", LastName = "Mansilla", DNI = "44523501", BirthDate = new DateTime(2003, 1, 11), BeltId = 11, RoleId = 1, AcademyId = 1 }
      );
    });
    builder.Entity<Academy>(tb =>
    {
      tb.Property(a => a.Name).HasColumnType("varchar(50)");
      tb.HasMany(a => a.Users).WithOne(u => u.Academy).HasForeignKey(u => u.AcademyId);
      tb.HasData(
        new Academy { Id = 1, Name = "Lion Force" }
      );
    });
  }
}