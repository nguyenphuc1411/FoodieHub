using FoodieHub.API.Models.DTOs.Contact;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : Controller
    {
        private readonly IContactService _service;
        public ContactController(IContactService service)
        {
            _service = service;
        }
        [HttpPost("AddContact")]
        public async Task<IActionResult> AddContact(ContactDTO contact)
        {
            var result = await _service.AddContact(contact);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return StatusCode(result.StatusCode, result);
        }
    }
}
