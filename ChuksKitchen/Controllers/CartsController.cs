using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Services;
using ChuksKitchen.Data.Entity;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly CartService _service;

        public CartsController(CartService service)
        {
            _service = service;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(AddCartDto dto)
        {
            try
            {
                var cart = await _service.AddToCartAsync(dto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(Guid userId)
        {
            var cart = await _service.GetCartAsync(userId);
            if (cart == null) return NotFound();
            return Ok(cart);
        }

        [HttpPost("clear/{userId}")]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _service.ClearCartAsync(userId);
            return Ok("Cart cleared");
        }
    }
}
