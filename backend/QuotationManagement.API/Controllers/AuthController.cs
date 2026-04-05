using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuotationManagement.API.DTOs;
using QuotationManagement.API.Models;
using QuotationManagement.API.Services;

namespace QuotationManagement.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        private static readonly List<User> Users = new()
        {
            new User { Username = "salesrep", Password = "123", Role = "SalesRep" },
            new User { Username = "salesmanager", Password = "123", Role = "SalesManager" },
            new User { Username = "admin", Password = "123", Role = "Admin" }
        };

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = Users.FirstOrDefault(u =>
                string.Equals(u.Username, dto.Username, StringComparison.Ordinal) &&
                string.Equals(u.Password, dto.Password, StringComparison.Ordinal));

            if (user is null)
            {
                return Unauthorized("Invalid username or password.");
            }

            try
            {
                var token = _authService.GenerateJwtToken(user);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate JWT token for user {Username}", dto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to login right now.");
            }
        }
    }
}
