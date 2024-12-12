using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Helpers;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class OrdersController : Controller
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        public async Task<IActionResult> Index(QueryOrderModel query)
        {
            var result = await orderService.Get(query);
            if(result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index","Home");
            }
            ViewBag.Query = query;
            return View(result);
        }


        public async Task<IActionResult> Details(string id)
        {
            var result = await orderService.GetByID(int.Parse(id));
            if (result == null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Index");
            }
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string orderId, string status)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(status))
            {
                NotificationHelper.SetErrorNotification(this, "Invalid orderID and status");
                return RedirectToAction("Details", new { id = orderId });
            }
            var result = await orderService.ChangeStatusForAdmin(int.Parse(orderId),status);
            if (result!=null && result.Success)
            {
                NotificationHelper.SetSuccessNotification(this, result.Message);
                return RedirectToAction("Index");
            }
            else
            {
                NotificationHelper.SetErrorNotification(this);
            }
            return RedirectToAction("Details", new { id = orderId });
        }
    }
}
