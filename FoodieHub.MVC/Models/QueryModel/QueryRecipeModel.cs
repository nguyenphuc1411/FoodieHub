﻿namespace FoodieHub.MVC.Models.QueryModel
{
    public class QueryRecipeModel:QueryModel
    {
        public int? CategoryID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdminUpload { get; set; }
        public int? CookOption { get; set; }
    }
}
