using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.MVC.Helpers
{
    public static class NotificationHelper
    {
        public static void SetSuccessNotification(Controller controller, string? message = null)
        {
            controller.TempData["SuccessMessage"] = message ?? "Operation completed successfully.";
        }

        public static void SetErrorNotification(Controller controller, string? message = null)
        {
            controller.TempData["ErrorMessage"] = message ?? "An error occurred. Please try again.";
        }
    }
}
