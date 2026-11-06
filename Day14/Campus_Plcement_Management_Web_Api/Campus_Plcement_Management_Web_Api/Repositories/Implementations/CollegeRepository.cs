using Campus_Plcement_Management_Web_Api.Data;
using Campus_Plcement_Management_Web_Api.Enums;
using Campus_Plcement_Management_Web_Api.Models;
using Campus_Plcement_Management_Web_Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacement.API.Repositories.Implementations;

public class CollegeRepository : ICollegeRepository
{
    private readonly ApplicationDbContext _context;

    public CollegeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<College?> GetByIdAsync(int id)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c => c.CollegeId == id);
    }

    public async Task<College?> GetByCollegeCodeAsync(string collegeCode)
    {
        return await _context.Colleges
            .FirstOrDefaultAsync(c =>
                c.CollegeCode == collegeCode);
    }

    public async Task<List<College>> GetAllAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<College>> GetPendingAsync()
    {
        return await _context.Colleges
            .AsNoTracking()
            .Where(c => c.Status == CollegeStatus.Pending)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(College college)
    {
        await _context.Colleges.AddAsync(college);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(College college)
    {
        _context.Colleges.Update(college);

        await _context.SaveChangesAsync();
    }
}
