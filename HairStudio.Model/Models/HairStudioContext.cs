using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HairStudio.Model.Models
{
    public partial class HairStudioContext : DbContext
    {
        public HairStudioContext()
        {
        }

        public HairStudioContext(DbContextOptions<HairStudioContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<EmailConfirmation> EmailConfirmations { get; set; } = null!;
        public virtual DbSet<Gender> Genders { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductType> ProductTypes { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<WorkHour> WorkHours { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.HasIndex(e => e.Name, "UQ__Brand__737584F6292306AC")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customer");

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);
            });

            modelBuilder.Entity<EmailConfirmation>(entity =>
            {
                entity.ToTable("EmailConfirmation");

                entity.Property(e => e.EmailConfirmationId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ConfirmationCode).HasMaxLength(256);

                entity.Property(e => e.ExpiresAt).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.EmailConfirmations)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailConfirmation_User");
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.ToTable("Gender");

                entity.HasIndex(e => e.Name, "UQ__Gender__737584F6C1E82456")
                    .IsUnique();

                entity.Property(e => e.GenderId).ValueGeneratedOnAdd();

                entity.Property(e => e.Name).HasMaxLength(6);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Message");

                entity.Property(e => e.Content).HasMaxLength(300);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_User");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaidAt).HasColumnType("datetime");

                entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.OrderStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.OrderStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_OrderStatus");

                entity.HasOne(d => d.PaymentStatus)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_PaymentStatus");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId })
                    .HasName("PK__OrderIte__08D097A33D9501EF");

                entity.ToTable("OrderItem");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_OrderItem_Order");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_Product");
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.ToTable("OrderStatus");

                entity.HasIndex(e => e.Name, "UQ__OrderSta__737584F6400B1A6D")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("PasswordResetToken");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.ResetToken).HasMaxLength(256);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PasswordResetTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PasswordResetToken_User");
            });

            modelBuilder.Entity<PaymentStatus>(entity =>
            {
                entity.ToTable("PaymentStatus");

                entity.HasIndex(e => e.Name, "UQ__PaymentS__737584F63A5C1B1E")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Brand");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_Product_User");

                entity.HasOne(d => d.ProductType)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_ProductType");
            });

            modelBuilder.Entity<ProductType>(entity =>
            {
                entity.ToTable("ProductType");

                entity.HasIndex(e => e.Name, "UQ__ProductT__737584F613EF2247")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("Reservation");

                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.HasOne(d => d.ClientCustomer)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.ClientCustomerId)
                    .HasConstraintName("FK_Reservation_Customer");

                entity.HasOne(d => d.ClientUser)
                    .WithMany(p => p.ReservationClientUsers)
                    .HasForeignKey(d => d.ClientUserId)
                    .HasConstraintName("FK_Reservation_User");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.ReservationEmployees)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Employee");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reservation_Service");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasIndex(e => e.Name, "UQ__Role__737584F672A420CC")
                    .IsUnique();

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Discount).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.CreatedByUser)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.CreatedByUserId)
                    .HasConstraintName("FK_Service_User");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.GenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Service_Gender");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D1053454A559E5")
                    .IsUnique();

                entity.Property(e => e.Bio).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.PasswordHash).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_Role"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_UserRole_User"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760AD8676E4C3");

                            j.ToTable("UserRole");
                        });
            });

            modelBuilder.Entity<WorkHour>(entity =>
            {
                entity.HasKey(e => e.WorkHourId)
                    .HasName("PK__WorkHour__1C68E87F945B24BC");

                entity.ToTable("WorkHour");

                entity.Property(e => e.WorkHourId).HasColumnName("WorkHourId");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.TimeFrom).HasColumnType("time(0)");

                entity.Property(e => e.TimeTo).HasColumnType("time(0)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkHours)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkHour_User");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
