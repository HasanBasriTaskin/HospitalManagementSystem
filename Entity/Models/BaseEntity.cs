using System.ComponentModel.DataAnnotations;

namespace Entity.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public DateTime? ModifiedDate { get; set; }
        
        public string? ModifiedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
} 