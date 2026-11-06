using Campus_Plcement_Management_Web_Api.Models;

namespace Campus_Plcement_Management_Web_Api.Repositories.Interfaces;

public interface ICollegeRepository
{
    Task<College?> GetByIdAsync(int id);

    Task<College?> GetByCollegeCodeAsync(string collegeCode);

    Task<List<College>> GetAllAsync();

    Task<List<College>> GetPendingAsync();

    Task AddAsync(College college);

    Task UpdateAsync(College college);
}
