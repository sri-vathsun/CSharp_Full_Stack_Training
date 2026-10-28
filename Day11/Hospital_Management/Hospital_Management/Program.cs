using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;
using HospitalAppointmentManagement.Services;

namespace HospitalAppointmentManagement;

internal class Program
{
    // Existing Department service
    private static readonly DepartmentService DepartmentService =
        new DepartmentService(new DepartmentRepository());

    // Patient service
    // Replace the database name with your actual SQL Server database name.
    private static readonly PatientService PatientService =
        new PatientService(
            new PatientRepository(
                @"Server=.\SQLEXPRESS;Database=HospitalAppointmentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"));

    private static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== HOSPITAL MANAGEMENT ===");
            Console.WriteLine("\n--- Department Management ---");
            Console.WriteLine("1. Add Department");
            Console.WriteLine("2. View All Departments");
            Console.WriteLine("3. Search Department by ID");
            Console.WriteLine("4. Update Department");
            Console.WriteLine("5. Deactivate Department");

            Console.WriteLine("\n--- Patient Management ---");
            Console.WriteLine("6. Register Patient");
            Console.WriteLine("7. View All Patients");
            Console.WriteLine("8. Search Patient by ID");
            Console.WriteLine("9. Search Patients by Name");
            Console.WriteLine("10. Update Patient");
            Console.WriteLine("11. Deactivate Patient");
            Console.WriteLine("12. View Patients by Department");

            Console.WriteLine("\n0. Exit");
            Console.Write("Enter choice: ");

            string choice = Console.ReadLine() ?? "";

            try
            {
                switch (choice)
                {
                    // Department operations
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

                    // Patient operations
                    case "6":
                        AddPatient();
                        break;

                    case "7":
                        ViewPatients();
                        break;

                    case "8":
                        SearchPatient();
                        break;

                    case "9":
                        SearchPatientsByName();
                        break;

                    case "10":
                        UpdatePatient();
                        break;

                    case "11":
                        DeactivatePatient();
                        break;

                    case "12":
                        ViewPatientsByDepartment();
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
                    "A record with that unique value already exists. " +
                    "Check the department name or patient email.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    // =========================================================
    // DEPARTMENT MANAGEMENT
    // =========================================================

    private static async Task AddDepartmentAsync()
    {
        Console.Write("Department name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Location (optional): ");
        string? location = Console.ReadLine();

        var department = new Department
        {
            DepartmentName = name,
            Location = location
        };

        int id = await DepartmentService.AddAsync(department);

        Console.WriteLine(
            $"Department added successfully. ID: {id}");
    }

    private static async Task ViewDepartmentsAsync()
    {
        List<Department> departments =
            await DepartmentService.GetAllAsync();

        if (departments.Count == 0)
        {
            Console.WriteLine("No departments found.");
            return;
        }

        Console.WriteLine(
            "\nID | Department Name | Location | Status");

        foreach (Department department in departments)
        {
            Console.WriteLine(
                $"{department.DepartmentId} | " +
                $"{department.DepartmentName} | " +
                $"{department.Location ?? "-"} | " +
                $"{(department.IsActive ? "Active" : "Inactive")}");
        }
    }

    private static async Task SearchDepartmentAsync()
    {
        int id = ReadDepartmentId();

        Department? department =
            await DepartmentService.GetByIdAsync(id);

        if (department is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        DisplayDepartment(department);
    }

    private static async Task UpdateDepartmentAsync()
    {
        int id = ReadDepartmentId();

        Department? existing =
            await DepartmentService.GetByIdAsync(id);

        if (existing is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        if (!existing.IsActive)
        {
            Console.WriteLine("Cannot update an inactive department.");
            return;
        }

        Console.Write($"New name ({existing.DepartmentName}): ");
        string name = Console.ReadLine() ?? "";

        Console.Write($"New location ({existing.Location ?? "-"}): ");
        string location = Console.ReadLine() ?? "";

        var updated = new Department
        {
            DepartmentId = id,
            DepartmentName = string.IsNullOrWhiteSpace(name)
                ? existing.DepartmentName
                : name.Trim(),
            Location = string.IsNullOrWhiteSpace(location)
                ? existing.Location
                : location.Trim(),
            IsActive = existing.IsActive
        };

        bool success =
            await DepartmentService.UpdateAsync(updated);

        Console.WriteLine(success
            ? "Department updated successfully."
            : "Department was not updated.");
    }

    private static async Task DeactivateDepartmentAsync()
    {
        int id = ReadDepartmentId();

        Department? existing =
            await DepartmentService.GetByIdAsync(id);

        if (existing is null)
        {
            Console.WriteLine("Department not found.");
            return;
        }

        if (!existing.IsActive)
        {
            Console.WriteLine("Department is already inactive.");
            return;
        }

        Console.Write(
            $"Deactivate {existing.DepartmentName}? (y/n): ");

        string confirmation = Console.ReadLine() ?? "";

        if (!confirmation.Equals(
                "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deactivation cancelled.");
            return;
        }

        bool success =
            await DepartmentService.DeactivateAsync(id);

        Console.WriteLine(success
            ? "Department deactivated successfully."
            : "Department could not be deactivated.");
    }

    private static int ReadDepartmentId()
    {
        Console.Write("Enter department ID: ");

        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
        {
            throw new ArgumentException(
                "Enter a positive numeric department ID.");
        }

        return id;
    }

    private static void DisplayDepartment(Department department)
    {
        Console.WriteLine($"\nID: {department.DepartmentId}");
        Console.WriteLine($"Name: {department.DepartmentName}");
        Console.WriteLine($"Location: {department.Location ?? "-"}");
        Console.WriteLine(
            $"Status: {(department.IsActive ? "Active" : "Inactive")}");
    }

    // =========================================================
    // PATIENT MANAGEMENT
    // =========================================================

    private static void AddPatient()
    {
        Console.Write("Patient name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Patient email: ");
        string email = Console.ReadLine() ?? "";

        Console.Write("Patient age: ");

        if (!int.TryParse(Console.ReadLine(), out int age))
        {
            throw new ArgumentException(
                "Enter a valid numeric age.");
        }

        Console.Write("Department ID: ");

        if (!int.TryParse(
                Console.ReadLine(), out int departmentId))
        {
            throw new ArgumentException(
                "Enter a valid numeric department ID.");
        }

        var patient = new Patient
        {
            Name = name,
            Email = email,
            Age = age,
            DepartmentId = departmentId
        };

        int id = PatientService.AddPatient(patient);

        Console.WriteLine(
            $"Patient registered successfully. ID: {id}");
    }

    private static void ViewPatients()
    {
        List<Patient> patients =
            PatientService.GetAllPatients();

        if (patients.Count == 0)
        {
            Console.WriteLine("No patients found.");
            return;
        }

        Console.WriteLine(
            "\nID | Name | Email | Age | Department | Registered | Status");

        foreach (Patient patient in patients)
        {
            DisplayPatient(patient);
        }
    }

    private static void SearchPatient()
    {
        int id = ReadPatientId();

        Patient? patient =
            PatientService.GetPatientById(id);

        if (patient is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        DisplayPatient(patient);
    }

    private static void SearchPatientsByName()
    {
        Console.Write("Enter patient name to search: ");
        string name = Console.ReadLine() ?? "";

        List<Patient> patients =
            PatientService.SearchPatientsByName(name);

        if (patients.Count == 0)
        {
            Console.WriteLine("No matching patients found.");
            return;
        }

        foreach (Patient patient in patients)
        {
            DisplayPatient(patient);
        }
    }

    private static void UpdatePatient()
    {
        int id = ReadPatientId();

        Patient? existing =
            PatientService.GetPatientById(id);

        if (existing is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        if (!existing.IsActive)
        {
            Console.WriteLine("Cannot update an inactive patient.");
            return;
        }

        Console.Write($"New name ({existing.Name}): ");
        string name = Console.ReadLine() ?? "";

        Console.Write($"New email ({existing.Email}): ");
        string email = Console.ReadLine() ?? "";

        Console.Write($"New age ({existing.Age}): ");
        string ageInput = Console.ReadLine() ?? "";

        int age = existing.Age;

        if (!string.IsNullOrWhiteSpace(ageInput) &&
            !int.TryParse(ageInput, out age))
        {
            throw new ArgumentException(
                "Enter a valid numeric age.");
        }

        Console.Write(
            $"New department ID ({existing.DepartmentId}): ");

        string departmentInput = Console.ReadLine() ?? "";

        int departmentId = existing.DepartmentId;

        if (!string.IsNullOrWhiteSpace(departmentInput) &&
            !int.TryParse(departmentInput, out departmentId))
        {
            throw new ArgumentException(
                "Enter a valid numeric department ID.");
        }

        var updated = new Patient
        {
            PatientId = id,
            Name = string.IsNullOrWhiteSpace(name)
                ? existing.Name
                : name.Trim(),
            Email = string.IsNullOrWhiteSpace(email)
                ? existing.Email
                : email.Trim(),
            Age = age,
            DepartmentId = departmentId,
            RegisteredDate = existing.RegisteredDate,
            IsActive = existing.IsActive
        };

        bool success =
            PatientService.UpdatePatient(updated);

        Console.WriteLine(success
            ? "Patient updated successfully."
            : "Patient was not updated.");
    }

    private static void DeactivatePatient()
    {
        int id = ReadPatientId();

        Patient? patient =
            PatientService.GetPatientById(id);

        if (patient is null)
        {
            Console.WriteLine("Patient not found.");
            return;
        }

        if (!patient.IsActive)
        {
            Console.WriteLine("Patient is already inactive.");
            return;
        }

        Console.Write(
            $"Deactivate patient {patient.Name}? (y/n): ");

        string confirmation = Console.ReadLine() ?? "";

        if (!confirmation.Equals(
                "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deactivation cancelled.");
            return;
        }

        bool success =
            PatientService.DeactivatePatient(id);

        Console.WriteLine(success
            ? "Patient deactivated successfully."
            : "Patient could not be deactivated.");
    }

    private static void ViewPatientsByDepartment()
    {
        Console.Write("Enter department ID: ");

        if (!int.TryParse(
                Console.ReadLine(), out int departmentId) ||
            departmentId <= 0)
        {
            throw new ArgumentException(
                "Enter a positive numeric department ID.");
        }

        List<Patient> patients =
            PatientService.GetPatientsByDepartment(departmentId);

        if (patients.Count == 0)
        {
            Console.WriteLine(
                "No patients found for this department.");
            return;
        }

        foreach (Patient patient in patients)
        {
            DisplayPatient(patient);
        }
    }

    private static int ReadPatientId()
    {
        Console.Write("Enter patient ID: ");

        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
        {
            throw new ArgumentException(
                "Enter a positive numeric patient ID.");
        }

        return id;
    }

    private static void DisplayPatient(Patient patient)
    {
        Console.WriteLine("\n--- Patient Details ---");
        Console.WriteLine($"Patient ID: {patient.PatientId}");
        Console.WriteLine($"Name: {patient.Name}");
        Console.WriteLine($"Email: {patient.Email}");
        Console.WriteLine($"Age: {patient.Age}");
        Console.WriteLine($"Department ID: {patient.DepartmentId}");
        Console.WriteLine(
            $"Registered Date: {patient.RegisteredDate}");
        Console.WriteLine(
            $"Status: {(patient.IsActive ? "Active" : "Inactive")}");
    }
}
