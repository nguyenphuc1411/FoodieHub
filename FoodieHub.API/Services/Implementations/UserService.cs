using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Data.Entities;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodieHub.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ImageExtentions _uploadImageHelper;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        public UserService(AppDbContext context, IAuthService authService, UserManager<ApplicationUser> userManager, IMapper mapper, ImageExtentions uploadImageHelper, IMailService mailService)
        {
            _context = context;
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
            _uploadImageHelper = uploadImageHelper;
            _mailService = mailService;
        }

        public async Task<ServiceResponse> Create(CreateUser createUser)
        {
            var isExist = await _userManager.FindByEmailAsync(createUser.Email);
            if (isExist != null)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Email already exists in system",
                    StatusCode = 400
                };
            }
            var newUser = new ApplicationUser
            {
                Fullname = createUser.Fullname,
                UserName = createUser.Email,
                Email = createUser.Email,
                Bio = createUser.Bio,
                IsActive = createUser.IsActive
            };
            if (createUser.File != null)
            {
                var uploadEesult = await _uploadImageHelper.UploadImage(createUser.File, "Avatar");
                if(uploadEesult == null || !uploadEesult.Success)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Failed to upload avatar. Please try again",
                        StatusCode = 400
                    };                    
                }
                newUser.Avatar = uploadEesult.FilePath.ToString();
            }
            var result = await _userManager.CreateAsync(newUser, createUser.Password);

            if (result.Succeeded)
            {              
                var result1 = await _userManager.AddToRoleAsync(newUser, createUser.Role);
                if (result1.Succeeded)
                {
                    return new ServiceResponse
                    {
                        Success = true,
                        Message = "The user has been successfully created",
                        StatusCode = 200
                    };
                }
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Fail to add role user",
                    StatusCode = 400
                };
            };
            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to create user. Please try again",
                StatusCode = 400
            };

        }

        public async Task<ServiceResponse> Disable(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "No user found with given ID",
                    StatusCode = 404
                };
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var newMail = new MailRequest
                {
                    Subject = "Your account has been teminated.",
                    ToEmail = user.Email,
                    Body = GenerateAccountDisabledEmail(user.Fullname, DateTime.Now)
                };
                await _mailService.SendEmailAsync(newMail);
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Disable user success",
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to disable user",
                StatusCode = 400
            };
        }
        public async Task<ServiceResponse> Restore(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new ServiceResponse
                {
                    Success = false,
                    Message = "No user found with given ID",
                    StatusCode = 404
                };
            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var newMail = new MailRequest
                {
                    Subject = "Your account has been restored.",
                    ToEmail = user.Email,
                    Body = GenerateAccountRestoredEmail(user.Fullname, DateTime.Now)
                };
                await _mailService.SendEmailAsync(newMail);
                return new ServiceResponse
                {
                    Success = true,
                    Message = "Restore user success",
                    StatusCode = 200
                };
            }
            return new ServiceResponse
            {
                Success = false,
                Message = "Failed to restore user",
                StatusCode = 400
            };
        }   

        public async Task<ServiceResponse> Get(string? role,string? email, int pageSize, int currentPage)
        {
            var users = await _userManager.Users.ToListAsync();
            var listUser = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                listUser.Add(new UserDTO
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar,
                    Bio = user.Bio,
                    JoinedAt = user.JoinedAt,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault(),
                    LockoutEnabled = user.LockoutEnabled
                });
            }
            if (!string.IsNullOrEmpty(email))
            {
                listUser = listUser.Where(x => x.Email.ToLowerInvariant().Contains(email.ToLowerInvariant())).ToList();
            }
            if (!string.IsNullOrEmpty(role))
            {
                listUser = listUser.Where(x => x.Role == role).ToList();
            }          
            return new ServiceResponse
            {
                Success = true,
                Message = "Get users successfully",
                Data = listUser.Paginate(pageSize,currentPage),
                StatusCode = 200
            };
        }
        public async Task<ServiceResponse> GetByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return new ServiceResponse
            {
                Success = false,
                Message = "Not found user",
                StatusCode = 404
            };
            return new ServiceResponse
            {
                Success = true,
                Data = _mapper.Map<UserDTO>(user),
                StatusCode = 200
            };
        }


        public string GenerateAccountDisabledEmail(string fullname, DateTime disabledDate)
        {
            return $@"
        Dear {fullname},

        We are writing to inform you that your account has been disabled as of {disabledDate:MMMM dd, yyyy}.

        This action was taken to ensure the security of your account or due to a violation of our terms of service. 
        If you believe this was done in error or would like to resolve the issue, please contact our support team at nguyenphuc14112003@gmail.com.

        We apologize for any inconvenience this may cause and appreciate your understanding.

        Best regards,
        The Support Team
    ";
        }

        public string GenerateAccountRestoredEmail(string fullName, DateTime restoredDate)
        {
            return $@"
        Dear {fullName},

        We are pleased to inform you that your account has been successfully restored as of {restoredDate}.

        You can now log in and continue using our services without any restrictions. If you have any questions or encounter any issues, please feel free to contact our support team at support@example.com.

        Thank you for your patience and understanding during this time. We value your trust and are committed to providing you with the best possible experience.

        Best regards,
        The Support Team
    ";
        }


    }
}
