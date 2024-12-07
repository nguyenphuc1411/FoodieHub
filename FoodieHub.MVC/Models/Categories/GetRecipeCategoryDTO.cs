using System.ComponentModel.DataAnnotations.Schema;

namespace FoodieHub.MVC.Models.Categories
{
    public class GetRecipeCategoryDTO
    {
        public int CategoryID { get; set; }


        public string CategoryName { get; set; }


        public string ImageURL { get; set; }

        public bool IsActice { get; set; } 

        public bool IsDeleted { get; set; } 
    }
}
