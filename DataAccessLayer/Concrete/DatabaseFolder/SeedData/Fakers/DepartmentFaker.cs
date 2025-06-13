using Bogus;
using Entity.Models;
using System.Collections.Generic;

namespace DataAccessLayer.Concrete.DatabaseFolder.SeedData.Fakers
{
    public class DepartmentFaker
    {
        private static readonly List<string> AvailableDepartments = new()
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
        };

        public static Faker<Department> CreateFaker()
        {
            var departmentFaker = new Faker<Department>("tr")
                .RuleFor(d => d.Name, f =>
                {
                    if (AvailableDepartments.Count == 0)
                        return "Genel Tıp"; // Fallback department name

                    var index = f.Random.Number(0, AvailableDepartments.Count - 1);
                    var departmentName = AvailableDepartments[index];
                    AvailableDepartments.RemoveAt(index);
                    return departmentName;
                })
                .RuleFor(d => d.Description, f => f.Lorem.Paragraph(1));

            return departmentFaker;
        }
    }
} 