using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;

    public ScreenService(IScreenRepository screenRepository)
    {
        _screenRepository = screenRepository;
    }

    public async Task<ScreenResponse> CreateAsync(
        CreateScreenRequest request)
    {
        var screenName = request.ScreenName.Trim();

        if (!await _screenRepository.TheatreExistsAsync(request.TheatreId))
        {
            throw new ArgumentException("Theatre not found.");
        }

        if (await _screenRepository.ScreenNameExistsAsync(
            request.TheatreId, screenName))
        {
            throw new ArgumentException(
                "This screen name already exists in this theatre.");
        }

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = screenName
        };

        await _screenRepository.AddAsync(screen);

        var savedScreen =
            await _screenRepository.GetByIdAsync(screen.ScreenId);

        return MapToResponse(savedScreen!);
    }

    public async Task<List<ScreenResponse>> GetAllAsync()
    {
        var screens = await _screenRepository.GetAllAsync();

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        return screen == null ? null : MapToResponse(screen);
    }

    public async Task<List<ScreenResponse>?> GetByTheatreIdAsync(
        int theatreId)
    {
        if (!await _screenRepository.TheatreExistsAsync(theatreId))
        {
            return null;
        }

        var screens =
            await _screenRepository.GetByTheatreIdAsync(theatreId);

        return screens.Select(MapToResponse).ToList();
    }

    public async Task<ScreenResponse?> UpdateAsync(
        int id,
        UpdateScreenRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        if (screen == null)
        {
            return null;
        }

        var screenName = request.ScreenName.Trim();

        if (!await _screenRepository.TheatreExistsAsync(request.TheatreId))
        {
            throw new ArgumentException("Theatre not found.");
        }

        if (await _screenRepository.ScreenNameExistsAsync(
            request.TheatreId, screenName, id))
        {
            throw new ArgumentException(
                "This screen name already exists in this theatre.");
        }

        screen.TheatreId = request.TheatreId;
        screen.ScreenName = screenName;

        await _screenRepository.UpdateAsync(screen);

        var updatedScreen =
            await _screenRepository.GetByIdAsync(id);

        return MapToResponse(updatedScreen!);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);

        if (screen == null)
        {
            return false;
        }

        await _screenRepository.DeleteAsync(screen);

        return true;
    }

    private static ScreenResponse MapToResponse(Screen screen)
    {
        return new ScreenResponse
        {
            ScreenId = screen.ScreenId,
            TheatreId = screen.TheatreId,
            TheatreName = screen.Theatre.Name,
            ScreenName = screen.ScreenName
        };
    }
}