using Entity.Models;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData
{
    public class DatabaseSeeder
    {
        private readonly ProjectMainContext _context;

        public DatabaseSeeder(ProjectMainContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Create departments
            var departments = await SeedDepartments();
            
            // Create doctors
            var doctors = await SeedDoctors(departments);
            
            // Create patients
            var patients = await SeedPatients();
            
            // Create doctor schedules
            await SeedDoctorSchedules(doctors);
            
            // Create appointments
            await SeedAppointments(doctors, patients);
        }

        private async Task<List<Department>> SeedDepartments()
        {
            if (await _context.Departments.AnyAsync())
                return await _context.Departments.ToListAsync();

            var departments = DepartmentFaker.CreateFaker().Generate(15);
            await _context.Departments.AddRangeAsync(departments);
            await _context.SaveChangesAsync();
            return departments;
        }

        private async Task<List<Doctor>> SeedDoctors(List<Department> departments)
        {
            if (await _context.Doctors.AnyAsync())
                return await _context.Doctors.ToListAsync();

            var doctors = DoctorFaker.CreateFaker(departments).Generate(150);
            await _context.Doctors.AddRangeAsync(doctors);
            await _context.SaveChangesAsync();
            return doctors;
        }

        private async Task<List<Patient>> SeedPatients()
        {
            if (await _context.Patients.AnyAsync())
                return await _context.Patients.ToListAsync();

            var patients = PatientFaker.CreateFaker().Generate(300);
            await _context.Patients.AddRangeAsync(patients);
            await _context.SaveChangesAsync();
            return patients;
        }

        private async Task SeedDoctorSchedules(List<Doctor> doctors)
        {
            if (await _context.DoctorSchedules.AnyAsync())
                return;

            var schedules = new List<DoctorSchedule>();
            foreach (var doctor in doctors)
            {
                var doctorSchedules = DoctorScheduleFaker.CreateFaker(new List<Doctor> { doctor })
                    .Generate(5); // 5 days schedule for each doctor
                schedules.AddRange(doctorSchedules);
            }

            await _context.DoctorSchedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
        }

        private async Task SeedAppointments(List<Doctor> doctors, List<Patient> patients)
        {
            if (await _context.Appointments.AnyAsync())
                return;

            var appointments = AppointmentFaker.CreateFaker(doctors, patients)
                .Generate(1000);
            await _context.Appointments.AddRangeAsync(appointments);
            await _context.SaveChangesAsync();
        }
    }
} 