using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IScreenRepository
{
    Task<List<Screen>> GetAllAsync();

    Task<Screen?> GetByIdAsync(int id);

    Task<List<Screen>> GetByTheatreIdAsync(int theatreId);

    Task<bool> TheatreExistsAsync(int theatreId);

    Task<bool> ScreenNameExistsAsync(
        int theatreId,
        string screenName,
        int? excludeScreenId = null);

    Task AddAsync(Screen screen);

    Task UpdateAsync(Screen screen);

    Task DeleteAsync(Screen screen);
}