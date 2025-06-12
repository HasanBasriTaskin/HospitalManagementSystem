using Bogus;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class DoctorScheduleFaker
    {
        public static Faker<DoctorSchedule> CreateFaker(List<Doctor> doctors)
        {
            var scheduleFaker = new Faker<DoctorSchedule>("tr")
                .RuleFor(ds => ds.DoctorId, (f, ds) => f.PickRandom(doctors).Id)
                .RuleFor(ds => ds.DayOfWeek, (f, ds) => (int)f.PickRandom<DayOfWeek>())
                .RuleFor(ds => ds.StartTime, (f, ds) => f.Date.Between(
                    DateTime.Today.AddHours(8), // 08:00 AM
                    DateTime.Today.AddHours(12) // 12:00 PM
                ).TimeOfDay)
                .RuleFor(ds => ds.EndTime, (f, ds) => ds.StartTime.Add(TimeSpan.FromHours(4)))
                .RuleFor(ds => ds.AppointmentDuration, (f, ds) => f.PickRandom(new[] { 15, 20, 30 }));

            return scheduleFaker;
        }
    }
} 