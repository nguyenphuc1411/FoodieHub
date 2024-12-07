namespace FoodieHub.MVC.Models.Product.TimeHelper
{
    public static class TimeHelper
    {
        public static string GetTimeAgo(DateTime reviewDate)
        {
            var timeSpan = DateTime.Now - reviewDate;

            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} day(s) ago";

            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hour(s) ago";

            if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minute(s) ago";

            return "Just now"; // Nếu là thời gian rất gần (dưới 1 phút)
        }

    }
}
