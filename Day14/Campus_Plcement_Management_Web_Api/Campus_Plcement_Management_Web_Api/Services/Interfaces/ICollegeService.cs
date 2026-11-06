
using Campus_Plcement_Management_Web_Api.DTOs.College;

namespace Campus_Plcement_Management_Web_Api.Services.Interfaces;

public interface ICollegeService
{
    Task<CollegeResponseDto> RegisterAsync(
        CreateCollegeDto dto);

    Task<List<CollegeResponseDto>> GetAllAsync();

    Task<List<CollegeResponseDto>> GetPendingAsync();

    Task<CollegeResponseDto> GetByIdAsync(int id);

    Task<CollegeResponseDto> ApproveAsync(int id);

    Task<CollegeResponseDto> RejectAsync(int id);

    Task<CollegeResponseDto> SuspendAsync(int id);

    Task<CollegeResponseDto> UpdateAsync(
        int id,
        UpdateCollegeDto dto);
}
