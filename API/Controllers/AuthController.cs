using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.LoginAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ChangePasswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return NotFound();

            var result = await _authService.ForgotPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid");
        }
    }
}
