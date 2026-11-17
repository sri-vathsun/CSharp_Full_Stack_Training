
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using EmployeeManagement.API.DTOs;
using EmployeeManagement.API.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // EMPLOYEE AND ADMIN LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1. Find user by email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            // 2. Verify password
            var isPasswordValid =
                await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!isPasswordValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });
            }

            // 3. Get user's roles
            var roles = await _userManager.GetRolesAsync(user);

            // 4. Create JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 5. Get JWT settings
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is missing.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // 6. Create signing credentials
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            // 7. Generate JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            // 8. Return token to Angular
            return Ok(new
            {
                token = tokenString,
                email = user.Email,
                roles = roles
            });
        }

        // ADMIN CREATES EMPLOYEE ACCOUNT
        [HttpPost("register-employee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterEmployee(
            RegisterEmployeeDto dto)
        {
            // 1. Check whether email already exists
            var existingUser =
                await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email already registered"
                });
            }

            // 2. Create employee user
            var employee = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = false
            };

            // 3. Save user with hashed password
            var result = await _userManager.CreateAsync(
                employee,
                dto.Password
            );

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            // 4. Assign Employee role
            var roleResult = await _userManager.AddToRoleAsync(
                employee,
                "Employee"
            );

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(employee);

                return StatusCode(500, new
                {
                    message = "Unable to assign employee role"
                });
            }

            return Ok(new
            {
                message = "Employee account created successfully",
                email = employee.Email
            });
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok("AuthController is working!");
        }
    }
}
