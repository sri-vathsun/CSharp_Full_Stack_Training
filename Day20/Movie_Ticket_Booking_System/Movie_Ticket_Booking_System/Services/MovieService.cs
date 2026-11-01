using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    // CREATE MOVIE
    public async Task<MovieResponse> CreateMovieAsync(
        CreateMovieRequest request)
    {
        var existingMovie =
            await _movieRepository.GetByTitleAsync(request.Title);

        if (existingMovie != null)
        {
            throw new InvalidOperationException(
                "A movie with this title already exists.");
        }

        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var createdMovie =
            await _movieRepository.AddAsync(movie);

        return MapToResponse(createdMovie);
    }

    // GET ALL MOVIES
    public async Task<List<MovieResponse>> GetAllMoviesAsync(
        int pageNumber,
        int pageSize,
        string? title)
    {
        var movies = await _movieRepository.GetAllAsync(
            pageNumber, pageSize, title);

        return movies.Select(MapToResponse).ToList();
    }

    // GET MOVIE BY ID
    public async Task<MovieResponse?> GetMovieByIdAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return null;

        return MapToResponse(movie);
    }

    // UPDATE MOVIE
    public async Task<MovieResponse?> UpdateMovieAsync(
        int id,
        UpdateMovieRequest request)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return null;

        var duplicate =
            await _movieRepository.GetByTitleAsync(request.Title);

        if (duplicate != null && duplicate.MovieId != id)
        {
            throw new InvalidOperationException(
                "Another movie with this title already exists.");
        }

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;

        await _movieRepository.UpdateAsync(movie);

        return MapToResponse(movie);
    }

    // DELETE MOVIE
    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);

        if (movie == null)
            return false;

        await _movieRepository.DeleteAsync(movie);

        return true;
    }

    // ENTITY TO DTO MAPPING
    private static MovieResponse MapToResponse(Movie movie)
    {
        return new MovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationMinutes = movie.DurationMinutes,
            ReleaseDate = movie.ReleaseDate,
            Language = movie.Language,
            Genre = movie.Genre
        };
    }
}