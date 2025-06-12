using Bogus;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class DoctorFaker
    {
        public static Faker<Doctor> CreateFaker(List<Department> departments)
        {
            var doctorFaker = new Faker<Doctor>("tr")
                .RuleFor(d => d.FirstName, f => f.Name.FirstName())
                .RuleFor(d => d.LastName, f => f.Name.LastName())
                .RuleFor(d => d.Title, f => f.PickRandom(new[]
                {
                    "Prof. Dr.",
                    "Doç. Dr.",
                    "Dr.",
                    "Uzm. Dr."
                }))
                .RuleFor(d => d.LicenseNumber, f => f.Random.Replace("####-####-####"))
                .RuleFor(d => d.Phone, f => f.Phone.PhoneNumber("0### ### ## ##"))
                .RuleFor(d => d.Email, (f, d) => f.Internet.Email(d.FirstName.ToLower(), d.LastName.ToLower()))
                .RuleFor(d => d.DepartmentId, f => f.PickRandom(departments).Id);

            return doctorFaker;
        }
    }
} 