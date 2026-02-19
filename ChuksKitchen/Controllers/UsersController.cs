using System;
using System.Text;
using System.Security.Cryptography;
using ChuksKitchen.Business.Dtos.Request;
using ChuksKitchen.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChuksKitchen.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            try
            {
                var user = await _userService.SignupAsync(dto);
                return Ok(new { Message = "User created", OTP = user.OtpCode });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(VerifyDto dto)
        {
            var ok = await _userService.VerifyAsync(dto);
            if (!ok) return BadRequest("Verification failed");
            return Ok("Account verified");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmailOrPhone) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email/phone and password are required");
            var user = await _userService.LoginAsync(dto.EmailOrPhone!, dto.Password!);
            if (user == null) return Unauthorized("Invalid credentials");

            return Ok(new { Message = "Login successful", UserId = user.Id });
        }
    }
}
