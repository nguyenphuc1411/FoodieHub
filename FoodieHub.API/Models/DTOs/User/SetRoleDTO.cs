using System.ComponentModel.DataAnnotations;

namespace FoodieHub.API.Models.DTOs.User
{
    public class SetRoleDTO
    {
        public string UserID { get; set; }

        [RegularExpression("^(User|Admin)$", ErrorMessage = "Role must be either 'User' or 'Admin'.")]
        public string Role { get; set; }
    }
}
