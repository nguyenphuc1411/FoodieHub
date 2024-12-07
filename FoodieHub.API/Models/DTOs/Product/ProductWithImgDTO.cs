﻿namespace FoodieHub.API.Models.DTOs.Product
{
    public class ProductWithImgDTO
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }


        public decimal Price { get; set; }


        public IFormFile MainImage { get; set; }


        public string Description { get; set; }

        public int Discount { get; set; }
        public int ShelfLife { get; set; }
        public int StockQuantity { get; set; }

        public int CategoryID { get; set; }
    }
}
