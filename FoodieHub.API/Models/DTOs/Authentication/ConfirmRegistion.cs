using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.Authentication
{
    public class ConfirmRegistion
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = default!;
        public string Data { get; set; } = default!;
    }
}
