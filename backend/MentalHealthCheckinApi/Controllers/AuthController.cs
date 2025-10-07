using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MentalHealthCheckinApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var username = User.Identity?.Name ?? "";
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
            return Ok(new { username, role, userId = id });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // No real "session" to invalidate because Basic Auth is stateless.
            // The frontend should just delete its stored token.
            return Ok(new { message = "Logged out successfully. Please remove stored credentials." });
        }
    }
}
