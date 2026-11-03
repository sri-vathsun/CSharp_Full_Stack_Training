
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;

    public ShowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Show>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        var query = _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Theatre)
            .Include(s => s.ShowSeats)
            .AsQueryable();

        if (date.HasValue)
        {
            var start = date.Value.Date;
            var end = start.AddDays(1);

            query = query.Where(s =>
                s.StartTime >= start &&
                s.StartTime < end);
        }

        if (movieId.HasValue)
            query = query.Where(s => s.MovieId == movieId.Value);

        if (theatreId.HasValue)
        {
            query = query.Where(s =>
                s.Screen.TheatreId == theatreId.Value);
        }

        return await query
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<Show?> GetByIdAsync(int id)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Theatre)
            .Include(s => s.ShowSeats)
            .FirstOrDefaultAsync(s => s.ShowId == id);
    }

    public async Task<List<Show>> GetByMovieIdAsync(int movieId)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Theatre)
            .Include(s => s.ShowSeats)
            .Where(s => s.MovieId == movieId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
    {
        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Theatre)
            .Include(s => s.ShowSeats)
            .Where(s => s.Screen.TheatreId == theatreId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Show>> GetByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _context.Shows
            .Include(s => s.Movie)
            .Include(s => s.Screen)
                .ThenInclude(sc => sc.Theatre)
            .Include(s => s.ShowSeats)
            .Where(s => s.StartTime >= start &&
                        s.StartTime < end)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public Task<bool> MovieExistsAsync(int movieId)
    {
        return _context.Movies.AnyAsync(m => m.MovieId == movieId);
    }

    public Task<bool> ScreenExistsAsync(int screenId)
    {
        return _context.Screens.AnyAsync(s => s.ScreenId == screenId);
    }

    public Task<bool> ShowExistsAsync(int showId)
    {
        return _context.Shows.AnyAsync(s => s.ShowId == showId);
    }

    public async Task<bool> HasOverlappingShowAsync(
        int screenId,
        DateTime startTime,
        DateTime endTime,
        int? excludeShowId = null)
    {
        var query = _context.Shows.Where(s =>
            s.ScreenId == screenId &&
            s.StartTime < endTime &&
            s.EndTime > startTime);

        if (excludeShowId.HasValue)
        {
            query = query.Where(s =>
                s.ShowId != excludeShowId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<List<Seat>> GetSeatsByScreenIdAsync(int screenId)
    {
        return await _context.Seats
            .Where(s => s.ScreenId == screenId)
            .OrderBy(s => s.RowName)
            .ThenBy(s => s.SeatNumber)
            .ToListAsync();
    }

    public async Task AddWithSeatsAsync(
        Show show,
        List<ShowSeat> showSeats)
    {
        // Save the show and its ShowSeats together.
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        await _context.Shows.AddAsync(show);
        await _context.SaveChangesAsync();

        foreach (var showSeat in showSeats)
        {
            showSeat.ShowId = show.ShowId;
        }

        await _context.ShowSeats.AddRangeAsync(showSeats);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }

    public async Task UpdateAsync(Show show)
    {
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(Show show)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var showSeats = await _context.ShowSeats
            .Where(ss => ss.ShowId == show.ShowId)
            .ToListAsync();

        _context.ShowSeats.RemoveRange(showSeats);

        _context.Shows.Remove(show);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }


    public async Task<List<ShowSeat>> GetShowSeatsAsync(int showId)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Where(ss => ss.ShowId == showId)
            .OrderBy(ss => ss.Seat.RowName)
            .ThenBy(ss => ss.Seat.SeatNumber)
            .ToListAsync();
    }
}
