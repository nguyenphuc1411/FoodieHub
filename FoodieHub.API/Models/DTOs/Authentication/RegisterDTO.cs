using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Authentication
{
    public class RegisterDTO
    {
        [MinLength(6)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{6,}", ErrorMessage = "Password invalid")]
        public string Password { get; set; }
    }
}
