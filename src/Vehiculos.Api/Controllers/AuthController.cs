using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vehiculos.Api.Contracts;
using Vehiculos.Api.Services;

namespace Vehiculos.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    JwtTokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(error.Code, error.Description);
            return BadRequest(ModelState);
        }
        return StatusCode(StatusCodes.Status201Created, new { user.Id, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !(await signInManager.CheckPasswordSignInAsync(user, request.Password, false)).Succeeded)
            return Unauthorized(new { message = "Invalid credentials." });
        return Ok(new { accessToken = tokenService.Create(user), tokenType = "Bearer", expiresIn = 3600 });
    }
}
