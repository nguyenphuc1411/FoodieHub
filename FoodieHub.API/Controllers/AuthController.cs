using FoodieHub.API.Models.DTOs.Authentication;
using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {           
            var result = await _service.Login(login);
            return StatusCode(result.StatusCode, result);         
        }

        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(LoginDTO login)
        {       
            var result = await _service.AdminLogin(login);
            return StatusCode(result.StatusCode, result);         
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {       
            var result = await _service.Register(register);
            return StatusCode(result.StatusCode, result);        
        }

        [HttpPost("confirm-registion")]
        public async Task<ActionResult> ConfirmRegistion(ConfirmRegistion confirm)
        {
            var result = await _service.ConfirmRegistion(confirm);
            if (result == null) return BadRequest();
            return Ok(result);
        }


        [HttpPost("request-forgot-password")]
        public async Task<IActionResult> RequestForgotPassword(ForgotPasswordDTO model)
        {         
            var result = await _service.RequestForgotPassword(model);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {       
            var result = await _service.ResetPassword(resetPassword);
            return StatusCode(result.StatusCode, result);         
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await _service.GetProfile();
            return StatusCode(result.StatusCode, result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update(UpdateUser user)
        {
            var result = await _service.UpdateUser(user);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {          
            var result = await _service.ChangePassword(changePassword);
            return StatusCode(result.StatusCode, result);        
        }

        [HttpGet("login-google")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleLoginCallback), "Auth", "google-response");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
       
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            string result = await _service.GoogleCallback();     
            return Redirect(result);
        }

        [HttpGet("getcurrentuser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _service.GetCurrentUser();
            return Ok(result);
        }
    }
}
