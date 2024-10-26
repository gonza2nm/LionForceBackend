﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using lion_force_be.DBContext;

#nullable disable

namespace lion_force_be.Migrations
{
    [DbContext(typeof(DbContextLF))]
    [Migration("20241026150255_deletelimitcharactersPassword")]
    partial class deletelimitcharactersPassword
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("lion_force_be.Models.Academy", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Academies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Lion Force"
                        });
                });

            modelBuilder.Entity("lion_force_be.Models.Belt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BeltRank")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<string>("Degree")
                        .HasColumnType("varchar(2)");

                    b.HasKey("Id");

                    b.ToTable("Belts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BeltRank = "Blanco"
                        },
                        new
                        {
                            Id = 2,
                            BeltRank = "Naranja"
                        },
                        new
                        {
                            Id = 3,
                            BeltRank = "Amarillo"
                        },
                        new
                        {
                            Id = 4,
                            BeltRank = "Camuflado"
                        },
                        new
                        {
                            Id = 5,
                            BeltRank = "Verde"
                        },
                        new
                        {
                            Id = 6,
                            BeltRank = "Violeta"
                        },
                        new
                        {
                            Id = 7,
                            BeltRank = "Azul"
                        },
                        new
                        {
                            Id = 8,
                            BeltRank = "Marron"
                        },
                        new
                        {
                            Id = 9,
                            BeltRank = "Rojo"
                        },
                        new
                        {
                            Id = 10,
                            BeltRank = "Rojo negro"
                        },
                        new
                        {
                            Id = 11,
                            BeltRank = "Negro",
                            Degree = "1"
                        },
                        new
                        {
                            Id = 12,
                            BeltRank = "Negro",
                            Degree = "2"
                        },
                        new
                        {
                            Id = 13,
                            BeltRank = "Negro",
                            Degree = "3"
                        },
                        new
                        {
                            Id = 14,
                            BeltRank = "Negro",
                            Degree = "4"
                        },
                        new
                        {
                            Id = 15,
                            BeltRank = "Negro",
                            Degree = "5"
                        },
                        new
                        {
                            Id = 16,
                            BeltRank = "Negro",
                            Degree = "6"
                        },
                        new
                        {
                            Id = 17,
                            BeltRank = "Negro",
                            Degree = "7"
                        },
                        new
                        {
                            Id = 18,
                            BeltRank = "Negro",
                            Degree = "8"
                        },
                        new
                        {
                            Id = 19,
                            BeltRank = "Negro",
                            Degree = "9"
                        },
                        new
                        {
                            Id = 20,
                            BeltRank = "Negro",
                            Degree = "10"
                        });
                });

            modelBuilder.Entity("lion_force_be.Models.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UntilDate")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("lion_force_be.Models.Relations.UserService", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("UserId", "ServiceId", "PaymentDate");

                    b.HasIndex("ServiceId");

                    b.ToTable("UserServices");
                });

            modelBuilder.Entity("lion_force_be.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Supervisor"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Instructor"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Student"
                        });
                });

            modelBuilder.Entity("lion_force_be.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AcademyId")
                        .HasColumnType("int");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AcademyId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("lion_force_be.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AcademyId")
                        .HasColumnType("int");

                    b.Property<int>("BeltId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime(0)");

                    b.Property<string>("DNI")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AcademyId");

                    b.HasIndex("BeltId");

                    b.HasIndex("DNI")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AcademyId = 1,
                            BeltId = 11,
                            BirthDate = new DateTime(2003, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DNI = "44523501",
                            LastName = "Mansilla",
                            Name = "Gonzalo",
                            Password = "admin",
                            RoleId = 1
                        });
                });

            modelBuilder.Entity("lion_force_be.Models.Price", b =>
                {
                    b.HasOne("lion_force_be.Models.Service", "Service")
                        .WithMany("Prices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("lion_force_be.Models.Relations.UserService", b =>
                {
                    b.HasOne("lion_force_be.Models.Service", "Service")
                        .WithMany("UserServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("lion_force_be.Models.User", "User")
                        .WithMany("UserServices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("lion_force_be.Models.Service", b =>
                {
                    b.HasOne("lion_force_be.Models.Academy", "Academy")
                        .WithMany("Services")
                        .HasForeignKey("AcademyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Academy");
                });

            modelBuilder.Entity("lion_force_be.Models.User", b =>
                {
                    b.HasOne("lion_force_be.Models.Academy", "Academy")
                        .WithMany("Users")
                        .HasForeignKey("AcademyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("lion_force_be.Models.Belt", "Belt")
                        .WithMany("Users")
                        .HasForeignKey("BeltId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("lion_force_be.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Academy");

                    b.Navigation("Belt");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("lion_force_be.Models.Academy", b =>
                {
                    b.Navigation("Services");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("lion_force_be.Models.Belt", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("lion_force_be.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("lion_force_be.Models.Service", b =>
                {
                    b.Navigation("Prices");

                    b.Navigation("UserServices");
                });

            modelBuilder.Entity("lion_force_be.Models.User", b =>
                {
                    b.Navigation("UserServices");
                });
#pragma warning restore 612, 618
        }
    }
}
