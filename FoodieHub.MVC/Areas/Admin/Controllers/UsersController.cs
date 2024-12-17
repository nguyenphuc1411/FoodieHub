using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace FoodieHub.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ValidateTokenForAdmin]
    public class UsersController : Controller
    {
       private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(QueryUserModel query)
        {
            var result = await _userService.Get(query);
            if(result == null) return RedirectToAction("Index", "Home");
            ViewBag.Query = query;
            return View(result);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return View(user); 
            }
            var result = await _userService.Create(user);
            if (result.Success)
            {
                NotificationHelper.SetSuccessNotification(this, result.Message);
                return RedirectToAction("Index");
            }
            NotificationHelper.SetErrorNotification(this, result.Message);
            return View(user);
        }
        public IActionResult Edit()
        {
            return View();
        }
        public async Task<IActionResult> Detail(string id)
        {
           var user = await _userService.GetByID(id);
            return View(user);
        }
        public async Task<IActionResult> Disable(string id)
        {
            bool result = await _userService.Disable(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Restore(string id)
        {
            bool result = await _userService.Restore(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SetRole(SetRoleDTO roleDTO)   
        {
            var result = await _userService.SetRole(roleDTO);
            if(result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Index");
        }
    }
}
