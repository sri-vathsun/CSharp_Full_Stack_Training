using Campus_Plcement_Management_Web_Api.Enums;
using Campus_Plcement_Management_Web_Api.Models;
using Campus_Plcement_Management_Web_Api.Repositories.Interfaces;
using Campus_Plcement_Management_Web_Api.DTOs.College;
using Campus_Plcement_Management_Web_Api.Services.Interfaces;

namespace Campus_Plcement_Management_Web_Api.Services.Implementations;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _repository;

    public CollegeService(ICollegeRepository repository)
    {
        _repository = repository;
    }

    public async Task<CollegeResponseDto> RegisterAsync(CreateCollegeDto dto)
    {
        var existingCollege = await _repository.GetByCollegeCodeAsync(
            dto.CollegeCode.Trim());

        if (existingCollege != null)
        {
            throw new InvalidOperationException(
                "College code already exists.");
        }

        var college = new College
        {
            CollegeName = dto.CollegeName.Trim(),
            CollegeCode = dto.CollegeCode.Trim(),
            UniversityName = dto.UniversityName?.Trim(),
            OfficialEmail = dto.OfficialEmail.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Address = dto.Address.Trim(),
            Website = dto.Website?.Trim(),
            Status = CollegeStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(college);

        return MapToResponse(college);
    }

    public async Task<List<CollegeResponseDto>> GetAllAsync()
    {
        var colleges = await _repository.GetAllAsync();
        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<List<CollegeResponseDto>> GetPendingAsync()
    {
        var colleges = await _repository.GetPendingAsync();
        return colleges.Select(MapToResponse).ToList();
    }

    public async Task<CollegeResponseDto> GetByIdAsync(int id)
    {
        var college = await GetCollegeOrThrow(id);
        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> ApproveAsync(int id)
    {
        var college = await GetCollegeOrThrow(id);

        if (college.Status != CollegeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending colleges can be approved.");
        }

        college.Status = CollegeStatus.Approved;

        await _repository.UpdateAsync(college);

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> RejectAsync(int id)
    {
        var college = await GetCollegeOrThrow(id);

        if (college.Status != CollegeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending colleges can be rejected.");
        }

        college.Status = CollegeStatus.Rejected;

        await _repository.UpdateAsync(college);

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> SuspendAsync(int id)
    {
        var college = await GetCollegeOrThrow(id);

        if (college.Status != CollegeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved colleges can be suspended.");
        }

        college.Status = CollegeStatus.Suspended;

        await _repository.UpdateAsync(college);

        return MapToResponse(college);
    }

    public async Task<CollegeResponseDto> UpdateAsync(
        int id, UpdateCollegeDto dto)
    {
        var college = await GetCollegeOrThrow(id);

        college.CollegeName = dto.CollegeName.Trim();
        college.UniversityName = dto.UniversityName?.Trim();
        college.OfficialEmail = dto.OfficialEmail.Trim();
        college.PhoneNumber = dto.PhoneNumber.Trim();
        college.Address = dto.Address.Trim();
        college.Website = dto.Website?.Trim();

        await _repository.UpdateAsync(college);

        return MapToResponse(college);
    }

    private async Task<College> GetCollegeOrThrow(int id)
    {
        var college = await _repository.GetByIdAsync(id);

        if (college == null)
        {
            throw new KeyNotFoundException(
                $"College with ID {id} was not found.");
        }

        return college;
    }

    private static CollegeResponseDto MapToResponse(College college)
    {
        return new CollegeResponseDto
        {
            CollegeId = college.CollegeId,
            CollegeName = college.CollegeName,
            CollegeCode = college.CollegeCode,
            UniversityName = college.UniversityName,
            OfficialEmail = college.OfficialEmail,
            PhoneNumber = college.PhoneNumber,
            Address = college.Address,
            Website = college.Website,
            Status = college.Status.ToString(),
            CreatedAt = college.CreatedAt
        };
    }
}
