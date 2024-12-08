using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Authentication
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = default!;
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string Password { get; set; } = default!;
    }
}
