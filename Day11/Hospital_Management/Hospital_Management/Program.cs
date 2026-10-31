
using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;
using HospitalAppointmentManagement.Services;

namespace HospitalAppointmentManagement;

internal class Program
{
    private const string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=HospitalAppointmentDB;Trusted_Connection=True;TrustServerCertificate=True;";

    // Services
    private static readonly DepartmentService DepartmentService =
        new DepartmentService(new DepartmentRepository());

    private static readonly PatientService PatientService =
        new PatientService(
            new PatientRepository(ConnectionString));

    private static readonly DoctorService DoctorService =
        new DoctorService(
            new DoctorRepository(ConnectionString));

    private static readonly AppointmentService AppointmentService =
        new AppointmentService(
            new AppointmentRepository(ConnectionString),
            PatientService,
            DoctorService);

    private static readonly AuditLogService AuditLogService =
        new AuditLogService(
            new AuditLogRepository(ConnectionString));

    private static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine(" HOSPITAL APPOINTMENT MANAGEMENT");
            Console.WriteLine("==================================");

            Console.WriteLine("\n--- Department Management ---");
            Console.WriteLine("1. Add Department");
            Console.WriteLine("2. View Departments");
            Console.WriteLine("3. Search Department");
            Console.WriteLine("4. Update Department");
            Console.WriteLine("5. Deactivate Department");

            Console.WriteLine("\n--- Patient Management ---");
            Console.WriteLine("6. Register Patient");
            Console.WriteLine("7. View Patients");
            Console.WriteLine("8. Search Patient by ID");
            Console.WriteLine("9. Search Patients by Name");
            Console.WriteLine("10. Update Patient");
            Console.WriteLine("11. Deactivate Patient");
            Console.WriteLine("12. View Patients by Department");

            Console.WriteLine("\n--- Doctor Management ---");
            Console.WriteLine("13. Add Doctor");
            Console.WriteLine("14. View Doctors");
            Console.WriteLine("15. Search Doctor by ID");
            Console.WriteLine("16. Search Doctors by Name");
            Console.WriteLine("17. Update Doctor");
            Console.WriteLine("18. Deactivate Doctor");
            Console.WriteLine("19. View Doctors by Department");

            Console.WriteLine("\n--- Appointment Management ---");
            Console.WriteLine("20. Book Appointment");
            Console.WriteLine("21. View Appointments");
            Console.WriteLine("22. Search Appointment by ID");
            Console.WriteLine("23. View Appointments by Patient");
            Console.WriteLine("24. Update Appointment Status");

            Console.WriteLine("\n--- Audit Logging ---");
            Console.WriteLine("25. View Audit Logs");

            Console.WriteLine("\n0. Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    case "1":
                        await AddDepartmentAsync();
                        break;
                    case "2":
                        await ViewDepartmentsAsync();
                        break;
                    case "3":
                        await SearchDepartmentAsync();
                        break;
                    case "4":
                        await UpdateDepartmentAsync();
                        break;
                    case "5":
                        await DeactivateDepartmentAsync();
                        break;

                    case "6":
                        await AddPatientAsync();
                        break;
                    case "7":
                        await ViewPatientsAsync();
                        break;
                    case "8":
                        await SearchPatientAsync();
                        break;
                    case "9":
                        await SearchPatientsByNameAsync();
                        break;
                    case "10":
                        await UpdatePatientAsync();
                        break;
                    case "11":
                        await DeactivatePatientAsync();
                        break;
                    case "12":
                        await ViewPatientsByDepartmentAsync();
                        break;

                    case "13":
                        await AddDoctorAsync();
                        break;
                    case "14":
                        await ViewDoctorsAsync();
                        break;
                    case "15":
                        await SearchDoctorAsync();
                        break;
                    case "16":
                        await SearchDoctorsByNameAsync();
                        break;
                    case "17":
                        await UpdateDoctorAsync();
                        break;
                    case "18":
                        await DeactivateDoctorAsync();
                        break;
                    case "19":
                        await ViewDoctorsByDepartmentAsync();
                        break;

                    case "20":
                        await BookAppointmentAsync();
                        break;
                    case "21":
                        await ViewAppointmentsAsync();
                        break;
                    case "22":
                        await SearchAppointmentAsync();
                        break;
                    case "23":
                        await ViewAppointmentsByPatientAsync();
                        break;
                    case "24":
                        await UpdateAppointmentStatusAsync();
                        break;

                    case "25":
                        await ViewAuditLogsAsync();
                        break;

                    case "0":
                        Console.WriteLine("Application closed.");
                        return;

                    default:
                        Console.WriteLine("Invalid menu choice.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }
            catch (SqlException ex)
                when (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine(
                    "Duplicate value detected. Check the unique fields.");
            }
            catch (SqlException ex)
                when (ex.Number == 547)
            {
                Console.WriteLine(
                    "Database constraint error. Check the related " +
                    "department, patient, doctor, or appointment.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // =====================================================
    // DEPARTMENT MANAGEMENT
    // =====================================================

    private static async Task AddDepartmentAsync()
    {
        Console.Write("Department name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Location (optional): ");
        string? location = Console.ReadLine();

        Department department = new()
        {
            DepartmentName = name,
            Location = string.IsNullOrWhiteSpace(location)
                ? null
                : location.Trim()
        };

        int id = await DepartmentService.AddAsync(department);

        Console.WriteLine($"Department added. ID: {id}");

        await TryLogAsync(
            "Department", id, "Created", name);
    }

    private static async Task ViewDepartmentsAsync()
    {
        List<Department> items =
            await DepartmentService.GetAllAsync();

        if (items.Count == 0)
        {
            Console.WriteLine("No departments found.");
            return;
        }

        foreach (Department item in items)
        {
            DisplayDepartment(item);
        }
    }

    private static async Task SearchDepartmentAsync()
    {
        int id = ReadPositiveId("Department ID: ");

        Department? item =
            await DepartmentService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        DisplayDepartment(item);
    }

    private static async Task UpdateDepartmentAsync()
    {
        int id = ReadPositiveId("Department ID: ");

        Department? old =
            await DepartmentService.GetByIdAsync(id);

        if (old is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        if (!old.IsActive)
        {
            Console.WriteLine("Department is inactive.");
            return;
        }

        Console.Write($"New name ({old.DepartmentName}): ");
        string name = Console.ReadLine() ?? "";

        Console.Write($"New location ({old.Location ?? "-"}): ");
        string location = Console.ReadLine() ?? "";

        Department updated = new()
        {
            DepartmentId = id,
            DepartmentName = string.IsNullOrWhiteSpace(name)
                ? old.DepartmentName
                : name.Trim(),
            Location = string.IsNullOrWhiteSpace(location)
                ? old.Location
                : location.Trim(),
            IsActive = old.IsActive
        };

        bool success =
            await DepartmentService.UpdateAsync(updated);

        Console.WriteLine(success
            ? "Department updated."
            : "Department was not updated.");

        if (success)
        {
            await TryLogAsync(
                "Department", id, "Updated",
                "Department details updated");
        }
    }

    private static async Task DeactivateDepartmentAsync()
    {
        int id = ReadPositiveId("Department ID: ");

        Department? item =
            await DepartmentService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        if (!item.IsActive)
        {
            Console.WriteLine("Department is already inactive.");
            return;
        }

        if (!Confirm($"Deactivate {item.DepartmentName}?"))
            return;

        bool success =
            await DepartmentService.DeactivateAsync(id);

        Console.WriteLine(success
            ? "Department deactivated."
            : "Department could not be deactivated.");

        if (success)
        {
            await TryLogAsync(
                "Department", id, "Deactivated",
                "Department deactivated");
        }
    }

    private static void DisplayDepartment(Department item)
    {
        Console.WriteLine(
            $"\nID: {item.DepartmentId}\n" +
            $"Name: {item.DepartmentName}\n" +
            $"Location: {item.Location ?? "-"}\n" +
            $"Status: {(item.IsActive ? "Active" : "Inactive")}");
    }

    // =====================================================
    // PATIENT MANAGEMENT
    // =====================================================

    private static async Task AddPatientAsync()
    {
        Console.Write("Patient name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Patient email: ");
        string email = Console.ReadLine() ?? "";

        int age = ReadInt("Patient age: ");
        int departmentId = ReadPositiveId("Department ID: ");

        Patient patient = new()
        {
            Name = name,
            Email = email,
            Age = age,
            DepartmentId = departmentId
        };

        int id = await PatientService.AddAsync(patient);

        Console.WriteLine($"Patient registered. ID: {id}");

        await TryLogAsync(
            "Patient", id, "Created",
            $"Patient {name} registered");
    }

    private static async Task ViewPatientsAsync()
    {
        List<Patient> items =
            await PatientService.GetAllAsync();

        if (items.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }

        foreach (Patient item in items)
        {
            DisplayPatient(item);
        }
    }

    private static async Task SearchPatientAsync()
    {
        int id = ReadPositiveId("Patient ID: ");

        Patient? item =
            await PatientService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        DisplayPatient(item);
    }

    private static async Task SearchPatientsByNameAsync()
    {
        Console.Write("Patient name to search: ");
        string name = Console.ReadLine() ?? "";

        List<Patient> items =
            await PatientService.SearchByNameAsync(name);

        if (items.Count == 0)
        {
            Console.WriteLine("No matching patients found.");
            return;
        }

        foreach (Patient item in items)
        {
            DisplayPatient(item);
        }
    }

    private static async Task UpdatePatientAsync()
    {
        int id = ReadPositiveId("Patient ID: ");

        Patient? old =
            await PatientService.GetByIdAsync(id);

        if (old is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        if (!old.IsActive)
        {
            Console.WriteLine("Cannot update an inactive patient.");
            return;
        }

        Console.Write($"New name ({old.Name}): ");
        string name = Console.ReadLine() ?? "";

        Console.Write($"New email ({old.Email}): ");
        string email = Console.ReadLine() ?? "";

        Console.Write($"New age ({old.Age}): ");
        string ageInput = Console.ReadLine() ?? "";

        int age = old.Age;

        if (!string.IsNullOrWhiteSpace(ageInput) &&
            !int.TryParse(ageInput, out age))
        {
            throw new ArgumentException("Enter a valid age.");
        }

        Console.Write($"Department ID ({old.DepartmentId}): ");
        string departmentInput = Console.ReadLine() ?? "";

        int departmentId = old.DepartmentId;

        if (!string.IsNullOrWhiteSpace(departmentInput) &&
            !int.TryParse(departmentInput, out departmentId))
        {
            throw new ArgumentException(
                "Enter a valid department ID.");
        }

        Patient updated = new()
        {
            PatientId = id,
            Name = string.IsNullOrWhiteSpace(name)
                ? old.Name
                : name.Trim(),
            Email = string.IsNullOrWhiteSpace(email)
                ? old.Email
                : email.Trim(),
            Age = age,
            DepartmentId = departmentId,
            RegisteredDate = old.RegisteredDate,
            IsActive = old.IsActive
        };

        bool success =
            await PatientService.UpdateAsync(updated);

        Console.WriteLine(success
            ? "Patient updated."
            : "Patient was not updated.");

        if (success)
        {
            await TryLogAsync(
                "Patient", id, "Updated",
                "Patient details updated");
        }
    }

    private static async Task DeactivatePatientAsync()
    {
        int id = ReadPositiveId("Patient ID: ");

        Patient? item =
            await PatientService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        if (!item.IsActive)
        {
            Console.WriteLine("Patient is already inactive.");
            return;
        }

        if (!Confirm($"Deactivate patient {item.Name}?"))
            return;

        bool success =
            await PatientService.DeactivateAsync(id);

        Console.WriteLine(success
            ? "Patient deactivated."
            : "Patient could not be deactivated.");

        if (success)
        {
            await TryLogAsync(
                "Patient", id, "Deactivated",
                "Patient deactivated");
        }
    }

    private static async Task ViewPatientsByDepartmentAsync()
    {
        int departmentId = ReadPositiveId("Department ID: ");

        List<Patient> items =
            await PatientService.GetByDepartmentAsync(departmentId);

        if (items.Count == 0)
        {
            Console.WriteLine("No patients found for this department.");
            return;
        }

        foreach (Patient item in items)
        {
            DisplayPatient(item);
        }
    }

    private static void DisplayPatient(Patient item)
    {
        Console.WriteLine(
            $"\nPatient ID: {item.PatientId}\n" +
            $"Name: {item.Name}\n" +
            $"Email: {item.Email}\n" +
            $"Age: {item.Age}\n" +
            $"Department ID: {item.DepartmentId}\n" +
            $"Registered: {item.RegisteredDate}\n" +
            $"Status: {(item.IsActive ? "Active" : "Inactive")}");
    }

    // =====================================================
    // DOCTOR MANAGEMENT
    // =====================================================

    private static async Task AddDoctorAsync()
    {
        Console.Write("Doctor name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Specialization: ");
        string specialization = Console.ReadLine() ?? "";

        decimal fee = ReadDecimal("Consultation fee: ");
        int slotMinutes = ReadPositiveId("Slot duration in minutes: ");
        int departmentId = ReadPositiveId("Department ID: ");

        Doctor doctor = new()
        {
            DoctorName = name,
            Specialization = specialization,
            ConsultationFee = fee,
            SlotMinutes = slotMinutes,
            DepartmentId = departmentId
        };

        int id = await DoctorService.AddAsync(doctor);

        Console.WriteLine($"Doctor added. ID: {id}");

        await TryLogAsync(
            "Doctor", id, "Created",
            $"Doctor {name} added");
    }

    private static async Task ViewDoctorsAsync()
    {
        List<Doctor> items =
            await DoctorService.GetAllAsync();

        if (items.Count == 0)
        {
            Console.WriteLine("No doctors found.");
            return;
        }

        foreach (Doctor item in items)
        {
            DisplayDoctor(item);
        }
    }

    private static async Task SearchDoctorAsync()
    {
        int id = ReadPositiveId("Doctor ID: ");

        Doctor? item =
            await DoctorService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Doctor not found.");
            return;
        }

        DisplayDoctor(item);
    }

    private static async Task SearchDoctorsByNameAsync()
    {
        Console.Write("Doctor name to search: ");
        string name = Console.ReadLine() ?? "";

        List<Doctor> items =
            await DoctorService.SearchByNameAsync(name);

        if (items.Count == 0)
        {
            Console.WriteLine("No matching doctors found.");
            return;
        }

        foreach (Doctor item in items)
        {
            DisplayDoctor(item);
        }
    }

    private static async Task UpdateDoctorAsync()
    {
        int id = ReadPositiveId("Doctor ID: ");

        Doctor? old =
            await DoctorService.GetByIdAsync(id);

        if (old is null)
        {
            Console.WriteLine("Doctor not found.");
            return;
        }

        if (!old.IsActive)
        {
            Console.WriteLine("Cannot update an inactive doctor.");
            return;
        }

        Console.Write($"Doctor name ({old.DoctorName}): ");
        string name = Console.ReadLine() ?? "";

        Console.Write($"Specialization ({old.Specialization}): ");
        string specialization = Console.ReadLine() ?? "";

        Console.Write($"Consultation fee ({old.ConsultationFee}): ");
        string feeInput = Console.ReadLine() ?? "";

        decimal fee = old.ConsultationFee;

        if (!string.IsNullOrWhiteSpace(feeInput) &&
            !decimal.TryParse(feeInput, out fee))
        {
            throw new ArgumentException(
                "Enter a valid consultation fee.");
        }

        Console.Write($"Slot minutes ({old.SlotMinutes}): ");
        string slotInput = Console.ReadLine() ?? "";

        int slotMinutes = old.SlotMinutes;

        if (!string.IsNullOrWhiteSpace(slotInput) &&
            !int.TryParse(slotInput, out slotMinutes))
        {
            throw new ArgumentException(
                "Enter a valid slot duration.");
        }

        Console.Write($"Department ID ({old.DepartmentId}): ");
        string departmentInput = Console.ReadLine() ?? "";

        int departmentId = old.DepartmentId;

        if (!string.IsNullOrWhiteSpace(departmentInput) &&
            !int.TryParse(departmentInput, out departmentId))
        {
            throw new ArgumentException(
                "Enter a valid department ID.");
        }

        Doctor updated = new()
        {
            DoctorId = id,
            DoctorName = string.IsNullOrWhiteSpace(name)
                ? old.DoctorName
                : name.Trim(),
            Specialization = string.IsNullOrWhiteSpace(specialization)
                ? old.Specialization
                : specialization.Trim(),
            ConsultationFee = fee,
            SlotMinutes = slotMinutes,
            DepartmentId = departmentId,
            IsActive = old.IsActive
        };

        bool success =
            await DoctorService.UpdateAsync(updated);

        Console.WriteLine(success
            ? "Doctor updated."
            : "Doctor was not updated.");

        if (success)
        {
            await TryLogAsync(
                "Doctor", id, "Updated",
                "Doctor details updated");
        }
    }

    private static async Task DeactivateDoctorAsync()
    {
        int id = ReadPositiveId("Doctor ID: ");

        Doctor? item =
            await DoctorService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Doctor not found.");
            return;
        }

        if (!item.IsActive)
        {
            Console.WriteLine("Doctor is already inactive.");
            return;
        }

        if (!Confirm($"Deactivate doctor {item.DoctorName}?"))
            return;

        bool success =
            await DoctorService.DeactivateAsync(id);

        Console.WriteLine(success
            ? "Doctor deactivated."
            : "Doctor could not be deactivated.");

        if (success)
        {
            await TryLogAsync(
                "Doctor", id, "Deactivated",
                "Doctor deactivated");
        }
    }

    private static async Task ViewDoctorsByDepartmentAsync()
    {
        int departmentId = ReadPositiveId("Department ID: ");

        List<Doctor> items =
            await DoctorService.GetByDepartmentAsync(departmentId);

        if (items.Count == 0)
        {
            Console.WriteLine("No doctors found for this department.");
            return;
        }

        foreach (Doctor item in items)
        {
            DisplayDoctor(item);
        }
    }

    private static void DisplayDoctor(Doctor item)
    {
        Console.WriteLine(
            $"\nDoctor ID: {item.DoctorId}\n" +
            $"Name: {item.DoctorName}\n" +
            $"Specialization: {item.Specialization}\n" +
            $"Consultation fee: {item.ConsultationFee:C}\n" +
            $"Slot minutes: {item.SlotMinutes}\n" +
            $"Department ID: {item.DepartmentId}\n" +
            $"Status: {(item.IsActive ? "Active" : "Inactive")}");
    }

    // =====================================================
    // APPOINTMENT MANAGEMENT
    // =====================================================

    private static async Task BookAppointmentAsync()
    {
        int patientId = ReadPositiveId("Patient ID: ");
        int doctorId = ReadPositiveId("Doctor ID: ");

        Console.Write("Appointment date and time (yyyy-MM-dd HH:mm): ");
        string input = Console.ReadLine() ?? "";

        if (!DateTime.TryParseExact(
                input,
                "yyyy-MM-dd HH:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out DateTime appointmentDate))
        {
            throw new ArgumentException(
                "Use the format yyyy-MM-dd HH:mm.");
        }

        Appointment appointment = new()
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDate = appointmentDate
        };

        int id = await AppointmentService.BookAsync(appointment);

        Console.WriteLine($"Appointment booked. ID: {id}");

        await TryLogAsync(
            "Appointment", id, "Created",
            $"Patient {patientId}, Doctor {doctorId}, " +
            $"Date {appointmentDate:yyyy-MM-dd HH:mm}");
    }

    private static async Task ViewAppointmentsAsync()
    {
        List<Appointment> items =
            await AppointmentService.GetAllAsync();

        if (items.Count == 0)
        {
            Console.WriteLine("No appointments found.");
            return;
        }

        foreach (Appointment item in items)
        {
            DisplayAppointment(item);
        }
    }

    private static async Task SearchAppointmentAsync()
    {
        int id = ReadPositiveId("Appointment ID: ");

        Appointment? item =
            await AppointmentService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Appointment not found.");
            return;
        }

        DisplayAppointment(item);
    }

    private static async Task ViewAppointmentsByPatientAsync()
    {
        int patientId = ReadPositiveId("Patient ID: ");

        List<Appointment> items =
            await AppointmentService.GetByPatientAsync(patientId);

        if (items.Count == 0)
        {
            Console.WriteLine("No appointments found for this patient.");
            return;
        }

        foreach (Appointment item in items)
        {
            DisplayAppointment(item);
        }
    }

    private static async Task UpdateAppointmentStatusAsync()
    {
        int id = ReadPositiveId("Appointment ID: ");

        Appointment? item =
            await AppointmentService.GetByIdAsync(id);

        if (item is null)
        {
            Console.WriteLine("Appointment not found.");
            return;
        }

        Console.WriteLine("Allowed statuses: Scheduled, Completed, Cancelled");
        Console.Write("New status: ");

        string status = Console.ReadLine() ?? "";

        bool success =
            await AppointmentService.UpdateStatusAsync(id, status);

        Console.WriteLine(success
            ? "Appointment status updated."
            : "Appointment status was not updated.");

        if (success)
        {
            await TryLogAsync(
                "Appointment", id, "StatusUpdated",
                $"New status: {status}");
        }
    }

    private static void DisplayAppointment(Appointment item)
    {
        Console.WriteLine(
            $"\nAppointment ID: {item.AppointmentId}\n" +
            $"Patient ID: {item.PatientId}\n" +
            $"Doctor ID: {item.DoctorId}\n" +
            $"Appointment date: {item.AppointmentDate}\n" +
            $"Booked at: {item.BookedAt}\n" +
            $"Status: {item.Status}");
    }

    // =====================================================
    // AUDIT LOGGING
    // =====================================================

    private static async Task ViewAuditLogsAsync()
    {
        List<AuditLog> logs =
            await AuditLogService.GetAllAsync();

        if (logs.Count == 0)
        {
            Console.WriteLine("No audit logs found.");
            return;
        }

        Console.WriteLine("\n--- Audit Logs ---");

        foreach (AuditLog log in logs)
        {
            Console.WriteLine(
                $"\nAudit ID: {log.AuditId}\n" +
                $"Entity: {log.EntityName}\n" +
                $"Entity ID: {log.EntityId}\n" +
                $"Action: {log.Action}\n" +
                $"Details: {log.Details ?? "-"}\n" +
                $"Created at: {log.CreatedAt}");
        }
    }

    private static async Task TryLogAsync(
        string entityName,
        int entityId,
        string action,
        string? details)
    {
        try
        {
            await AuditLogService.LogAsync(
                entityName, entityId, action, details);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Warning: The operation succeeded, but its audit " +
                $"log could not be saved: {ex.Message}");
        }
    }

    // =====================================================
    // COMMON INPUT HELPERS
    // =====================================================

    private static int ReadPositiveId(string prompt)
    {
        Console.Write(prompt);

        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
        {
            throw new ArgumentException(
                "Enter a positive numeric ID.");
        }

        return id;
    }

    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);

        if (!int.TryParse(Console.ReadLine(), out int value))
        {
            throw new ArgumentException(
                "Enter a valid whole number.");
        }

        return value;
    }

    private static decimal ReadDecimal(string prompt)
    {
        Console.Write(prompt);

        if (!decimal.TryParse(
                Console.ReadLine(),
                System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal value))
        {
            throw new ArgumentException(
                "Enter a valid numeric amount, for example 500.00.");
        }

        return value;
    }

    private static bool Confirm(string prompt)
    {
        Console.Write($"{prompt} (y/n): ");
        string answer = Console.ReadLine() ?? "";

        if (answer.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        Console.WriteLine("Operation cancelled.");
        return false;
    }
}
