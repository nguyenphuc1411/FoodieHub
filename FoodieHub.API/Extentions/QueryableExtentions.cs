using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodieHub.API.Extentions
{
    public static class QueryableExtentions
    {
        public static async Task<PaginatedModel<T>> ApplyQuery<T>(
        this IQueryable<T> queryable,
        QueryModel query,
        Expression<Func<T, string>>? searchFieldSelector = null)
        {
            if (!string.IsNullOrEmpty(query.SearchItem) && searchFieldSelector != null)
            {
                var searchTerm = query.SearchItem.Trim().ToLower();
                queryable = queryable.Where(entity =>
                    EF.Functions.Like(EF.Property<string>(entity!, GetMemberName(searchFieldSelector)), $"%{searchTerm}%"));
            }
            // Sắp xếp
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                queryable = query.Ascending
                    ? queryable.OrderBy(x => EF.Property<object>(x!, query.SortBy))
                    : queryable.OrderByDescending(x => EF.Property<object>(x!, query.SortBy));
            }

            // Phân trang
            var totalItems = await queryable.CountAsync();
            var items = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var pageCount = (int)Math.Ceiling(totalItems / (double)query.PageSize);

            return new PaginatedModel<T>
            {
                TotalItems = totalItems,
                Page = query.Page,
                TotalPages = pageCount,
                PageSize = query.PageSize,
                Items = items
            };
        }

        private static string GetMemberName<T>(Expression<Func<T, string>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException("Expression must be a member expression.");
        }
    }
}
