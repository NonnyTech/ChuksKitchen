using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;
        private readonly UserService _userService;

        public OrdersController(OrderService service, UserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            try
            {
                var order = await _service.CreateAsync(dto);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _service.UpdateStatusAsync(id, dto.Status);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, Guid userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var ok = await _service.CancelAsync(id);
            if (!ok) return NotFound();
            return Ok("Order cancelled");
        }

        [HttpPost("{id}/cancel/customer")]
        public async Task<IActionResult> CustomerCancel(Guid id)
        {
            var ok = await _service.CancelByCustomerAsync(id);
            if (!ok) return BadRequest("Cannot cancel order at this stage");
            return Ok("Order cancelled by customer");
        }
    }
}
