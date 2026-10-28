
using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;

namespace HospitalAppointmentManagement.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService(DepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Department department)
    {
        ValidateDepartment(department);

        return await _repository.AddAsync(department);
    }

    public Task<List<Department>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Department?> GetByIdAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.");
        }

        return await _repository.GetByIdAsync(departmentId);
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        if (department.DepartmentId <= 0)
        {
            throw new ArgumentException(
                "Enter a valid department ID.");
        }

        ValidateDepartment(department);

        return await _repository.UpdateAsync(department);
    }

    public async Task<bool> DeactivateAsync(int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException(
                "Department ID must be greater than zero.");
        }

        return await _repository.DeactivateAsync(departmentId);
    }

    private static void ValidateDepartment(Department department)
    {
        if (string.IsNullOrWhiteSpace(department.DepartmentName))
        {
            throw new ArgumentException(
                "Department name is required.");
        }

        if (department.DepartmentName.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Department name cannot exceed 100 characters.");
        }

        if (department.Location?.Length > 100)
        {
            throw new ArgumentException(
                "Location cannot exceed 100 characters.");
        }

        department.DepartmentName = department.DepartmentName.Trim();
        department.Location = string.IsNullOrWhiteSpace(department.Location)
            ? null
            : department.Location.Trim();
    }
}
