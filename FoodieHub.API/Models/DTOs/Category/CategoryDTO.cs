using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FoodieHub.API.Models.DTOs.Product;

namespace FoodieHub.API.Models.DTOs.Category
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        
    }
}
