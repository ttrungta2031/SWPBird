using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace SWPBirdBoarding.Models
{
    public partial class SWPBirdBoardingContext : DbContext
    {
        public SWPBirdBoardingContext()
        {
        }

        public SWPBirdBoardingContext(DbContextOptions<SWPBirdBoardingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<BirdProfile> BirdProfiles { get; set; }
        public virtual DbSet<BirdShelter> BirdShelters { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<BookingReport> BookingReports { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Service> Services { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("workstation id=SWPBirdBoarding.mssql.somee.com;packet size=4096;user id=ttrungta;pwd=admin@1234;data source=SWPBirdBoarding.mssql.somee.com;persist security info=False;initial catalog=SWPBirdBoarding");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Dob).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(200);

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Telephone).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(200);
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("Article");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);
            });

            modelBuilder.Entity<BirdProfile>(entity =>
            {
                entity.ToTable("BirdProfile");

                entity.Property(e => e.Name).HasMaxLength(300);

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(300);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.BirdProfiles)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_BirdProfile_Account");
            });

            modelBuilder.Entity<BirdShelter>(entity =>
            {
                entity.ToTable("BirdShelter");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(200);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.BirdShelters)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_BirdShelter_Account");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Booking");

                entity.Property(e => e.DateBooking).HasColumnType("datetime");

                entity.Property(e => e.DateEnd).HasColumnType("date");

                entity.Property(e => e.DateFinish).HasColumnType("datetime");

                entity.Property(e => e.DateStart).HasColumnType("date");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Booking_Account");

                entity.HasOne(d => d.BirdProfile)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.BirdProfileId)
                    .HasConstraintName("FK_Booking_BirdProfile");

                entity.HasOne(d => d.BirdShelter)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.BirdShelterId)
                    .HasConstraintName("FK_Booking_BirdShelter");
            });

            modelBuilder.Entity<BookingDetail>(entity =>
            {
                entity.ToTable("BookingDetail");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_BookingDetail_Booking");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.BookingDetails)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_BookingDetail_Service");
            });

            modelBuilder.Entity<BookingReport>(entity =>
            {
                entity.ToTable("BookingReport");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.BookingReports)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_BookingReport_Booking");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedback");

                entity.Property(e => e.Description).HasMaxLength(300);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Feedback_Account");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_Feedback_Booking");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_Notification_Account");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("FK_Notification_Booking");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.ToTable("Service");

                entity.Property(e => e.DateChange).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(200);

                entity.Property(e => e.Unit).HasMaxLength(200);

                entity.HasOne(d => d.BirdShelter)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.BirdShelterId)
                    .HasConstraintName("FK_Service_BirdShelter");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
