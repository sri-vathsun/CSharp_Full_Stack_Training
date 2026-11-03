using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class UserService(
    IUserRepository repo,
    IPasswordHasher<User> passwordHasher,
    IJwtService jwtService) : IUserService
{
    public async Task<(bool Success, string Message)> RegisterAsync(
        RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        if (await repo.GetByEmailAsync(email) is not null)
            return (false, "Email is already registered.");

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            Role = UserRoles.Customer
        };

        user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
        await repo.AddAsync(user);
        await repo.SaveChangesAsync();

        return (true, "Registration successful.");
    }

    public async Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await repo.GetByEmailAsync(email);
        if (user is null)
            return (false, "Invalid email or password.", null);

        var verification = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);
        if (verification == PasswordVerificationResult.Failed)
            return (false, "Invalid email or password.", null);

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            await repo.SaveChangesAsync();
        }

        return (true, "Login successful.", jwtService.GenerateToken(user));
    }
}