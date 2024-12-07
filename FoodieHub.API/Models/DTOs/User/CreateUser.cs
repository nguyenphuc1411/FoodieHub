using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.User
{
    public class CreateUser
    {
        [Required]
        [MinLength(4)]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(255)]
        public string? Bio { get; set; }
        public IFormFile? File { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string Role { get; set; }
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string Password { get; set; }
    }
}
