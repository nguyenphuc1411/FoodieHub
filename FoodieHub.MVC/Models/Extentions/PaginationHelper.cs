using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Extentions
{
    public static class PaginationHelper
    {
        public static PaginatedModel<T> Paginate<T>(this IEnumerable<T> items, int pageSize, int currentPage)
        {
            if (items == null || !items.Any())
            {
                return new PaginatedModel<T>
                {
                    TotalItems = 0,
                    TotalPages = 1, 
                    Page = 1,
                    PageSize = pageSize,
                    Items = new List<T>() 
                };
            }
            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedItems = items.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedModel<T>
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Page = currentPage,
                PageSize = pageSize,
                Items = pagedItems
            };
        }
    }
}
