using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.Contact
{
    public class ContactDTO
    {
        public string FullName { get; set; }
        [Column(TypeName = "VARCHAR(255)")]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR(11)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(255)")]
        public string? Note { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ProductID { get; set; }
    }
}
