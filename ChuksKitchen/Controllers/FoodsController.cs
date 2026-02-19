using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodsController : ControllerBase
    {
        private readonly FoodService _service;
        private readonly UserService _userService;

        public FoodsController(FoodService service, UserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var foods = await _service.GetAllAsync();
            return Ok(foods);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdmin([FromQuery] Guid userId)
        {
            // verify user role
            var user = await _userService.GetByIdAsync(userId);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var foods = await _service.GetAllAdminAsync();
            return Ok(foods);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFoodDto dto)
        {
            if (!dto.UserId.HasValue)
                return Forbid();

            var user = await _userService.GetByIdAsync(dto.UserId.Value);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var food = await _service.CreateAsync(dto);
            return Ok(food);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateFoodDto dto, [FromQuery] Guid userId)
        {
            // verify user role
            var user = await _userService.GetByIdAsync(userId);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var food = await _service.UpdateAsync(id, dto);
            if (food == null) return NotFound();
            return Ok(food);
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> SetAvailability(Guid id, [FromQuery] Guid userId, [FromBody] bool available)
        {
            // verify user role
            var user = await _userService.GetByIdAsync(userId);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var food = await _service.SetAvailabilityAsync(id, available);
            if (food == null) return NotFound();
            return Ok(food);
        }
    }
}
