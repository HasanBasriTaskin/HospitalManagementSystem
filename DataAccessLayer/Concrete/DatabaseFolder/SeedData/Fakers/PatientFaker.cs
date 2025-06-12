using Bogus;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class PatientFaker
    {
        public static Faker<Patient> CreateFaker()
        {
            var patientFaker = new Faker<Patient>("tr")
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.IdentityNumber, f => f.Random.Replace("###########"))
                .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber("0### ### ## ##"))
                .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName.ToLower(), p.LastName.ToLower()))
                .RuleFor(p => p.Address, f => f.Address.FullAddress())
                .RuleFor(p => p.EmergencyContact, f => f.Name.FullName())
                .RuleFor(p => p.EmergencyPhone, f => f.Phone.PhoneNumber("0### ### ## ##"));

            return patientFaker;
        }
    }
} 