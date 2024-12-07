using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.API.Models.DTOs.Category
{
    public class RecipeCategoryWithImgDTO
    {
        public int CategoryID { get; set; }


        public string CategoryName { get; set; }


        public IFormFile ImageURL { get; set; }

    }
}
