
using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int userId,
        string customerName,
        string customerEmail);

    Task<List<BookingResponse>> GetAllAsync(
        int? showId = null,
        string? status = null);

    Task<BookingResponse?> GetByIdAsync(int id, int? userId = null);

    Task<List<BookingResponse>> GetByShowIdAsync(int showId);

    Task<BookingResponse?> CancelAsync(int id, int? userId = null);
}
