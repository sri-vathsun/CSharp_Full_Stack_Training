using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;

namespace HospitalAppointmentManagement.Services;

public class DoctorService
{
    private readonly DoctorRepository _repository;

    public DoctorService(DoctorRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Doctor doctor)
    {
        ValidateDoctor(doctor);

        doctor.DoctorName = doctor.DoctorName.Trim();
        doctor.Specialization = doctor.Specialization.Trim();

        return await _repository.AddAsync(doctor);
    }

    public Task<List<Doctor>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Doctor?> GetByIdAsync(int doctorId)
    {
        ValidateId(doctorId, "doctor");

        return await _repository.GetByIdAsync(doctorId);
    }

    public async Task<List<Doctor>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Doctor name cannot be empty.");
        }

        return await _repository.SearchByNameAsync(name.Trim());
    }

    public async Task<List<Doctor>> GetByDepartmentAsync(
        int departmentId)
    {
        ValidateId(departmentId, "department");

        return await _repository.GetByDepartmentAsync(departmentId);
    }

    public async Task<bool> UpdateAsync(Doctor doctor)
    {
        ValidateId(doctor.DoctorId, "doctor");
        ValidateDoctor(doctor);

        doctor.DoctorName = doctor.DoctorName.Trim();
        doctor.Specialization = doctor.Specialization.Trim();

        return await _repository.UpdateAsync(doctor);
    }

    public async Task<bool> DeactivateAsync(int doctorId)
    {
        ValidateId(doctorId, "doctor");

        return await _repository.DeactivateAsync(doctorId);
    }

    private static void ValidateDoctor(Doctor doctor)
    {
        if (string.IsNullOrWhiteSpace(doctor.DoctorName))
        {
            throw new ArgumentException(
                "Doctor name is required.");
        }

        if (doctor.DoctorName.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Doctor name cannot exceed 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(doctor.Specialization))
        {
            throw new ArgumentException(
                "Specialization is required.");
        }

        if (doctor.Specialization.Trim().Length > 100)
        {
            throw new ArgumentException(
                "Specialization cannot exceed 100 characters.");
        }

        if (doctor.ConsultationFee < 0)
        {
            throw new ArgumentException(
                "Consultation fee cannot be negative.");
        }

        if (doctor.SlotMinutes <= 0)
        {
            throw new ArgumentException(
                "Slot duration must be greater than zero.");
        }

        if (doctor.DepartmentId <= 0)
        {
            throw new ArgumentException(
                "Select a valid department ID.");
        }
    }

    private static void ValidateId(int id, string entity)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                $"Enter a valid positive {entity} ID.");
        }
    }
}