using MovieTicketBooking.DTOs.Theatres;

namespace MovieTicketBooking.Services.Interfaces;

public interface ITheatreService
{
    Task<TheatreResponse> CreateTheatreAsync(CreateTheatreRequest request);

    Task<List<TheatreResponse>> GetAllTheatresAsync();

    Task<TheatreResponse?> GetTheatreByIdAsync(int id);

    Task<TheatreResponse?> UpdateTheatreAsync(
        int id,
        UpdateTheatreRequest request);

    Task<bool> DeleteTheatreAsync(int id);
}