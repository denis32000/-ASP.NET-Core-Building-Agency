using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using BuildingAgency.Models;

namespace BuildingAgency.DataAccessLevel
{
    public partial class BuildingAgencyContext : DbContext
    {
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<PrivateOwner> PrivateOwner { get; set; }
        public virtual DbSet<PropertyForRent> PropertyForRent { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Viewing> Viewing { get; set; }

        public BuildingAgencyContext(DbContextOptions<BuildingAgencyContext> options)
        : base(options)
        { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //    optionsBuilder.UseNpgsql(@"Host=localhost;Database=BuildingAgency;Username=denis;Password=123");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(e => e.ClientPassportNo)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(8);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(15);

                entity.Property(e => e.PrefType)
                    .IsRequired()
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.RentFinish).HasColumnType("date");

                entity.Property(e => e.RentStart).HasColumnType("date");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("Contract_ClientId_fkey");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Contract)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Contract_PropertyId_fkey");
            });

            modelBuilder.Entity<PrivateOwner>(entity =>
            {
                entity.HasKey(e => e.OwnerId)
                    .HasName("PK_PrivateOwner");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.OwnerPassportNo)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(8);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(15);

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar");
            });

            modelBuilder.Entity<PropertyForRent>(entity =>
            {
                entity.HasKey(e => e.PropertyId)
                    .HasName("PK_PropertyForRent");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.PostCode)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(6);

                entity.Property(e => e.PropertyNo)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.HasOne(d => d.OverseesBy)
                    .WithMany(p => p.PropertyForRent)
                    .HasForeignKey(d => d.OverseesById)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PropertyForRent_OverseesById_fkey");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.PropertyForRent)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("PropertyForRent_OwnerId_fkey");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.Position)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(1);

                entity.Property(e => e.StaffPassportNo)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(8);

                entity.HasOne(d => d.Superviser)
                    .WithMany(p => p.InverseSuperviser)
                    .HasForeignKey(d => d.SuperviserId)
                    .HasConstraintName("Staff_SuperviserId_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.Passport)
                    .IsRequired()
                    .HasColumnType("bpchar")
                    .HasMaxLength(8);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.HasOne(d => d.Client)
                    .WithOne(p => p.User)
                    //.HasForeignKey(d => d.ClientId)
                    .HasConstraintName("User_ClientId_fkey");

                entity.HasOne(d => d.PrivateOwner)
                    .WithOne(p => p.User)
                    //.HasForeignKey(d => d.ClientId)
                    .HasConstraintName("User_OwnerId_fkey");

                entity.HasOne(d => d.Staff)
                    .WithOne(p => p.User)
                    //.HasForeignKey(d => d.ClientId)
                    .HasConstraintName("User_StaffId_fkey");
                
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("User_RoleId_fkey");
            });

            
            modelBuilder.Entity<Viewing>(entity =>
            {
                entity.HasKey(e => e.ViewNo)
                    .HasName("PK_Viewing");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnType("varchar");

                entity.Property(e => e.ViewDate).HasColumnType("date");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Viewing)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Viewing_ClientId_fkey");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.Viewing)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("Viewing_PropertyId_fkey");
            });
        }
    }
}