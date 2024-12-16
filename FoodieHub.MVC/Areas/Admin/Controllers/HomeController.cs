using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Areas.Admin.Models;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Models.Contact;
using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.MVC.Service.Interfaces;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.QueryModel;
namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService authService;
        public HomeController(IHttpClientFactory httpClientFactory, IAuthService authService)
        {
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            this.authService = authService;
        }

        public async Task<IActionResult> Index(
            string order="Today",
            string revenue = "ThisMonth",
            string customer= "ThisMonth", 
            string topsellingby="day",
             string toprevenueby = "day",
            string orderreport = "7days",
            string revenuereport = "7days"
            )
        {         
            var responseOrder = await _httpClient.GetAsync("statistics/order"+"?by="+order);
            if (responseOrder.IsSuccessStatusCode)
            {
                var data = await responseOrder.Content.ReadFromJsonAsync<Statistics<int>>();
                ViewBag.DataOrder = data;
            }
            var responseRevenue = await _httpClient.GetAsync("statistics/revenue" + "?by=" + revenue);
            if (responseRevenue.IsSuccessStatusCode)
            {
                var data = await responseRevenue.Content.ReadFromJsonAsync<Statistics<decimal>>();
                ViewBag.DataRevenue = data;
            }
            var responseCustomer = await _httpClient.GetAsync("statistics/customer" + "?by=" + customer);
            if (responseCustomer.IsSuccessStatusCode)
            {
                var data = await responseCustomer.Content.ReadFromJsonAsync<Statistics<int>>();
                ViewBag.DataCustomer = data;
            }

            var queryOrder = new QueryOrderModel { SortBy = "OrderedAt" };
            var responseRecent = await _httpClient.GetAsync("orders");
            if (responseRecent.IsSuccessStatusCode)
            {

                var data = await responseRecent.Content.ReadFromJsonAsync<PaginatedModel<GetDetailOrder>>();
                ViewBag.RecentlyOrder = data?.Items;
            }

            var topSelling = await _httpClient.GetAsync("statistics/topselling?by="+topsellingby);
            if (topSelling.IsSuccessStatusCode)
            {
                var data = await topSelling.Content.ReadFromJsonAsync<List<TopSelling>>();
                ViewBag.TopSelling = data;
                if (topsellingby == "day")
                {
                    ViewBag.topsellingby = "Today";
                }
                else
                {
                    if (topsellingby == "month")
                    {
                        ViewBag.topsellingby = "This Month";
                    }
                    else
                    {
                        ViewBag.topsellingby = "This Year";
                    }                   
                }              
            }

            var topRevenue = await _httpClient.GetAsync("statistics/toprevenue?by=" + toprevenueby);
            if (topRevenue.IsSuccessStatusCode)
            {
                var data = await topRevenue.Content.ReadFromJsonAsync<List<TopSelling>>();
                ViewBag.TopRevenue = data;
                if (toprevenueby == "day")
                {
                    ViewBag.toprevenueby = "Today";
                }
                else
                {
                    if (toprevenueby == "month")
                    {
                        ViewBag.toprevenueby = "This Month";
                    }
                    else
                    {
                        ViewBag.toprevenueby = "This Year";
                    }
                }
            }


            var orderReports = await _httpClient.GetAsync("statistics/orderreports?by=" + orderreport);
            if (orderReports.IsSuccessStatusCode)
            {
                var data = await orderReports.Content.ReadFromJsonAsync<List<OrderReport>>();
                ViewBag.OrderReportData = data;
                if (orderreport == "7days")
                {
                    ViewBag.orderreport = "7 Days";
                }
                else
                {
                    if (orderreport == "30days")
                    {
                        ViewBag.orderreport = "30 Days";
                    }
                    else
                    {
                        ViewBag.orderreport = "7 Month";
                    }
                }
            }

            var revenueReports = await _httpClient.GetAsync("statistics/revenuereports?by=" + revenuereport);
            if (revenueReports.IsSuccessStatusCode)
            {
                var data = await revenueReports.Content.ReadFromJsonAsync<List<RevenueReport>>();
                ViewBag.RevenueReportData = data;
                if (revenuereport == "7days")
                {
                    ViewBag.revenuereport = "7 Days";
                }
                else
                {
                    if (revenuereport == "30days")
                    {
                        ViewBag.revenuereport = "30 Days";
                    }
                    else
                    {
                        ViewBag.revenuereport = "7 Month";
                    }
                }
            }

            await GetContactInfo();

            var profile = await authService.GetProfile();
            Response.SetCookie("FullNameAdmin", profile?.Fullname ??"");
            Response.SetCookie("AvatarAdmin",profile?.Avatar ??"");
            return View();
        }

        private async Task GetContactInfo()
        {
            var responseContact = await _httpClient.GetAsync("Contacts");
            
            if (responseContact.IsSuccessStatusCode)
            {
                var contentContact = await responseContact.Content.ReadFromJsonAsync<APIResponse<List<GetContact>>>();
                ViewBag.ContactInfo = contentContact.Data;
            }
        }

        [HttpPut]
        public async Task<IActionResult> ToggleIsRead(int id)
        {
            var response = await _httpClient.PutAsync($"Contacts/ToggleIsRead/{id}", null);
            var content = await response.Content.ReadFromJsonAsync<APIResponse>();
            if (content.Success)
            {
                TempData["SuccessMessage"] = content.Message;
            }
            else
            {
                TempData["ErrorMessage"] = content.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
