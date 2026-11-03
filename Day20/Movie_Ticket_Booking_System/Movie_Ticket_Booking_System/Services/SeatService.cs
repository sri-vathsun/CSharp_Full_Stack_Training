
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _repository;

    public SeatService(ISeatRepository repository)
    {
        _repository = repository;
    }

    public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
    {
        var rowName = request.RowName.Trim().ToUpperInvariant();
        var seatType = request.SeatType.Trim();

        if (string.IsNullOrWhiteSpace(rowName) ||
            string.IsNullOrWhiteSpace(seatType))
            throw new ArgumentException("Row name and seat type are required.");

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
            throw new KeyNotFoundException("Screen not found.");

        if (await _repository.SeatPositionExistsAsync(
            request.ScreenId, rowName, request.SeatNumber))
            throw new ArgumentException("This seat position already exists.");

        var seat = new Seat
        {
            ScreenId = request.ScreenId,
            RowName = rowName,
            SeatNumber = request.SeatNumber,
            SeatType = seatType
        };

        await _repository.AddAsync(seat);

        return MapToResponse(seat);
    }

    public async Task<List<SeatResponse>> BulkCreateAsync(
        BulkCreateSeatsRequest request)
    {
        if (!await _repository.ScreenExistsAsync(request.ScreenId))
            throw new KeyNotFoundException("Screen not found.");

        if (request.Seats == null || request.Seats.Count == 0)
            throw new ArgumentException("At least one seat is required.");

        var seatsToCreate = new List<Seat>();
        var positions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in request.Seats)
        {
            var rowName = item.RowName.Trim().ToUpperInvariant();
            var seatType = item.SeatType.Trim();

            if (string.IsNullOrWhiteSpace(rowName) ||
                string.IsNullOrWhiteSpace(seatType))
                throw new ArgumentException("Row name and seat type are required.");

            var position = $"{rowName}-{item.SeatNumber}";

            if (!positions.Add(position))
                throw new ArgumentException(
                    $"Duplicate seat in request: {rowName}{item.SeatNumber}.");

            if (await _repository.SeatPositionExistsAsync(
                request.ScreenId, rowName, item.SeatNumber))
                throw new ArgumentException(
                    $"Seat {rowName}{item.SeatNumber} already exists.");

            seatsToCreate.Add(new Seat
            {
                ScreenId = request.ScreenId,
                RowName = rowName,
                SeatNumber = item.SeatNumber,
                SeatType = seatType
            });
        }

        await _repository.AddRangeAsync(seatsToCreate);

        return seatsToCreate.Select(MapToResponse).ToList();
    }

    public async Task<List<SeatResponse>?> GetByScreenIdAsync(int screenId)
    {
        if (!await _repository.ScreenExistsAsync(screenId))
            return null;

        var seats = await _repository.GetByScreenIdAsync(screenId);

        return seats.Select(MapToResponse).ToList();
    }

    public async Task<SeatResponse?> GetByIdAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        return seat == null ? null : MapToResponse(seat);
    }

    public async Task<SeatResponse?> UpdateAsync(
        int id,
        UpdateSeatRequest request)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
            return null;

        var rowName = request.RowName.Trim().ToUpperInvariant();
        var seatType = request.SeatType.Trim();

        if (string.IsNullOrWhiteSpace(rowName) ||
            string.IsNullOrWhiteSpace(seatType))
            throw new ArgumentException("Row name and seat type are required.");

        if (!await _repository.ScreenExistsAsync(request.ScreenId))
            throw new KeyNotFoundException("Screen not found.");

        if (await _repository.SeatPositionExistsAsync(
            request.ScreenId,
            rowName,
            request.SeatNumber,
            id))
            throw new ArgumentException("This seat position already exists.");

        seat.ScreenId = request.ScreenId;
        seat.RowName = rowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = seatType;

        await _repository.UpdateAsync(seat);

        var updatedSeat = await _repository.GetByIdAsync(id);

        return updatedSeat == null ? null : MapToResponse(updatedSeat);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var seat = await _repository.GetByIdAsync(id);

        if (seat == null)
            return false;

        await _repository.DeleteAsync(seat);

        return true;
    }

    private static SeatResponse MapToResponse(Seat seat)
    {
        return new SeatResponse
        {
            SeatId = seat.SeatId,
            ScreenId = seat.ScreenId,
            ScreenName = seat.Screen.ScreenName,
            RowName = seat.RowName,
            SeatNumber = seat.SeatNumber,
            SeatLabel = $"{seat.RowName}{seat.SeatNumber}",
            SeatType = seat.SeatType
        };
    }
}
