namespace FoodieHub.MVC.Helpers
{
    public static class QueryStringHelper
    {
        public static string ToQueryString(this object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.GetValue(obj, null)?.ToString() ?? "")}";

            return properties.Any() ? "?" + string.Join("&", properties) : string.Empty;
        }
    }

}
