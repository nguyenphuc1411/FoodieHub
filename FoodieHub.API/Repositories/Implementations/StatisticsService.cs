using FoodieHub.API.Data;
using FoodieHub.API.Models.DTOs.Statistics;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Repositories.Implementations
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        public StatisticsService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<StatisticsDTO<int>> GetOrder(string by)
        {
            var data = new StatisticsDTO<int>();
            if (by == "Today")
            {
                var todayCount = await _context.Orders
                    .Where(x=>DateOnly.FromDateTime(x.OrderedAt.Date)==DateOnly.FromDateTime(DateTime.Now)).CountAsync();

                var yesterdayCount = await _context.Orders
                    .Where(x => DateOnly.FromDateTime(x.OrderedAt.Date) == DateOnly.FromDateTime(DateTime.Now.AddDays(-1))).CountAsync();
                data.Total = todayCount;
                data.GetBy = "Today";
                if (yesterdayCount > 0)
                {
                    data.Trend = (int)Math.Round((double)(todayCount - yesterdayCount) / yesterdayCount * 100);
                }
                else
                {
                    data.Trend = 0;
                }
            }
            else
            {
                if (by == "ThisMonth")
                {
                    // Lấy số đơn hàng trong tháng hiện tại
                    var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var thisMonthCount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfThisMonth && x.OrderedAt.Date < firstDayOfThisMonth.AddMonths(1))
                        .CountAsync();

                    // Lấy số đơn hàng trong tháng trước
                    var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);
                    var lastMonthCount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfLastMonth && x.OrderedAt.Date < firstDayOfThisMonth)
                        .CountAsync();

                    data.Total = thisMonthCount;
                    data.GetBy = "This Month";
                    // Tính toán phần trăm tăng trưởng so với tháng trước
                    if (lastMonthCount > 0)
                    {
                        data.Trend = (int)Math.Round((double)(thisMonthCount - lastMonthCount) / lastMonthCount * 100);
                    }
                    else
                    {
                        data.Trend = 0; 
                    }
                }

                else
                {
                    // Lấy số đơn hàng trong năm hiện tại
                    var firstDayOfThisYear = new DateTime(DateTime.Now.Year,1,1);
                    var thisYearCount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfThisYear && x.OrderedAt.Date < firstDayOfThisYear.AddYears(1))
                        .CountAsync();

                    // Lấy số đơn hàng trong năm trước
                    var firstDayOfLastYear= firstDayOfThisYear.AddYears(-1);
                    var lastYearCount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfLastYear && x.OrderedAt.Date < firstDayOfThisYear)
                        .CountAsync();

                    data.Total = thisYearCount;
                    data.GetBy = "This Year";
                    if (lastYearCount > 0)
                    {
                        data.Trend = (int)Math.Round((double)(thisYearCount - lastYearCount) / lastYearCount * 100);
                    }
                    else
                    {
                        data.Trend = 0; 
                    }
                }
            }
            return data;
        }

        public async Task<StatisticsDTO<decimal>> GetRevenue(string by)
        {
            var data = new StatisticsDTO<decimal>();
            if (by == "Today")
            {
                var todayAmount = await _context.Orders
                    .Where(x => DateOnly.FromDateTime(x.OrderedAt.Date) == DateOnly.FromDateTime(DateTime.Now)
                        && x.Status!= "CANCELED"
                    )
                    .SumAsync(x=>x.TotalAmount);

                var yesterdayAmount = await _context.Orders
                    .Where(x => DateOnly.FromDateTime(x.OrderedAt.Date) == DateOnly.FromDateTime(DateTime.Now.AddDays(-1)) && x.Status != "CANCELED")
                    .SumAsync(x=>x.TotalAmount);
                data.Total = todayAmount;
                data.GetBy = "Today";
                if (yesterdayAmount > 0)
                {
                    data.Trend = (int)Math.Round((todayAmount - yesterdayAmount) / yesterdayAmount * 100);
                }
                else
                {
                    data.Trend = 0;
                }
            }
            else
            {
                if (by == "ThisMonth")
                {
                    var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var thisMonthAmount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfThisMonth && x.OrderedAt.Date < firstDayOfThisMonth.AddMonths(1) && x.Status != "CANCELED")
                        .SumAsync(x=>x.TotalAmount-(x.Discount??0) - (x.DiscountOfCoupon??0));
                    var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);
                    var lastMonthAmount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfLastMonth && x.OrderedAt.Date < firstDayOfThisMonth && x.Status != "CANCELED")
                        .SumAsync(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0));

                    data.Total = thisMonthAmount;
                    data.GetBy = "This Month";
                    if (lastMonthAmount > 0)
                    {
                        data.Trend = (int)Math.Round((thisMonthAmount - lastMonthAmount) / lastMonthAmount * 100);
                    }
                    else
                    {
                        data.Trend = 0;
                    }
                }

                else
                {
                    // Lấy số đơn hàng trong năm hiện tại
                    var firstDayOfThisYear = new DateTime(DateTime.Now.Year, 1, 1);
                    var thisYearAmount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfThisYear && x.OrderedAt.Date < firstDayOfThisYear.AddYears(1) && x.Status != "CANCELED")
                        .SumAsync(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0));

                    // Lấy số đơn hàng trong năm trước
                    var firstDayOfLastYear = firstDayOfThisYear.AddYears(-1);
                    var lastYearAmount = await _context.Orders
                        .Where(x => x.OrderedAt.Date >= firstDayOfLastYear && x.OrderedAt.Date < firstDayOfThisYear && x.Status != "CANCELED")
                        .SumAsync(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0));

                    data.Total = thisYearAmount;
                    data.GetBy = "This Year";
                    if (lastYearAmount > 0)
                    {
                        data.Trend = (int)Math.Round((thisYearAmount - lastYearAmount) / lastYearAmount * 100);
                    }
                    else
                    {
                        data.Trend = 0;
                    }
                }
            }
            return data;
        }

        public async Task<StatisticsDTO<int>> GetCustomer(string by)
        {
            var listUsers = await _context.Users.ToListAsync();
            var listCustomers = new List<ApplicationUser>();
            foreach (var item in listUsers)
            {
                bool isAdmin = await _authService.IsAdmin(item.Id);
                if(!isAdmin) listCustomers.Add(item);
            }

            var data = new StatisticsDTO<int>();
            if (by == "Today")
            {
                var todayCount = listCustomers
                    .Where(x => DateOnly.FromDateTime(x.JoinedAt.Date) == DateOnly.FromDateTime(DateTime.Now)).Count();

                var yesterdayCount = listCustomers
                    .Where(x => DateOnly.FromDateTime(x.JoinedAt.Date) == DateOnly.FromDateTime(DateTime.Now.AddDays(-1))).Count();
                data.Total = todayCount;
                data.GetBy = "Today";
                if (yesterdayCount > 0)
                {
                    data.Trend = (int)Math.Round((double)(todayCount - yesterdayCount) / yesterdayCount * 100);
                }
                else
                {
                    data.Trend = 0;
                }
            }
            else
            {
                if (by == "ThisMonth")
                {
                    var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var thisMonthCount = listCustomers
                        .Where(x => x.JoinedAt.Date >= firstDayOfThisMonth && x.JoinedAt.Date < firstDayOfThisMonth.AddMonths(1))
                        .Count();

                    var firstDayOfLastMonth = firstDayOfThisMonth.AddMonths(-1);
                    var lastMonthCount = listCustomers
                        .Where(x => x.JoinedAt.Date >= firstDayOfLastMonth && x.JoinedAt.Date < firstDayOfThisMonth)
                        .Count();

                    data.Total = thisMonthCount;
                    data.GetBy = "This Month";
                    if (lastMonthCount > 0)
                    {
                        data.Trend = (int)Math.Round((double)(thisMonthCount - lastMonthCount) / lastMonthCount * 100);
                    }
                    else
                    {
                        data.Trend = 0;
                    }
                }

                else
                {
                    var firstDayOfThisYear = new DateTime(DateTime.Now.Year, 1, 1);
                    var thisYearCount = listCustomers
                        .Where(x => x.JoinedAt.Date >= firstDayOfThisYear && x.JoinedAt.Date < firstDayOfThisYear.AddYears(1))
                        .Count();

                    // Lấy số đơn hàng trong năm trước
                    var firstDayOfLastYear = firstDayOfThisYear.AddYears(-1);
                    var lastYearCount = listCustomers
                        .Where(x => x.JoinedAt.Date >= firstDayOfLastYear && x.JoinedAt.Date < firstDayOfThisYear)
                        .Count();

                    data.Total = thisYearCount;
                    data.GetBy = "This year";
                    if (lastYearCount > 0)
                    {
                        data.Trend = (int)Math.Round((double)(thisYearCount - lastYearCount) / lastYearCount * 100);
                    }
                    else
                    {
                        data.Trend = 0;
                    }
                }
            }
            return data;
        }

        public async Task<List<TopSelling>> TopSelling(string by)
        {
            var orderdetail = _context.OrderDetails.AsQueryable();
            orderdetail = orderdetail.Where(x => x.Order.Status != "CANCELED");
            if (by == "day")
            {
                orderdetail = orderdetail.Where(x=>DateOnly.FromDateTime(x.Order.OrderedAt.Date)==DateOnly.FromDateTime(DateTime.Now));
            }
            else
            {
                if(by == "month")
                {
                    var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    orderdetail = orderdetail.Where(x=>x.Order.OrderedAt>=firstDayOfMonth && x.Order.OrderedAt<=DateTime.Now);
                }
                else
                {
                    var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    orderdetail = orderdetail.Where(x => x.Order.OrderedAt >= firstDayOfYear && x.Order.OrderedAt <= DateTime.Now);
                }
            }
            var data = await orderdetail.GroupBy(x => x.ProductID)
               .Select(g => new TopSelling
               {
                   ProductID = g.Key,
                   ProductName = g.First().Product.ProductName,
                   ImageUrl = g.First().Product.MainImage,
                   Price = g.First().UnitPrice,
                   Sold = g.Sum(x => x.Quantity),
                   TotalRevenue = g.Sum(x => x.TotalPrice)
               }).OrderByDescending(x => x.Sold)
               .Take(10).ToListAsync();
            return data;
        }


        public async Task<List<OrderReport>> OrderReports(string by)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            IQueryable<OrderReport> data;

            if (by == "7days")
            {
                data = _context.Orders
                    .Where(o => DateOnly.FromDateTime(o.OrderedAt.Date) >= today.AddDays(-7) && DateOnly.FromDateTime(o.OrderedAt.Date) <= today)
                    .GroupBy(o => o.OrderedAt.Date)
                    .Select(x => new OrderReport
                    {
                        Label = x.Key.ToString("yyyy-MM-dd"), // Chuyển đổi ngày thành chuỗi
                        Data = x.Count()
                    });
            }
            else if (by == "30days")
            {
                data = _context.Orders
                    .Where(o => DateOnly.FromDateTime(o.OrderedAt.Date) >= today.AddDays(-30) && DateOnly.FromDateTime(o.OrderedAt.Date) <= today)
                    .GroupBy(o => o.OrderedAt.Date)
                    .Select(x => new OrderReport
                    {
                        Label = x.Key.ToString("yyyy-MM-dd"), // Chuyển đổi ngày thành chuỗi
                        Data = x.Count()
                    });
            }
            else if (by == "7months")
            {
                data = _context.Orders
                    .Where(o => o.OrderedAt >= new DateTime(today.Year, today.Month, 1).AddMonths(-6) && o.OrderedAt <= DateTime.Now)
                    .GroupBy(o => new { o.OrderedAt.Year, o.OrderedAt.Month })
                    .Select(x => new OrderReport
                    {
                        Label = $"{x.Key.Year}-{x.Key.Month:D2}", // Định dạng năm-tháng
                        Data = x.Count()
                    });
            }
            else
            {
                throw new ArgumentException("Invalid parameter. Use '7day', '30days', or '7months'.");
            }

            return await data.ToListAsync();
        }

        public async Task<List<RevenueReport>> RevenueReports(string by)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            IQueryable<RevenueReport> data;

            if (by == "7days")
            {
                data = _context.Orders
                    .Where(o => DateOnly.FromDateTime(o.OrderedAt.Date) >= today.AddDays(-7) && DateOnly.FromDateTime(o.OrderedAt.Date) <= today && o.Status != "CANCELED")
                    .GroupBy(o => o.OrderedAt.Date)
                    .Select(x => new RevenueReport
                    {
                        Label = x.Key.ToString("yyyy-MM-dd"), 
                        Data = x.Sum(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0))
                    });
            }
            else if (by == "30days")
            {
                data = _context.Orders
                    .Where(o => DateOnly.FromDateTime(o.OrderedAt.Date) >= today.AddDays(-30) && DateOnly.FromDateTime(o.OrderedAt.Date) <= today && o.Status != "CANCELED")
                    .GroupBy(o => o.OrderedAt.Date)
                    .Select(x => new RevenueReport
                    {
                        Label = x.Key.ToString("yyyy-MM-dd"), 
                        Data = x.Sum(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0))
                    });
            }
            else if (by == "7months")
            {
                data = _context.Orders
                    .Where(o => o.OrderedAt >= new DateTime(today.Year, today.Month, 1).AddMonths(-6) && o.OrderedAt <= DateTime.Now && o.Status != "CANCELED")
                    .GroupBy(o => new { o.OrderedAt.Year, o.OrderedAt.Month })
                    .Select(x => new RevenueReport
                    {
                        Label = $"{x.Key.Year}-{x.Key.Month:D2}",
                        Data = x.Sum(x => x.TotalAmount - (x.Discount ?? 0) - (x.DiscountOfCoupon ?? 0))
                    });
            }
            else
            {
                throw new ArgumentException("Invalid parameter. Use '7day', '30days', or '7months'.");
            }
            return await data.ToListAsync();
        }

        public async Task<List<TopSelling>> TopRevenue(string by)
        {
            var orderdetail = _context.OrderDetails.AsQueryable();
            orderdetail = orderdetail.Where(x => x.Order.Status != "CANCELED");
            if (by == "day")
            {
                orderdetail = orderdetail.Where(x => DateOnly.FromDateTime(x.Order.OrderedAt.Date) == DateOnly.FromDateTime(DateTime.Now));
            }
            else
            {
                if (by == "month")
                {
                    var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    orderdetail = orderdetail.Where(x => x.Order.OrderedAt >= firstDayOfMonth && x.Order.OrderedAt <= DateTime.Now);
                }
                else
                {
                    var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                    orderdetail = orderdetail.Where(x => x.Order.OrderedAt >= firstDayOfYear && x.Order.OrderedAt <= DateTime.Now);
                }
            }
            var data = await orderdetail.GroupBy(x => x.ProductID)
               .Select(g => new TopSelling
               {
                   ProductID = g.Key,
                   ProductName = g.First().Product.ProductName,
                   ImageUrl = g.First().Product.MainImage,
                   Price = g.First().UnitPrice,
                   Sold = g.Sum(x => x.Quantity),
                   TotalRevenue = g.Sum(x => x.TotalPrice)
               }).OrderByDescending(x => x.TotalRevenue)
               .Take(10).ToListAsync();
            return data;
        }
    }
}
