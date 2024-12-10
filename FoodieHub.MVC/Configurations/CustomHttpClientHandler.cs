using FoodieHub.MVC.Helpers;
using System.Net.Http.Headers;

namespace FoodieHub.MVC.Configurations
{
    public class CustomHttpClientHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenAdmin = _httpContextAccessor.HttpContext.Request.GetCookie("TokenAdmin");
            var tokenUser = _httpContextAccessor.HttpContext.Request.GetCookie("TokenUser");
            if (!string.IsNullOrEmpty(tokenUser))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenUser);
            }
            else
            {
                if(!string.IsNullOrEmpty(tokenAdmin))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAdmin);
                }             
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
