using Azure;
using FoodieHub.MVC.Configurations;
using FoodieHub.MVC.Libraries;
using FoodieHub.MVC.Models.Article;
using FoodieHub.MVC.Models.Favorite;
using FoodieHub.MVC.Models.Order;
using FoodieHub.MVC.Models.Recipe;
using FoodieHub.MVC.Models.Request;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace FoodieHub.MVC.Controllers
{
    public class Account : Controller
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        public Account(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient("MyAPI");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("auth/login", login);

                var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                if (data != null)
                {
                    if (data.Success)
                    {
                        TempData["SuccessMessage"] = data.Message;
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            Expires = DateTime.UtcNow.AddDays(30)
                        };
                        var cookieOptions1 = new CookieOptions
                        {
                            HttpOnly = false,
                            Secure = true,
                            Expires = DateTime.UtcNow.AddDays(-1)
                        };
                        Response.Cookies.Append("TokenAdmin",string.Empty, cookieOptions1);
                        Response.Cookies.Append("TokenUser", data.Data.ToString(), cookieOptions);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = data.Message;
                        return View(login);
                    }

                }
                TempData["ErrorMessage"] = "Server Error, please try again!";
                return View(login);
            }
            return View(login);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("auth/register", register);

                var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                if (data != null)
                {
                    if (data.Success)
                    {
                        TempData["SuccessMessage"] = data.Message;
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = data.Message;
                        return View(register);
                    }

                }
                TempData["ErrorMessage"] = "Server Error, please try again!";
                return View(register);
            }
            return View(register);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var httpResponse = await _httpClient.PostAsJsonAsync("auth/request-forgot-password", forgotPassword);

                var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                if (data != null)
                {
                    if (data.Success)
                    {
                        TempData["SuccessMessage"] = data.Message;
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = data.Message;
                        return View(forgotPassword);
                    }

                }
                TempData["ErrorMessage"] = "Server Error, please try again!";
                return View(forgotPassword);
            }
            return View(forgotPassword);
        }
        public IActionResult ResetPassword(string email, string token)
        {
            var data = new ResetPassword
            {
                Email = email,
                Token = token
            };
            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            if (ModelState.IsValid)
            {
                var newRequest = new
                {
                    email = resetPassword.Email,
                    token = resetPassword.Token,
                    newPassword = resetPassword.NewPassword
                };
                var httpResponse = await _httpClient.PostAsJsonAsync("auth/reset-password", newRequest);

                var data = await httpResponse.Content.ReadFromJsonAsync<APIResponse>();
                if (data != null)
                {
                    if (data.Success)
                    {
                        TempData["SuccessMessage"] = data.Message;
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = data.Message;
                        return View(resetPassword);
                    }

                }
                TempData["ErrorMessage"] = "Server Error, please try again!";
                return View(resetPassword);
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
                    TempData["SuccessMessage"] = jsonObject.Message;

                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(7)
                    };
                    var cookieOptions1 = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.UtcNow.AddDays(-1)
                    };

                    Response.Cookies.Append("TokenAdmin", string.Empty, cookieOptions);
                    Response.Cookies.Append("TokenUser", jsonObject.Token, cookieOptions);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = jsonObject.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "occurred while login";
            }
            return RedirectToAction("Login");
        }
        [ValidateTokenForUser]
        public IActionResult Logout()
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
            TempData["SuccessMessage"] = "Logout successfully";
            return RedirectToAction("Login");
        }
        [ValidateTokenForUser]
        public IActionResult Dashboard()
        {
            return View();
        }


        [ValidateTokenForUser]
        public async Task<IActionResult> Order(int? pageSize, int? currentPage)
        {
            pageSize ??= 10;
            currentPage ??= 1;
            var response = await _httpClient.GetAsync($"Orders/ordered?pageSize={pageSize}&currentPage={currentPage}");

            var content = await response.Content.ReadFromJsonAsync<APIResponse<Models.Response.PaginatedResult<GetOrder>>>();
            var orderList = content?.Data.Items;

            int totalOrders = content?.Data.TotalItems ?? 0;
            int totalPages = content?.Data.TotalPages ?? 0;

            ViewData["CurrentPage"] = currentPage;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            if (orderList == null)
            {
                TempData["ErrorMessage"] = content?.Message ?? "No orders found.";
                return RedirectToAction("Index");
            }

            return View(orderList);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> OrderDetail(string id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<APIResponse<GetDetailOrder>>();
                var orderDetails = content?.Data;

                if (orderDetails == null)
                {
                    TempData["ErrorMessage"] = content.Message;
                    return RedirectToAction("Order");
                }

                // Assign possible statuses
               

                return View(orderDetails); // Return view with order details
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to load order details from API.";
                return RedirectToAction("Order");
            }
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

                // Fetching current user's following
                var responseFollowingCurrent = await _httpClient.GetAsync("users/following");
                var dataFollowingCurrent = await responseFollowingCurrent.Content.ReadFromJsonAsync<APIResponse<List<UserDTO>>>();
                ViewBag.FollowingCurrent = dataFollowingCurrent?.Success == true ? dataFollowingCurrent.Data : new List<UserDTO>();

                // Fetching following users
                var responseFollowing = await _httpClient.GetAsync($"users/following/{id}");
                var dataFollowing = await responseFollowing.Content.ReadFromJsonAsync<APIResponse<List<UserDTO>>>();
                ViewBag.Following = (dataFollowing != null && dataFollowing.Success) ? dataFollowing.Data : new List<UserDTO>();

                // Fetching followers
                var responseFollower = await _httpClient.GetAsync($"users/follower/{id}");
                var dataFollower = await responseFollower.Content.ReadFromJsonAsync<APIResponse<List<UserDTO>>>();
                ViewBag.Follower = (dataFollower != null && dataFollower.Success) ? dataFollower.Data : new List<UserDTO>();

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
            var data = await response.Content.ReadFromJsonAsync<APIResponse<UpdateUser>>();
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
        public async Task<IActionResult> Profile(UpdateUser user)
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
            var favoriteArticlesResponse = await _httpClient.GetAsync("Favorites/FavoriteArticle");
            var favoriteArticlesContent = await favoriteArticlesResponse.Content.ReadFromJsonAsync<APIResponse<List<GetArticles>>>();
            var favoriteArticles = favoriteArticlesContent?.Data;

            var favoriteRecipesResponse = await _httpClient.GetAsync("Favorites/FavoriteRecipe");
            var favoriteRecipesContent = await favoriteRecipesResponse.Content.ReadFromJsonAsync<APIResponse<List<GetRecipes>>>();
            var favoriteRecipes = favoriteRecipesContent?.Data;
            if (!favoriteArticlesContent.Success)
            {
                TempData["ErrorMessage"] = "No favorite articles found.";
            }
            if (!favoriteRecipesContent.Success)
            {
                TempData["ErrorMessage"] = "No favorite recipes found.";
            }
            var viewModel = new FavoriteViewModel
            {
                FavoriteArticles = favoriteArticles ?? new List<GetArticles>(),
                FavoriteRecipes = favoriteRecipes ?? new List<GetRecipes>()
            };

            return View(viewModel);
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavoriteArticles(int id)
        {
            var response = await _httpClient.DeleteAsync($"Favorites/unfa/{id}");
            var Content = await response.Content.ReadFromJsonAsync<APIResponse>();

            if (Content.Success)
            {
                TempData["SuccessMessage"] = Content.Message;
            }
            else
            {
                TempData["ErrorMessage"] = Content.Message;
            }

            return RedirectToAction("Favorite");
        }
        [ValidateTokenForUser]
        public async Task<IActionResult> UnFavoriteRecipes(int id)
        {
            var response = await _httpClient.DeleteAsync($"Favorites/unfr/{id}");

            var content = await response.Content.ReadFromJsonAsync<APIResponse>();

            if (content.Success)
            {
                TempData["SuccessMessage"] = content.Message;
            }
            else
            {
                TempData["ErrorMessage"] = content.Message;
            }

            return RedirectToAction("Favorite");
        }


        [ValidateTokenForUser]
        public async Task<IActionResult> Follow(string id)
        {
            var response = await _httpClient.PostAsJsonAsync("users/follow", id);
            var data = await response.Content.ReadFromJsonAsync<APIResponse>();
            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());

        }
        [ValidateTokenForUser]
        public async Task<IActionResult> UnFollow(string id)
        {
            var response = await _httpClient.PostAsJsonAsync("users/unfollow", id);
            var data = await response.Content.ReadFromJsonAsync<APIResponse>();
            if (data != null)
            {
                if (data.Success)
                {
                    TempData["SuccessMessage"] = data.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = data.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
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
            var request = new
            {
                Email = email,
                Token = token
            };

            var response = await _httpClient.PostAsJsonAsync("auth/confirm-registion", request);
            if (response.IsSuccessStatusCode)
            {
                var jwt = await response.Content.ReadAsStringAsync();

                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30)
                };
                Response.Cookies.Append("TokenUser", jwt, cookieOption);

                TempData["SuccessMessage"] = "Verify token successfully";

                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to verify token";
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
