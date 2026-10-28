using Hospital_Management.Models;
using Hospital_Management.Repositories;

namespace Hospital_Management.Services;

public class PatientService
{
    private readonly PatientRepository _repository;

    public PatientService(PatientRepository repository)
    {
        _repository = repository;
    }

    public void RegisterPatient(Patient patient)
    {
        ValidatePatient(patient);
        _repository.AddPatient(patient);
    }

    public List<Patient> GetAllPatients()
    {
        return _repository.GetAllPatients();
    }

    public Patient? GetPatientById(int patientId)
    {
        if (patientId <= 0)
            throw new ArgumentException("Invalid patient ID.");

        return _repository.GetPatientById(patientId);
    }

    public List<Patient> SearchPatientsByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Patient name is required.");

        return _repository.SearchPatientsByName(name.Trim());
    }

    public List<Patient> GetPatientsByDepartment(int departmentId)
    {
        if (departmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        return _repository.GetPatientsByDepartment(departmentId);
    }

    public bool UpdatePatient(Patient patient)
    {
        if (patient.PatientId <= 0)
            throw new ArgumentException("Invalid patient ID.");

        ValidatePatient(patient);

        return _repository.UpdatePatient(patient);
    }

    public bool DeactivatePatient(int patientId)
    {
        if (patientId <= 0)
            throw new ArgumentException("Invalid patient ID.");

        return _repository.DeactivatePatient(patientId);
    }

    private static void ValidatePatient(Patient patient)
    {
        patient.Name = patient.Name.Trim();
        patient.Email = patient.Email.Trim();

        if (string.IsNullOrWhiteSpace(patient.Name))
            throw new ArgumentException("Patient name is required.");

        if (string.IsNullOrWhiteSpace(patient.Email) ||
            !System.Net.Mail.MailAddress.TryCreate(
                patient.Email, out var address) ||
            address.Address != patient.Email)
        {
            throw new ArgumentException("Enter a valid email address.");
        }

        if (patient.Age < 0 || patient.Age > 120)
            throw new ArgumentException("Age must be between 0 and 120.");

        if (patient.DepartmentId <= 0)
            throw new ArgumentException("Select a valid department.");
    }
}