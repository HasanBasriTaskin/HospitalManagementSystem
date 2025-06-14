using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.DepartmentDtos
{
    public class DepartmentCreateDto
    {
        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Department name can be max 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can be max 500 characters.")]
        public string? Description { get; set; }
    }
} 