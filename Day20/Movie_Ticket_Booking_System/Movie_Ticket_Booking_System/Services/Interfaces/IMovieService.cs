using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<MovieResponse> CreateMovieAsync(
        CreateMovieRequest request);

    Task<List<MovieResponse>> GetAllMoviesAsync(
        int pageNumber,
        int pageSize,
        string? title);

    Task<MovieResponse?> GetMovieByIdAsync(int id);

    Task<MovieResponse?> UpdateMovieAsync(
        int id,
        UpdateMovieRequest request);

    Task<bool> DeleteMovieAsync(int id);
}