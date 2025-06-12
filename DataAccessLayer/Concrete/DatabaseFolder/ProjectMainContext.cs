using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder
{
    public class ProjectMainContext : IdentityDbContext<IdentityUser>
    {
        public ProjectMainContext(DbContextOptions<ProjectMainContext> options) : base(options)
        {
        }

        // Hospital Management DbSets
        public DbSet<Department> Departments { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints
            ConfigureDepartment(modelBuilder);
            ConfigureDoctor(modelBuilder);
            ConfigurePatient(modelBuilder);
            ConfigureDoctorSchedule(modelBuilder);
            ConfigureAppointment(modelBuilder);
        }

        private static void ConfigureDepartment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }

        private static void ConfigureDoctor(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(20);
                entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                
                entity.HasIndex(e => e.LicenseNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                // Relationship with Department
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static void ConfigurePatient(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IdentityNumber).IsRequired().HasMaxLength(11);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.EmergencyContact).HasMaxLength(100);
                entity.Property(e => e.EmergencyPhone).HasMaxLength(15);

                entity.HasIndex(e => e.IdentityNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Phone);
            });
        }

        private static void ConfigureDoctorSchedule(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DayOfWeek).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.AppointmentDuration).IsRequired().HasDefaultValue(30);

                // Prevent overlapping schedules for same doctor on same day
                entity.HasIndex(e => new { e.DoctorId, e.DayOfWeek, e.StartTime }).IsUnique();

                // Relationship with Doctor
                entity.HasOne(ds => ds.Doctor)
                    .WithMany(d => d.DoctorSchedules)
                    .HasForeignKey(ds => ds.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureAppointment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AppointmentDate).IsRequired();
                entity.Property(e => e.Duration).IsRequired().HasDefaultValue(30);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.Complaint).HasMaxLength(1000);
                entity.Property(e => e.Notes).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).HasMaxLength(50).HasDefaultValue("System");

                // Prevent double booking - same doctor at same time
                entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate }).IsUnique();

                // Index for performance
                entity.HasIndex(e => e.AppointmentDate);
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.Status);

                // Relationships
                entity.HasOne(a => a.Patient)
                    .WithMany(p => p.Appointments)
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Doctor)
                    .WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
