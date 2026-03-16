using Microsoft.EntityFrameworkCore;
using SU26_PRN222_Healthcare.CareBusiness.Entities;

namespace SU26_PRN222_Healthcare.CareBusiness
{
    public class CareDbContext : DbContext
    {
        public CareDbContext(DbContextOptions<CareDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique index on Doctor.LicenseNumber
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.LicenseNumber)
                .IsUnique();

            // Appointment -> Patient (restrict delete)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.PatientID)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment -> Doctor (restrict delete)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorID)
                .OnDelete(DeleteBehavior.Restrict);

            // Session -> User (cascade delete)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                ID = 1,
                FullName = "Administrator",
                Email = "admin@care.com",
                Password = "Admin@123",
                Role = "Admin",
                CreatedAt = new DateTime(2026, 1, 1)
            });

            // Seed patient user
            modelBuilder.Entity<User>().HasData(new User
            {
                ID = 2,
                FullName = "John Patient",
                Email = "patient@care.com",
                Password = "Patient@123",
                Role = "Patient",
                CreatedAt = new DateTime(2026, 1, 1)
            });

            // Seed sample doctors
            modelBuilder.Entity<Doctor>().HasData(
                new Doctor
                {
                    ID = 1,
                    DoctorName = "Dr. Nguyen Van A",
                    Specialty = "Cardiology",
                    LicenseNumber = "LIC-001",
                    MaxPatients = 10,
                    Active = true
                },
                new Doctor
                {
                    ID = 2,
                    DoctorName = "Dr. Tran Thi B",
                    Specialty = "Neurology",
                    LicenseNumber = "LIC-002",
                    MaxPatients = 8,
                    Active = true
                },
                new Doctor
                {
                    ID = 3,
                    DoctorName = "Dr. Le Van C",
                    Specialty = "Dermatology",
                    LicenseNumber = "LIC-003",
                    MaxPatients = 15,
                    Active = false
                }
            );
        }
    }
}
