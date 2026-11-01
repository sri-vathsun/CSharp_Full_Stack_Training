using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ScreenRepository : IScreenRepository
{
    private readonly ApplicationDbContext _context;

    public ScreenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Screen>> GetAllAsync()
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Screen?> GetByIdAsync(int id)
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .FirstOrDefaultAsync(s => s.ScreenId == id);
    }

    public async Task<List<Screen>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Screens
            .Include(s => s.Theatre)
            .AsNoTracking()
            .Where(s => s.TheatreId == theatreId)
            .ToListAsync();
    }

    public async Task<bool> TheatreExistsAsync(int theatreId)
    {
        return await _context.Theatres
            .AnyAsync(t => t.TheatreId == theatreId);
    }

    public async Task<bool> ScreenNameExistsAsync(
        int theatreId,
        string screenName,
        int? excludeScreenId = null)
    {
        return await _context.Screens.AnyAsync(s =>
            s.TheatreId == theatreId &&
            s.ScreenName == screenName &&
            (!excludeScreenId.HasValue ||
             s.ScreenId != excludeScreenId.Value));
    }

    public async Task AddAsync(Screen screen)
    {
        _context.Screens.Add(screen);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Screen screen)
    {
        _context.Screens.Update(screen);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Screen screen)
    {
        _context.Screens.Remove(screen);
        await _context.SaveChangesAsync();
    }
}