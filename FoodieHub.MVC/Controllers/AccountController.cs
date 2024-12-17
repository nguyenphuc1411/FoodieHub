
using FoodieHub.MVC.Models.Authentication;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Models.User;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FoodieHub.MVC.Models.QueryModel;
using FoodieHub.MVC.Models.Article;


namespace FoodieHub.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IFavoriteService favoriteService;
        private readonly IAuthService authService;
        private readonly IOrderService orderService;
        private readonly IRecipeService recipeService;
        private readonly IUserService userService;
        private readonly IArticleService articleService;
        public AccountController(IConfiguration config,
            IFavoriteService favoriteService,
            IAuthService authService,
            IOrderService orderService,
            IRecipeService recipeService, 
            IUserService userService,
            IArticleService articleService)
        {
            _config = config;
            this.favoriteService = favoriteService;
            this.authService = authService;
            this.orderService = orderService;
            this.recipeService = recipeService;
            this.userService = userService;
            this.articleService = articleService;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (ModelState.IsValid)
            {
               var result = await authService.Login(login);
               if(!result.Success)
               {
                    NotificationHelper.SetErrorNotification(this,result.Message);
                    return View(login);
               }
               else
               {
                    Response.DeleteCookie("TokenAdmin");
                    Response.SetCookie("TokenUser", result.Data.ToString()??throw new Exception("An error"));
                    NotificationHelper.SetSuccessNotification(this,result.Message);
                    return RedirectToAction("Index", "Home");
               }
            }
            return View(login);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (ModelState.IsValid)
            {
               var result = await authService.Register(register);
                if (!result.Success)
                {
                    NotificationHelper.SetErrorNotification(this,result.Message);
                    return View(register);
                }
                else
                {
                    NotificationHelper.SetSuccessNotification(this,result.Message);
                    return RedirectToAction("Login", "Account");
                }               
            }
            return View(register);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            if (ModelState.IsValid)
            {
               var result = await authService.ForgotPassword(forgotPassword);
                if (!result.Success)
                {
                    NotificationHelper.SetErrorNotification(this, result.Message);
                    return View(forgotPassword);
                }
                else
                {
                    NotificationHelper.SetSuccessNotification(this, result.Message);
                    return RedirectToAction("Login", "Account");
                }              
            }
            return View(forgotPassword);
        }
        public IActionResult ResetPassword(string email, string token)
        {
            var data = new ResetPasswordDTO
            {
                Email = email,
                Data = token
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            if (ModelState.IsValid)
            {
                var newRequest = new ResetPasswordDTO
                {
                    Email = resetPassword.Email,
                    Data = resetPassword.Data,
                    NewPassword = resetPassword.NewPassword,
                    ConfirmPassword = resetPassword.ConfirmPassword
                };
                var result = await authService.ResetPassword(resetPassword);
                if (!result)
                {
                    NotificationHelper.SetErrorNotification(this);
                    return View(resetPassword);
                }
                else
                {
                    NotificationHelper.SetSuccessNotification(this);
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(resetPassword);
        }
        public IActionResult LoginGoogle()
        {
            var url = _config["BaseHost"]+"api/" + "auth/login-google";
            return Redirect(url);
        }
        public IActionResult GoogleCallBack(string data)
        {
            var decodedData = WebUtility.UrlDecode(data);
            var jsonObject = System.Text.Json.JsonSerializer.Deserialize<GoogleResponse>(decodedData);
            if (jsonObject != null)
            {
                if (jsonObject.Success)
                {
                    Response.SetCookie("TokenUser", jsonObject.Data);
                    Response.DeleteCookie("TokenAdmin");
                    NotificationHelper.SetSuccessNotification(this,jsonObject.Message);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    NotificationHelper.SetErrorNotification(this, jsonObject.Message);
                }
            }
            else
            {
                NotificationHelper.SetErrorNotification(this, "An error occurred while login");
            }
            return RedirectToAction("Login");
        }
        [ValidateTokenForUser]
        public IActionResult Logout()
        {
            Response.DeleteCookie("TokenUser");
            Response.DeleteCookie("Name");
            Response.DeleteCookie("Avatar");
            NotificationHelper.SetSuccessNotification(this,"Logout successfully");
            return RedirectToAction("Login");
        }
        [ValidateTokenForUser]
        public IActionResult Dashboard()
        {
            return View();
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Order(QueryOrderModel queryOrder)
        {
            var result = await orderService.GetForUser(queryOrder);
            if(result== null)
            {
                NotificationHelper.SetErrorNotification(this);
                return RedirectToAction("Dashboard");
            }
            return View(result);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> OrderDetail(string id)
        {
            var result = await orderService.GetByID(int.Parse(id));
            var userID = Request.GetCookie("UserID");
            if (result == null || result.UserID!=userID)
            {
                NotificationHelper.SetErrorNotification(this,$"Not found order with ID: {id}");
                return RedirectToAction("Order");
            }  
            return View(result);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> CancelOrder(int orderID, string cancellationReason)
        {
            var orderDetails = await orderService.GetByID(orderID);
            if (orderDetails == null)
            {
                NotificationHelper.SetErrorNotification(this, "Not found this order");
                return RedirectToAction("Order");
            }

            if (orderDetails.Status == "PENDING")
            {
                orderDetails.Status = "CANCELED";
            }
            var result = await orderService.ChangeStatus(orderID, orderDetails.Status, cancellationReason);   
            if (result!=null)
            {
                if (result.Success)
                {
                    var userDetail = await authService.GetProfile();
                    if (userDetail == null) return RedirectToAction("Order");
                    if (userDetail.LockoutEnd == null || userDetail.LockoutEnd <= DateTime.UtcNow || userDetail.LockoutEnabled == false)
                    {
                        NotificationHelper.SetSuccessNotification(this, result.Message);
                    }
                    else
                    {
                        Response.DeleteCookie("TokenUser");
                        Response.DeleteCookie("FullName");
                        Response.DeleteCookie("Avatar");
                        NotificationHelper.SetErrorNotification(this, "Your account has been locked for 1 day due to security reasons.");
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    NotificationHelper.SetErrorNotification(this,result.Message);
                }
            }
            else
            {
                NotificationHelper.SetErrorNotification(this, "PLease try again later.");
            }
            return RedirectToAction("Order");
        }

        public async Task<IActionResult> UserInfo(string id)
        {
            var userData = await userService.GetByID(id);
            var recipeData = await recipeService.GetByUser(id);
            if (recipeData != null)
            {
                ViewBag.RecipeData = recipeData.Where(x=>x.IsActive);
            }
            var articleData = await articleService.GetOfUser(id);
            if (articleData != null)
            {
                ViewBag.ArticleData = articleData.Where(x => x.IsActive);
            }
            return View(userData);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Recipes()
        {
            var dataProfile = await authService.GetProfile();
            ViewBag.Profile = dataProfile;
            var dataRecipes = await recipeService.GetOfUser();
            if (dataRecipes == null) return RedirectToAction("Dashboard");
            ViewBag.Recipes = dataRecipes;
            return View();           
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Profile()
        {
            var userDTO = await authService.GetProfile();
            if (userDTO != null)
            {
                var updateProfile = new UpdateProfileDTO
                {
                    Fullname = userDTO.Fullname,
                    Email = userDTO.Email,
                    Avatar = userDTO.Avatar??string.Empty,
                    Bio = userDTO.Bio
                };
                return View(updateProfile);
            }
            NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Profile");
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileDTO user)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.UpdateProfile(user);
                if (result != null)
                {
                    if (result.Success)
                    {
                        NotificationHelper.SetSuccessNotification(this,result.Message);
                        var opt = new CookieOptions
                        {
                            HttpOnly = false,
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.SetCookie("FullName", user.Fullname);
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        NotificationHelper.SetErrorNotification(this, result.Message);
                    }
                }
                else NotificationHelper.SetErrorNotification(this, "Please try again.");
            }
            return View(user);
        }

        [ValidateTokenForUser]
        public async Task<IActionResult> Favorite()
        {
            var dataFR = await favoriteService.GetFR();
            var dataFA = await favoriteService.GetFA();
            return View(new FavoriteViewModel
            {
                FavoriteArticles = dataFA??new List<GetArticleDTO>(),
                FavoriteRecipes = dataFR ?? new List<GetRecipeDTO>()
            });
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavorite(FavoriteDTO favorite)
        {
            bool result = await favoriteService.Delete(favorite);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Favorite");
        }

        [HttpGet("orders/qrcode/{id}")]
        public async Task<IActionResult> OrderQRCode(string id)
        {
            if (!int.TryParse(id, out int orderId))
            {
                NotificationHelper.SetErrorNotification(this, "Invalid order ID");
                return RedirectToAction("Order", "Account");
            }

            var order = await orderService.GetByID(orderId);
            if (order == null)
            {
                NotificationHelper.SetErrorNotification(this, "Order not found");
                return RedirectToAction("Order", "Account");
            }
            return View(order);
        }

        [Route("confirmregister")]
        public async Task<IActionResult> ConfirmRegistion([FromQuery]string email,[FromQuery]string token)
        {
            var request = new ConfirmRegistion
            {
                Email = email,
                Data = token
            };
            var jwtToken = await authService.ConfirmRegistion(request);
            if (string.IsNullOrEmpty(jwtToken))
            {
                NotificationHelper.SetErrorNotification(this, "Failed to verify token");
                return RedirectToAction("Login", "Account");
            }
            else
            {
                NotificationHelper.SetSuccessNotification(this, "Verify token successfully");
                Response.SetCookie("TokenUser", jwtToken);
                return RedirectToAction("Index", "Home");
            }          
        }

        public async Task<IActionResult> Coupon()
        {
            return View();
        }
    }
}
