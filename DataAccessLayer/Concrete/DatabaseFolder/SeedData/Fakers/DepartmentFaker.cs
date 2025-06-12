using Bogus;
using Entity.Models;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class DepartmentFaker
    {
        public static Faker<Department> CreateFaker()
        {
            var departmentFaker = new Faker<Department>("tr")
                .RuleFor(d => d.Name, f => f.PickRandom(new[]
                {
                    "Kardiyoloji",
                    "Nöroloji",
                    "Ortopedi",
                    "Göz Hastalıkları",
                    "Kulak Burun Boğaz",
                    "Dahiliye",
                    "Cildiye",
                    "Gastroenteroloji",
                    "Üroloji",
                    "Kadın Hastalıkları ve Doğum",
                    "Çocuk Hastalıkları",
                    "Psikiyatri",
                    "Fizik Tedavi",
                    "Göğüs Hastalıkları",
                    "Endokrinoloji"
                }))
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph(1));

            return departmentFaker;
        }
    }
} 