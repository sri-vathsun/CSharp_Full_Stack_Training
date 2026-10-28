
using Microsoft.Data.SqlClient;
using HospitalAppointmentManagement.Models;
using HospitalAppointmentManagement.Repositories;
using HospitalAppointmentManagement.Services;

namespace HospitalAppointmentManagement;

internal class Program
{
    private static readonly DepartmentService DepartmentService =
        new DepartmentService(new DepartmentRepository());

    private static async Task Main()
    {
        while (true)
        {
            Console.WriteLine("\n=== DEPARTMENT MANAGEMENT ===");
            Console.WriteLine("1. Add Department");
            Console.WriteLine("2. View All Departments");
            Console.WriteLine("3. Search Department by ID");
            Console.WriteLine("4. Update Department");
            Console.WriteLine("5. Deactivate Department");
            Console.WriteLine("0. Exit");
            Console.Write("Enter choice: ");

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
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                Console.WriteLine(
                    "A department with that name already exists.");
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

        Console.WriteLine($"Department added successfully. ID: {id}");
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

        bool success = await DepartmentService.UpdateAsync(updated);

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

        bool success = await DepartmentService.DeactivateAsync(id);

        Console.WriteLine(success
            ? "Department deactivated successfully."
            : "Department could not be deactivated.");
    }

    private static int ReadDepartmentId()
    {
        Console.Write("Enter department ID: ");
        string input = Console.ReadLine() ?? "";

        if (!int.TryParse(input, out int id) || id <= 0)
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
}
