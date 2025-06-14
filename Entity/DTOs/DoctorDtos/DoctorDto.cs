namespace Entity.DTOs.DoctorDtos
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Title { get; set; } = string.Empty;
        public string DisplayName => $"{Title} {FullName}";
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
} 