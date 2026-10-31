using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;

namespace HospitalAppointmentManagement.Services;

public class AppointmentService
{
    private readonly AppointmentRepository _repository;
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;

    public AppointmentService(
        AppointmentRepository repository,
        PatientService patientService,
        DoctorService doctorService)
    {
        _repository = repository;
        _patientService = patientService;
        _doctorService = doctorService;
    }

    public async Task<int> BookAsync(Appointment appointment)
    {
        if (appointment.AppointmentDate <= DateTime.Now)
            throw new ArgumentException(
                "Appointment must be scheduled for a future date and time.");

        Patient? patient =
            await _patientService.GetByIdAsync(appointment.PatientId);

        if (patient is null || !patient.IsActive)
            throw new ArgumentException(
                "Patient does not exist or is inactive.");

        Doctor? doctor =
            await _doctorService.GetByIdAsync(appointment.DoctorId);

        if (doctor is null || !doctor.IsActive)
            throw new ArgumentException(
                "Doctor does not exist or is inactive.");

        appointment.Status = "Scheduled";

        return await _repository.AddAsync(appointment);
    }

    public Task<List<Appointment>> GetAllAsync() =>
        _repository.GetAllAsync();

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<Appointment>> GetByPatientAsync(int patientId)
    {
        ValidateId(patientId);
        return await _repository.GetByPatientAsync(patientId);
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        ValidateId(id);

        string[] validStatuses =
        {
            "Scheduled",
            "Completed",
            "Cancelled"
        };

        if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException(
                "Status must be Scheduled, Completed, or Cancelled.");

        Appointment? existing = await _repository.GetByIdAsync(id);

        if (existing is null)
            return false;

        if (existing.Status != "Scheduled")
            throw new ArgumentException(
                "Only scheduled appointments can be updated.");

        return await _repository.UpdateStatusAsync(id, status);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be a positive number.");
    }
}