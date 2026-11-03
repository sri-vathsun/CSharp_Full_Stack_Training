
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetAllAsync(
        int? showId = null,
        string? status = null)
    {
        var query = _context.Bookings
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .AsQueryable();

        if (showId.HasValue)
            query = query.Where(b => b.ShowId == showId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(b => b.Status == status);

        return await query
            .AsNoTracking()
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id, int? userId = null)
    {
        var query = _context.Bookings
            .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(b => b.UserId == userId.Value);

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.BookingId == id);
    }

    public async Task<List<Booking>> GetByShowIdAsync(int showId)
    {
        return await GetAllAsync(showId);
    }

    public async Task<bool> ShowExistsAsync(int showId)
    {
        return await _context.Shows
            .AnyAsync(s => s.ShowId == showId);
    }

    public async Task<List<ShowSeat>> GetShowSeatsForBookingAsync(
        int showId,
        List<int> seatIds)
    {
        return await _context.ShowSeats
            .Include(ss => ss.Seat)
            .Include(ss => ss.Show)
            .Where(ss =>
                ss.ShowId == showId &&
                seatIds.Contains(ss.SeatId))
            .ToListAsync();
    }

    public async Task<Booking> AddWithSeatsAsync(
        Booking booking,
        List<ShowSeat> showSeats)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Add booking first to generate BookingId.
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Generate a unique booking reference.
            booking.BookingReference = $"MOV{booking.BookingId}";

            // Mark selected seats as booked.
            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Booked";
            }

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return booking;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Booking?> CancelAsync(int bookingId, int? userId = null)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingSeats)
                .FirstOrDefaultAsync(b =>
                    b.BookingId == bookingId &&
                    (!userId.HasValue || b.UserId == userId.Value));

            if (booking == null)
                return null;

            var seatIds = booking.BookingSeats
                .Select(bs => bs.SeatId)
                .ToList();

            var showSeats = await _context.ShowSeats
                .Where(ss =>
                    ss.ShowId == booking.ShowId &&
                    seatIds.Contains(ss.SeatId))
                .ToListAsync();

            booking.Status = "Cancelled";

            foreach (var showSeat in showSeats)
            {
                showSeat.Status = "Available";
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return booking;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
