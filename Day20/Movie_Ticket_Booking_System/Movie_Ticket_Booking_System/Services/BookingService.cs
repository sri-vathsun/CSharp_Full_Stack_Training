
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;

    public BookingService(IBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookingResponse> CreateAsync(
        CreateBookingRequest request,
        int userId,
        string customerName,
        string customerEmail)
    {
        if (request.SeatIds == null || request.SeatIds.Count == 0)
            throw new ArgumentException("Select at least one seat.");

        if (request.SeatIds.Any(id => id <= 0))
            throw new ArgumentException("Seat IDs must be positive.");

        if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
            throw new ArgumentException("Duplicate seat IDs are not allowed.");

        if (!await _repository.ShowExistsAsync(request.ShowId))
            throw new KeyNotFoundException("Show not found.");

        var showSeats = await _repository.GetShowSeatsForBookingAsync(
            request.ShowId,
            request.SeatIds);

        // Ensure every requested seat belongs to this show.
        if (showSeats.Count != request.SeatIds.Count)
            throw new ArgumentException(
                "One or more selected seats do not belong to this show.");

        // Ensure every requested seat is available.
        if (showSeats.Any(ss => ss.Status != "Available"))
            throw new InvalidOperationException(
                "One or more selected seats are already booked.");

        var ticketPrice = showSeats[0].Show.TicketPrice;

        // Calculate price on the server.
        var totalAmount = ticketPrice * showSeats.Count;

        var booking = new Booking
        {
            ShowId = request.ShowId,
            UserId = userId,
            CustomerName = customerName.Trim(),
            CustomerEmail = customerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            BookingDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = "Confirmed",

            BookingSeats = showSeats.Select(ss => new BookingSeat
            {
                SeatId = ss.SeatId,
                Price = ticketPrice
            }).ToList()
        };

        var savedBooking = await _repository.AddWithSeatsAsync(
            booking,
            showSeats);

        return MapToResponse(savedBooking);
    }

    public async Task<List<BookingResponse>> GetAllAsync(
        int? showId = null,
        string? status = null)
    {
        var bookings = await _repository.GetAllAsync(showId, status);

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<BookingResponse?> GetByIdAsync(int id, int? userId = null)
    {
        var booking = await _repository.GetByIdAsync(id, userId);

        return booking == null ? null : MapToResponse(booking);
    }

    public async Task<List<BookingResponse>> GetByShowIdAsync(int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
            throw new KeyNotFoundException("Show not found.");

        var bookings = await _repository.GetByShowIdAsync(showId);

        return bookings.Select(MapToResponse).ToList();
    }

    public async Task<BookingResponse?> CancelAsync(int id, int? userId = null)
    {
        var existingBooking = await _repository.GetByIdAsync(id, userId);

        if (existingBooking == null)
            return null;

        if (existingBooking.Status != "Confirmed")
            throw new InvalidOperationException(
                "Only confirmed bookings can be cancelled.");

        var cancelledBooking = await _repository.CancelAsync(id, userId);

        return cancelledBooking == null
            ? null
            : MapToResponse(cancelledBooking);
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse
        {
            BookingId = booking.BookingId,
            BookingReference = booking.BookingReference,
            ShowId = booking.ShowId,
            CustomerName = booking.CustomerName,
            CustomerEmail = booking.CustomerEmail,
            CustomerPhone = booking.CustomerPhone,
            BookingDate = booking.BookingDate,
            TotalAmount = booking.TotalAmount,
            Status = booking.Status,

            Seats = booking.BookingSeats
                .Select(bs => $"{bs.Seat.RowName}{bs.Seat.SeatNumber}")
                .ToList()
        };
    }
}
