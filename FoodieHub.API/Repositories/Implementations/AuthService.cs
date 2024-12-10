using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace FoodieHub.API.Repositories.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly ImageExtentions _uploadHelper;
        public AuthService(IConfiguration configuration, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, AppDbContext context, IMailService mailService, IHttpContextAccessor contextAccessor, IMapper mapper, ImageExtentions uploadHelper)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _mailService = mailService;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _uploadHelper = uploadHelper;
        }

        public async Task<ServiceResponse> Login(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Your account is not active.",
                        StatusCode = 401
                    };
                }
                var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                if (result.Succeeded)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = $"Login successfully. Welcome, {user.Fullname}",
                        Data = AuthExtentions.GenerateToken(user.Id, _configuration["JWT:Key"]??"", ""),
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to Login. Please try again later.",
                        StatusCode = 400
                    };
                }
            }

            return new ServiceResponse { 
                Success = false, 
                Message = $"No user found with email: {login.Email}", 
                StatusCode = 404 
            };
        }


        public async Task<ServiceResponse> Register(RegisterDTO register)
        {
            var isExist = await _userManager.FindByEmailAsync(register.Email);
            if (isExist != null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "An account with this email already exists",
                    StatusCode = 400
                };
            var newUser = new ApplicationUser
            {
                Fullname = register.FullName,
                Email = register.Email,
                IsActive = false,
                UserName = register.Email
            };
            var result = await _userManager.CreateAsync(newUser, register.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "USER");

                var tokenNew = Guid.NewGuid().ToString();

                var newUserToken = new Token
                {
                    Email = newUser.Email,
                    Data = tokenNew,
                    TokenType = "CONFIRM REGISTION",
                    UserID = newUser.Id
                };
                await _context.Tokens.AddAsync(newUserToken);
                await _context.SaveChangesAsync();

                string link = _configuration["OriginFE"] + "/confirmregister?email="+newUser.Email+"&token="+tokenNew;



                var newMail = new MailRequest
                {
                    ToEmail = register.Email,
                    Subject = "Register account successfully",
                    Body = AuthExtentions.GenerateRegistrationSuccessEmail(newUser.Fullname, link)
                };
                await _mailService.SendEmailAsync(newMail);
                return new ServiceResponse
                {
                    Success = true,
                    Message = "The account has been successfully created. Check email to confirm registion.",
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Fail to createthe account. Please try again later",
                StatusCode = 400
            };
        }

        public async Task<ServiceResponse> AdminLogin(LoginDTO login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Your account has been deactivated. Please contact support for assistance.",
                        StatusCode = 401
                    };
                }
                var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                if (result.Succeeded)
                {
                    bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    if (isAdmin)
                    {
                        return new ServiceResponse
                        {
                            Success = true,
                            Message = $"Login successfully. Welcome {user.Fullname} to Admin Dashboard",
                            Data = AuthExtentions.GenerateToken(user.Id, _configuration["JWT:Key"]??"", "Admin"),
                            StatusCode = 200
                        };
                    }
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "This account not permission to access admin page",
                        StatusCode = 403
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Incorrect password. Please try again",
                        StatusCode = 400
                    };
                }
            }

            return new ServiceResponse { Success = false, Message = "No user found with this email", StatusCode = 404 };
        }

        public async Task<ServiceResponse> RequestForgotPassword(ForgotPasswordDTO forgotPassword)
        {
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (user == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "This email is not exists in system",
                    StatusCode = 400
                };
            // Tạo token mới
            var newToken = AuthExtentions.GenerateToken(user.Id, _configuration["JWT:Key"]??"", "");
            // Tạo resetLink
            string resetPasswordLink = $"{_configuration["OriginFE"]}/account/resetpassword?email={user.Email}&token={newToken}";
            // Check token hệ thống
            var tokenusers = await _context.Tokens.Where(x => x.UserID == user.Id && x.TokenType == "FORGOT PASSWORD").ToListAsync();
            if (tokenusers.Count == 0)
            {
                // Chưa có trong hệ thống và xác nhận gửi mail

                var mailRequest = new MailRequest
                {
                    ToEmail = forgotPassword.Email,
                    Subject = "Reset Password Confirm",
                    Body = AuthExtentions.GenerateForgotPasswordEmailBody(resetPasswordLink, user.Fullname)
                };

                var isSendMailSuccess = await _mailService.SendEmailAsync(mailRequest);

                if (isSendMailSuccess)
                {
                    // Lưu thông tin token vào database
                    var userToken = new Token
                    {
                        Data = newToken,
                        TokenType = "FORGOT PASSWORD",
                        Email = user.Email,
                        UserID = user.Id
                    };
                    _context.Tokens.Add(userToken);
                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        return new ServiceResponse
                        {
                            Success = true,
                            Message = "An email has been successfully sent. Please check your inbox to reset your password",
                            StatusCode = 200
                        };
                    }
                    else
                    {
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = "Something went wrong. Please try again later",
                            StatusCode = 400
                        };
                    }
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to send email, please try again later",
                        StatusCode = 400
                    };
                }
            }
            // Đã có token 

            var newestToken = tokenusers.OrderByDescending(x => x.ExpirationDate).FirstOrDefault();
            if (newestToken.ExpirationDate > DateTime.Now)
            {
                var remainTime =(int)Math.Ceiling((newestToken.ExpirationDate - DateTime.Now).TotalMinutes);
                // Chưa hết hạn token cũ
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"An email has already been sent. Please try again in {remainTime} minutes",
                    StatusCode = 400
                };
            }

            var mailRequest1 = new MailRequest
            {
                ToEmail = forgotPassword.Email,
                Subject = "Reset Password Confirm",
                Body = AuthExtentions.GenerateForgotPasswordEmailBody(resetPasswordLink, user.Fullname)
            };

            var isSendMailSuccess1 = await _mailService.SendEmailAsync(mailRequest1);

            if (isSendMailSuccess1)
            {
                // Lưu thông tin token vào database
                var userToken1 = new Token
                {
                    Data = newToken,
                    TokenType = "FORGOT PASSWORD",
                    Email = user.Email,
                    UserID = user.Id
                };
                _context.Tokens.Add(userToken1);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "An email has been successfully sent. Please check your index to reset your password",
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Something went wrong. Please try again later",
                        StatusCode = 400
                    };
                }
            }
            else
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Failed to sent email. Please try again later",
                    StatusCode = 400
                };
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordDTO resetPassword)
        {
            var userToken = await _context.Tokens.FirstOrDefaultAsync(x => x.TokenType == "FORGOT PASSWORD" 
            && x.ExpirationDate>DateTime.Now && x.Data == resetPassword.Data && x.Email == resetPassword.Email && x.IsUsed == false );

            if (userToken == null) return false;

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            var result = await _userManager.RemovePasswordAsync(user);
            var result2 = await _userManager.AddPasswordAsync(user, resetPassword.NewPassword);
            if (result.Succeeded)
            {
                userToken.IsUsed = true;
                _context.Tokens.Update(userToken);
                await _context.SaveChangesAsync();
                return true;
            }
           return false;
        }

        public async Task<ServiceResponse> ChangePassword(ChangePassword changePassword)
        {
            var user = await GetCurrentUser();
            var result = await _userManager.ChangePasswordAsync(user,changePassword.OldPassword,changePassword.NewPassword);
            if (result.Succeeded)
            {
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Password changed successfully",
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Password change failed. Please try again",
                StatusCode = 400
            };
        }
        public string GetUserID()
        {
            var userID = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userID))
            {
                return string.Empty;
            }
            return userID;
        }

        public async Task<UserDTO?> GetCurrentUserDTO()
        {
            var userID = GetUserID();
            var user = await _userManager.FindByIdAsync(userID);            
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            var userID = GetUserID();
            var user = await _userManager.FindByIdAsync(userID);  
            return user;
        }       
        public async Task<string> GoogleCallback()
        {        
            var result = await _contextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var data = new GoogleResponse();
            if (!result.Succeeded)
            {
                data.Success = false;
                data.Message = "Failed to login with Google";
            }

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
            {
                data.Success = false;
                data.Message = "Not found email";             
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var name = claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var newUser = new ApplicationUser
                {
                    Fullname = name,
                    Email = email,
                    UserName = email,
                    NormalizedEmail = email.ToUpper(),
                    NormalizedUserName = email.ToUpper()
                };
                var result1 = await _userManager.CreateAsync(newUser);
                if (result1.Succeeded)
                {
                    data.Success = true;
                    data.Message = "Login with Google successfully";
                    data.Data = AuthExtentions.GenerateToken(newUser.Id, _configuration["JWT:Key"], "");                 
                }
                else
                {
                    data.Success = false;
                    data.Message = "An error occurred while creating the user";                  
                }
            }
            else
            {
                data.Success = true;
                data.Message = "Login with Google success";
                data.Data = AuthExtentions.GenerateToken(user.Id, _configuration["JWT:Key"], "");
            }

            var jsonData = JsonSerializer.Serialize(data);
            var jsonEncoded = WebUtility.UrlEncode(jsonData);
            string returnUrl = _configuration["OriginFE"] + $"/account/googlecallback?data={jsonEncoded}";
            return returnUrl;
        }

        public async Task<ServiceResponse> UpdateUser(UpdateProfileDTO user)
        {
            var currentID = GetUserID();
            var userInfo = await _userManager.FindByIdAsync(currentID);
            if (userInfo != null)
            {
                userInfo.Fullname = user.Fullname;
                userInfo.Bio = user.Bio;             
                if(user.File != null)
                {
                    var uploadImage = await _uploadHelper.UploadImage(user.File, "Users");
                    if (uploadImage.Success)
                    {
                        if (!string.IsNullOrEmpty(userInfo.Avatar))
                        {
                            _uploadHelper.DeleteImage(userInfo.Avatar);
                        }
                        userInfo.Avatar = uploadImage.FilePath.ToString();
                    }
                }
                if(!string.IsNullOrEmpty(user.OldPassword)&& !string.IsNullOrEmpty(user.NewPassword))
                {
                    var passwordVerificationResult = await _userManager.CheckPasswordAsync(userInfo, user.OldPassword);
                    if (!passwordVerificationResult)
                    {
                        return new ServiceResponse
                        {
                            Success = false,
                            Message = "Password incorrect",
                            StatusCode = 400
                        };
                    }
                    await _userManager.ChangePasswordAsync(userInfo, user.OldPassword, user.NewPassword);
                }

                var result = await _userManager.UpdateAsync(userInfo);
                if (result.Succeeded)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "Update profile successfully",
                        StatusCode = 200
                    };
                }
                else
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Update profile failed",
                        StatusCode = 400
                    };
                }
               
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Not found user",
                StatusCode = 404
            };
        }

        

        public async Task<string> ConfirmRegistion(ConfirmRegistion confirmRegistion)
        {
            var check = await _context.Tokens
                .FirstOrDefaultAsync(x=>x.Email == confirmRegistion.Email && x.Data == confirmRegistion.Data
                && x.TokenType== "CONFIRM REGISTION"&& !x.IsUsed);

            if (check == null) return null;

            var user = await _userManager.FindByEmailAsync(confirmRegistion.Email);

            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var token = AuthExtentions.GenerateToken(user.Id, _configuration["JWT:Key"], "");
                return token;
            }
            return null;
        }

        public async Task<bool> IsAdmin(string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null) return false;
            return await _userManager.IsInRoleAsync(user,"Admin");
        }
    }
}
