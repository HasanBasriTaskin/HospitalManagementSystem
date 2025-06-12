using Bogus;
using Entity.Enums;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class AppointmentFaker
    {
        public static Faker<Appointment> CreateFaker(List<Doctor> doctors, List<Patient> patients)
        {
            var appointmentFaker = new Faker<Appointment>("tr")
                .RuleFor(a => a.DoctorId, (f, a) => f.PickRandom(doctors).Id)
                .RuleFor(a => a.PatientId, (f, a) => f.PickRandom(patients).Id)
                .RuleFor(a => a.AppointmentDate, (f, a) => f.Date.Between(
                    DateTime.Now.AddDays(-30), // 30 days ago
                    DateTime.Now.AddDays(30)   // 30 days from now
                ))
                .RuleFor(a => a.Duration, (f, a) => f.PickRandom(new[] { 15, 20, 30 }))
                .RuleFor(a => a.Status, (f, a) => f.PickRandom<AppointmentStatus>())
                .RuleFor(a => a.Complaint, (f, a) => f.Lorem.Sentence())
                .RuleFor(a => a.Notes, (f, a) => f.Lorem.Paragraph())
                .RuleFor(a => a.CreatedBy, (f, a) => "System");

            return appointmentFaker;
        }
    }
} 