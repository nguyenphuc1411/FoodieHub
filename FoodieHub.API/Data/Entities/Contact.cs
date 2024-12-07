using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Data.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        [Column(TypeName ="NVARCHAR(255)")]
        public string FullName { get; set; } = default!;
        [Column(TypeName = "VARCHAR(255)")]
        public string Email { get; set; } = default!;
        [Column(TypeName = "VARCHAR(11)")]
        public string PhoneNumber { get; set; } = default!;
        [Column(TypeName = "NVARCHAR(255)")]
        public string? Note { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int ProductID { get; set; }
        public Product Product { get; set; } = default!;
    }
}
