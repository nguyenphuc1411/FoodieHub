using FoodieHub.MVC.Models.Request;
using FoodieHub.MVC.Models.Response;
using FoodieHub.MVC.Models.User;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace FoodieHub.MVC.Service.Interfaces
{
    public interface IAccountService
    {
        Task<APIResponse> Login(LoginVM loginVM);

        
    }
}
