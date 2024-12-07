using FoodieHub.MVC.Models.Response;

namespace FoodieHub.MVC.Service.Interfaces
{
    public static class PanigationHelper
    {
        public static PaginatedResult<T> Paginate<T>(this IEnumerable<T> items, int pageSize, int currentPage)
        {
            if (items == null || !items.Any())
            {
                return new PaginatedResult<T>
                {
                    TotalItems = 0,
                    TotalPages = 1,
                    CurrentPage = 1,
                    PageSize = pageSize,
                    Items = new List<T>()
                };
            }
            int totalItems = items.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedItems = items.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedResult<T>
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize,
                Items = pagedItems
            };
        }
    }
}
