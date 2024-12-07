using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.Contact
{
    public class GetContact
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Note { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductID { get; set; }

        public string ProductName { get; set; }
        public string ProductImage { get; set; }
    }
}
