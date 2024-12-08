using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Authentication
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = default!;
    }
}
