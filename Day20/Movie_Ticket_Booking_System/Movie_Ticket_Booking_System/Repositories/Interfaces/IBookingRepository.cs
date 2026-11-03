
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<List<Booking>> GetAllAsync(
        int? showId = null,
        string? status = null);

    Task<Booking?> GetByIdAsync(int id, int? userId = null);

    Task<List<Booking>> GetByShowIdAsync(int showId);

    Task<List<ShowSeat>> GetShowSeatsForBookingAsync(
        int showId,
        List<int> seatIds);

    Task<bool> ShowExistsAsync(int showId);

    Task<Booking> AddWithSeatsAsync(
        Booking booking,
        List<ShowSeat> showSeats);

    Task<Booking?> CancelAsync(int bookingId, int? userId = null);
}
