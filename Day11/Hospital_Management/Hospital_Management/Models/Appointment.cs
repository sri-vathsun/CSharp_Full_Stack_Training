namespace HospitalAppointmentManagement.Models;

public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime BookedAt { get; set; }
    public string Status { get; set; } = "Scheduled";
}