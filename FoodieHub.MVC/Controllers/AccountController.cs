using FoodieHub.API.Models.DTOs.Article;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.Recipe;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Helpers;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;


namespace FoodieHub.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly IFavoriteService favoriteService;
        private readonly IAuthService authService;
        private readonly IOrderService orderService;
        public AccountController(IConfiguration config, IHttpClientFactory httpClientFactory,
            IFavoriteService favoriteService, IAuthService authService, IOrderService orderService)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient("MyAPI");
            this.favoriteService = favoriteService;
            this.authService = authService;
            this.orderService = orderService;
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
            var url = _config["BaseURL"] + "auth/login-google";
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
                    Response.SetCookie("TokenUser", jsonObject.Token);
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
            ViewData["CurrentPage"] = queryOrder.Page;
            ViewData["TotalPages"] = result.TotalPages;
            ViewData["PageSize"] = queryOrder.PageSize;
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
            var response = await _httpClient.GetAsync($"Orders/{orderID}");
            var content = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
            var orderDetails = content?.Data;
            if (orderDetails == null)
            {
                TempData["ErrorMessage"] = content.Message;
                return RedirectToAction("Order");
            }

            if (orderDetails.Status == "PENDING")
            {
                orderDetails.Status = "CANCELED";
            }
            var ChangeStatus = await _httpClient.PatchAsync($"Orders/ChangeStatusUser/{orderID}?status={orderDetails.Status}&cancellationReason={cancellationReason}", null);
            var changeStatusContent = await ChangeStatus.Content.ReadFromJsonAsync<APIResponse>();
            if (changeStatusContent.Success)
            {
                var userresponse = await _httpClient.GetAsync("Auth/profile");
                var usercontent = await userresponse.Content.ReadFromJsonAsync<APIResponse<UserDTO>>();
                var userDetail = usercontent?.Data;
                if (userDetail.LockoutEnd == null || userDetail.LockoutEnd <= DateTime.UtcNow || userDetail.LockoutEnabled == false)
                {
                    TempData["SuccessMessage"] = changeStatusContent.Message;
                }
                else
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    };

                    Response.Cookies.Append("TokenUser", string.Empty, cookieOptions);
                    Response.Cookies.Append("Name", string.Empty, cookieOptions);
                    Response.Cookies.Append("Avatar", string.Empty, cookieOptions);
                    TempData["ErrorMessage"] = "Your account has been locked for 1 day due to security reasons.";
                    return RedirectToAction("Login");
                }
                
            }
            else
            {
                TempData["ErrorMessage"] = changeStatusContent.Message;
            }

            return RedirectToAction("Order");
        }

        public async Task<IActionResult> UserInfo(string id)
        {
            var response = await _httpClient.GetAsync($"users/{id}");
            if (response.IsSuccessStatusCode)
            {
                // Fetching profile data
                var responseProfile = await _httpClient.GetAsync("auth/profile");
                var dataProfile = await responseProfile.Content.ReadFromJsonAsync<APIResponse<JsonElement>>();
                if (dataProfile != null && dataProfile.Success)
                {
                    ViewBag.Profile = dataProfile.Data;
                }
                else
                {
                    ViewBag.Profile = null;
                }


                var data = await response.Content.ReadFromJsonAsync<APIResponse<JsonElement>>();

                // Fetching recipes for the user
                var responseRecipes = await _httpClient.GetAsync($"recipes/user/{id}");
                var dataRecipes = await responseRecipes.Content.ReadFromJsonAsync<APIResponse<List<JsonElement>>>();
                ViewBag.Recipes = dataRecipes?.Success == true ? dataRecipes.Data: new List<JsonElement>() ;

                return View(data.Data);
            }
            TempData["ErrorMessage"] = "User not found";
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Recipes()
        {
            // Fetching profile data
            var responseProfile = await _httpClient.GetAsync("auth/profile");
            var dataProfile = await responseProfile.Content.ReadFromJsonAsync<APIResponse<JsonElement>>();
            if (dataProfile != null && dataProfile.Success)
            {
                // Fetching recipes for the user
                var responseRecipes = await _httpClient.GetAsync($"recipes/user");
                var jsonResponse = await responseRecipes.Content.ReadAsStringAsync();

                // Deserialize JSON thành JsonDocument để xử lý
                var document = JsonDocument.Parse(jsonResponse);
                JsonElement dataRecipes = document.RootElement;

                // Kiểm tra xem response có chứa trường Success không và xử lý
                if (dataRecipes.TryGetProperty("success", out JsonElement successElement) && successElement.GetBoolean())
                {
                    ViewBag.Recipes = dataRecipes.GetProperty("data").EnumerateArray();
                }
                else
                {
                    ViewBag.Recipes = new List<JsonElement>(); // Trả về list rỗng nếu không thành công
                }
             
                return View(dataProfile.Data);
            }
            else
            {
                TempData["ErrorMessage"] = "User not found";
                return Redirect(HttpContext.Request.Headers["Referer"].ToString());
            }        
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> Profile()
        {
            var response = await _httpClient.GetAsync("auth/profile");
            var data = await response.Content.ReadFromJsonAsync<APIResponse<UpdateProfileDTO>>();
            if (data != null)
            {
                if (data.Success)
                {
                    return View(data.Data);
                }
            }
            else
            {
                TempData["ErrorMessgae"] = "Some thing went wrong. Please try again";
            }
            return RedirectToAction("Profile");
        }
        [ValidateTokenForUser]
        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileDTO user)
        {
            if (ModelState.IsValid)
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(user.Fullname), "Fullname");

                    if (!string.IsNullOrEmpty(user.Bio))
                    {
                        content.Add(new StringContent(user.Bio), "Bio");
                    }
                    if (!string.IsNullOrEmpty(user.OldPassword) && !string.IsNullOrEmpty(user.NewPassword))
                    {
                        content.Add(new StringContent(user.OldPassword), "OldPassword");
                        content.Add(new StringContent(user.NewPassword), "NewPassword");
                    }
                    if (user.File != null && user.File.Length > 0)
                    {
                        var fileContent = new StreamContent(user.File.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(user.File.ContentType);
                        content.Add(fileContent, "File", user.File.FileName);
                    }

                    var httpResponse = await _httpClient.PutAsync("auth", content);
                    var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                    if (data != null)
                    {
                        if (data.Success)
                        {
                            TempData["SuccessMessage"] = data.Message;
                            var opt = new CookieOptions
                            {
                                HttpOnly = false,
                                Expires = DateTime.Now.AddDays(30)
                            };
                            Response.Cookies.Append("Name", user.Fullname, opt);
                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = data.Message;
                        }
                    }
                }
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
        public async Task<IActionResult> UnFavorite(int id)
        {
            bool result = await favoriteService.Delete(id);
            if (result) NotificationHelper.SetSuccessNotification(this);
            else NotificationHelper.SetErrorNotification(this);
            return RedirectToAction("Favorite");
        }      

        [HttpGet("orders/qrcode/{id}")]
        public async Task<IActionResult> OrderQRCode(string id)
        {
            var response = await _httpClient.GetAsync("orders/" + id);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
                if (data.Data.PaymentStatus)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(data.Data);
            }
            TempData["ErrorMessage"] = "Not found this order";
            return RedirectToAction("Order", "Account");
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
    }
}
