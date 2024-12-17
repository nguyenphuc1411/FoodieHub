using FoodieHub.API.Models.DTOs.User;
using FoodieHub.API.Models.QueryModel;
using FoodieHub.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] QueryUserModel query)
        {
            var result = await _service.Get(query);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] CreateUserDTO createUser)
        {
            var result = await _service.Create(createUser);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("disable/{id}")]
        public async Task<IActionResult> Disble([FromRoute] string id)
        {
            var result = await _service.Disable(id);

            return result ? Ok() : BadRequest();
        }
        [Authorize(Roles = "Admin")]
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

        [HttpGet("admins")]
        public async Task<ActionResult> GetAdmin()
        {
            var result = await _service.GetAdmin();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SetRoleDTO setRole)
        {
            var result = await _service.SetRole(setRole);
            return result ? NoContent() : BadRequest();
        }
    }
}
