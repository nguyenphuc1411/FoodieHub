using AutoMapper;
using FoodieHub.API.Data;
using FoodieHub.API.Extentions;
using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Models.Response;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FoodieHub.API.Repositories.Implementations
{
    public class UserService : IUserService
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ImageExtentions _uploadImageHelper;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        public UserService(IAuthService authService, UserManager<ApplicationUser> userManager, IMapper mapper, ImageExtentions uploadImageHelper, IMailService mailService)
        {
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
            _uploadImageHelper = uploadImageHelper;
            _mailService = mailService;
        }

        public async Task<ServiceResponse> Create(CreateUserDTO createUser)
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

        public async Task<bool> Disable(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;
            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (user.Email != null)
                {
                    var newMail = new MailRequest
                    {
                        Subject = "Your account has been teminated.",
                        ToEmail = user.Email,
                        Body = AuthExtentions.GenerateAccountDisabledEmail(user.Fullname, DateTime.Now)
                    };
                    await _mailService.SendEmailAsync(newMail);
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> Restore(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;
            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if(user.Email != null)
                {
                    var newMail = new MailRequest
                    {
                        Subject = "Your account has been restored.",
                        ToEmail = user.Email,
                        Body = AuthExtentions.GenerateAccountRestoredEmail(user.Fullname, DateTime.Now)
                    };
                    await _mailService.SendEmailAsync(newMail);
                    return true;
                }                         
            }
            return false;
        }

        public async Task<PaginatedModel<UserDTO>> Get(QueryUserModel query)
        {
            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(query.Email))
            {
                users = users.Where(x => x.Email.ToLower().Contains(query.Email.ToLower()));
            }
            var userList = await users.ToListAsync();
            var result = new List<UserDTO>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new UserDTO
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Avatar = user.Avatar??string.Empty,
                    Bio = user.Bio ?? string.Empty,
                    JoinedAt = user.JoinedAt,
                    Email = user.Email ?? string.Empty,
                    IsActive = user.IsActive,
                    Role = roles.FirstOrDefault() ?? "No Role",
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                });
            }
            if (!string.IsNullOrEmpty(query.Role))
            {
                result = result.Where(x => x.Role==query.Role).ToList();
            }
            return result.Paginate(query.PageSize,query.Page);
        }

        public async Task<UserDTO?> GetByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAdmin()
        {
            var users = await _userManager.Users.ToListAsync();
            var listAdmin = new List<UserDTO>();
            foreach (var item in users)
            {
                var isAdmin = await _authService.IsAdmin(item.Id);
                if (isAdmin) listAdmin.Add(_mapper.Map<UserDTO>(item));
            }
            return listAdmin;
        }

        public async Task<bool> SetRole(SetRoleDTO setRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(setRoleDTO.UserID);
            if (user == null) return false;
            var currentRoles = await _userManager.GetRolesAsync(user);

            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return false;
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, setRoleDTO.Role);
            if (!addRoleResult.Succeeded)
            {
                return false;
            }

            return true; 
        }

    }
}
