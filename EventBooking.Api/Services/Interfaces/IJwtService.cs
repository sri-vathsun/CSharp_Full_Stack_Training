using EventBooking.Api.Models;

namespace EventBooking.Api.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}