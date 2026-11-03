using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email)
    {
        return context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public Task SaveChangesAsync()
    {
        return context.SaveChangesAsync();
    }
}
