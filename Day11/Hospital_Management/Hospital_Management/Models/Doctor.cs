namespace HospitalAppointmentManagement.Models;

public class Doctor
{
    public int DoctorId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string Specialization { get; set; } = string.Empty;

    public decimal ConsultationFee { get; set; }

    public int SlotMinutes { get; set; }

    public int DepartmentId { get; set; }

    public bool IsActive { get; set; } = true;
}