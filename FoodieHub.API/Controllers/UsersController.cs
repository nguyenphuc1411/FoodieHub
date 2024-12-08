using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodieHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] QueryUserModel query)
        {
            var result = await _service.Get(query);
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateUserDTO createUser)
        {
            var result = await _service.Create(createUser);
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Policy = "RequireAdmin")]
        [HttpPatch("disable/{id}")]
        public async Task<IActionResult> Disble([FromRoute] string id)
        {
            var result = await _service.Disable(id);

            return result ? Ok() : BadRequest();
        }
        [Authorize(Policy = "RequireAdmin")]
        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> Restore([FromRoute] string id)
        {
            var result = await _service.Restore(id);
            return result ? Ok() : BadRequest();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetByID([FromRoute] string id)
        {
            var result = await _service.GetByID(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
