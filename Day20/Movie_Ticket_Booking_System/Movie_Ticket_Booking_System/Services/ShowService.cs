
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _repository;

    public ShowService(IShowRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShowResponse> CreateAsync(
        CreateShowRequest request)
    {
        ValidateTimeAndPrice(
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        if (!await _repository.MovieExistsAsync(request.MovieId))
            throw new KeyNotFoundException("Movie not found.");

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
            throw new KeyNotFoundException("Screen not found.");

        bool overlap = await _repository.HasOverlappingShowAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime);

        if (overlap)
            throw new ArgumentException(
                "This screen already has a show during the selected time.");

        var seats = await _repository.GetSeatsByScreenIdAsync(
            request.ScreenId);

        if (seats.Count == 0)
            throw new ArgumentException(
                "Cannot create a show because this screen has no seats.");

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        var showSeats = seats.Select(seat => new ShowSeat
        {
            SeatId = seat.SeatId,
            Status = "Available"
        }).ToList();

        await _repository.AddWithSeatsAsync(show, showSeats);

        var savedShow = await _repository.GetByIdAsync(show.ShowId);

        return MapToResponse(savedShow!);
    }

    public async Task<List<ShowResponse>> GetAllAsync(
        DateTime? date,
        int? movieId,
        int? theatreId)
    {
        var shows = await _repository.GetAllAsync(
            date, movieId, theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> GetByIdAsync(int id)
    {
        var show = await _repository.GetByIdAsync(id);

        return show == null ? null : MapToResponse(show);
    }

    public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
    {
        var shows = await _repository.GetByMovieIdAsync(movieId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> GetByTheatreIdAsync(
        int theatreId)
    {
        var shows = await _repository.GetByTheatreIdAsync(theatreId);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<List<ShowResponse>> GetByDateAsync(DateTime date)
    {
        var shows = await _repository.GetByDateAsync(date);

        return shows.Select(MapToResponse).ToList();
    }

    public async Task<ShowResponse?> UpdateAsync(
        int id,
        UpdateShowRequest request)
    {
        ValidateTimeAndPrice(
            request.StartTime,
            request.EndTime,
            request.TicketPrice);

        var show = await _repository.GetByIdAsync(id);

        if (show == null)
            return null;

        if (!await _repository.MovieExistsAsync(request.MovieId))
            throw new KeyNotFoundException("Movie not found.");

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
            throw new KeyNotFoundException("Screen not found.");

        // Do not allow changing a show after booking records exist.
        // Booking support is implemented in the next phase.
        // For now, the ShowSeat records are preserved.

        bool overlap = await _repository.HasOverlappingShowAsync(
            request.ScreenId,
            request.StartTime,
            request.EndTime,
            id);

        if (overlap)
            throw new ArgumentException(
                "This screen already has a show during the selected time.");

        if (request.ScreenId != show.ScreenId)
        {
            var newScreenSeats =
                await _repository.GetSeatsByScreenIdAsync(request.ScreenId);

            if (newScreenSeats.Count == 0)
                throw new ArgumentException(
                    "The selected screen has no seats.");

            // Do not change screen when seat availability has already
            // been created. A future rescheduling workflow can handle it.
            throw new ArgumentException(
                "Changing the screen of an existing show is not supported. Create a new show instead.");
        }

        show.MovieId = request.MovieId;
        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.TicketPrice = request.TicketPrice;

        await _repository.UpdateAsync(show);

        return MapToResponse(show);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var show = await _repository.GetByIdAsync(id);

        if (show == null)
            return false;

        await _repository.DeleteAsync(show);

        return true;
    }

    public async Task<List<ShowSeatResponse>?> GetShowSeatsAsync(
        int showId)
    {
        if (!await _repository.ShowExistsAsync(showId))
            return null;

        var seats = await _repository.GetShowSeatsAsync(showId);

        var show = await _repository.GetByIdAsync(showId);

        return seats.Select(seat =>
            MapSeatToResponse(seat, show!.TicketPrice)).ToList();
    }

    public async Task<List<ShowSeatResponse>?> GetAvailableSeatsAsync(
        int showId)
    {
        var seats = await GetShowSeatsAsync(showId);

        return seats?.Where(s => s.Status == "Available").ToList();
    }

    public async Task<List<ShowSeatResponse>?> GetBookedSeatsAsync(
        int showId)
    {
        var seats = await GetShowSeatsAsync(showId);

        return seats?.Where(s => s.Status == "Booked").ToList();
    }

    private static void ValidateTimeAndPrice(
        DateTime startTime,
        DateTime endTime,
        decimal ticketPrice)
    {
        if (endTime <= startTime)
            throw new ArgumentException(
                "End time must be later than start time.");

        if (ticketPrice < 0)
            throw new ArgumentException(
                "Ticket price cannot be negative.");
    }

    private static ShowResponse MapToResponse(Show show)
    {
        return new ShowResponse
        {
            ShowId = show.ShowId,
            MovieId = show.MovieId,
            MovieTitle = show.Movie.Title,
            ScreenId = show.ScreenId,
            ScreenName = show.Screen.ScreenName,
            TheatreId = show.Screen.TheatreId,
            TheatreName = show.Screen.Theatre.Name,
            StartTime = show.StartTime,
            EndTime = show.EndTime,
            TicketPrice = show.TicketPrice,
            TotalSeats = show.ShowSeats.Count
        };
    }

    private static ShowSeatResponse MapSeatToResponse(
        ShowSeat showSeat,
        decimal ticketPrice)
    {
        return new ShowSeatResponse
        {
            SeatId = showSeat.SeatId,
            RowName = showSeat.Seat.RowName,
            SeatNumber = showSeat.Seat.SeatNumber,
            SeatLabel = $"{showSeat.Seat.RowName}{showSeat.Seat.SeatNumber}",
            SeatType = showSeat.Seat.SeatType,
            Price = ticketPrice,
            Status = showSeat.Status
        };
    }
}
