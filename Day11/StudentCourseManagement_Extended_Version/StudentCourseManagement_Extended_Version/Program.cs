
using StudentCourseManagement.Models;
using StudentCourseManagement.Services;

DepartmentService departmentService = new DepartmentService();

while (true)
{
    Console.WriteLine("\n===== STUDENT COURSE MANAGEMENT =====");
    Console.WriteLine("1. Department Management");
    Console.WriteLine("2. Student Management");
    Console.WriteLine("3. Course Management");
    Console.WriteLine("4. Enrollment Management");
    Console.WriteLine("5. Reports");
    Console.WriteLine("6. Audit Logs");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            DepartmentManagementMenu();
            break;

        case "2":
            StudentManagementMenu();
            break;

        case "3":
            CourseManagementMenu();
            break;

        case "4":
            EnrollmentManagementMenu();
            break;

        case "5":
            ReportsMenu();
            break;

        case "6":
            AuditLogsMenu();
            break;

        case "0":
            Console.WriteLine("Exiting the application.");
            return;

        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }
}

void DepartmentManagementMenu()
{
    while (true)
    {
        Console.WriteLine("\n===== DEPARTMENT MANAGEMENT =====");
        Console.WriteLine("1. Add Department");
        Console.WriteLine("2. View All Departments");
        Console.WriteLine("3. Update Department");
        Console.WriteLine("4. Deactivate Department");
        Console.WriteLine("0. Back to Main Menu");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Enter department name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Enter location (optional): ");
                string? location = Console.ReadLine();

                Console.WriteLine(
                    departmentService.AddDepartment(name, location));
                break;

            case "2":
                List<Department> departments =
                    departmentService.GetAllDepartments();

                if (departments.Count == 0)
                {
                    Console.WriteLine("No departments found.");
                    break;
                }

                foreach (Department department in departments)
                {
                    Console.WriteLine(
                        $"ID: {department.DepartmentId} | " +
                        $"Name: {department.DepartmentName} | " +
                        $"Location: {department.Location ?? "Not specified"} | " +
                        $"Active: {department.IsActive}");
                }

                break;

            case "3":
                Console.Write("Enter department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int updateId) ||
                    updateId <= 0)
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.Write("Enter new department name: ");
                string newName = Console.ReadLine() ?? "";

                Console.Write("Enter new location: ");
                string? newLocation = Console.ReadLine();

                Console.WriteLine(departmentService.UpdateDepartment(
                    updateId, newName, newLocation));
                break;

            case "4":
                Console.Write("Enter department ID to deactivate: ");
                if (!int.TryParse(Console.ReadLine(), out int deactivateId) ||
                    deactivateId <= 0)
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.WriteLine(
                    departmentService.DeactivateDepartment(deactivateId));
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}

void StudentManagementMenu()
{
    StudentService service = new StudentService();

    while (true)
    {
        Console.WriteLine("\n===== STUDENT MANAGEMENT =====");
        Console.WriteLine("1. Add Student");
        Console.WriteLine("2. View All Students");
        Console.WriteLine("3. Search Student");
        Console.WriteLine("4. Filter by Department");
        Console.WriteLine("5. Update Student");
        Console.WriteLine("6. Deactivate Student");
        Console.WriteLine("0. Back");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Email: ");
                string email = Console.ReadLine() ?? "";

                Console.Write("Age: ");
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine("Invalid age.");
                    break;
                }

                Console.Write("Department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int departmentId))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.WriteLine(service.AddStudent(
                    name, email, age, departmentId));
                break;

            case "2":
                DisplayStudents(service.GetStudents());
                break;

            case "3":
                Console.Write("Enter name or email: ");
                DisplayStudents(service.GetStudents(Console.ReadLine()));
                break;

            case "4":
                Console.Write("Department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int filterId) ||
                    filterId <= 0)
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                DisplayStudents(
                    service.GetStudents(departmentId: filterId));
                break;

            case "5":
                Console.Write("Student ID: ");
                if (!int.TryParse(Console.ReadLine(), out int studentId) ||
                    studentId <= 0)
                {
                    Console.WriteLine("Invalid student ID.");
                    break;
                }

                Console.Write("New name: ");
                string newName = Console.ReadLine() ?? "";

                Console.Write("New email: ");
                string newEmail = Console.ReadLine() ?? "";

                Console.Write("New age: ");
                if (!int.TryParse(Console.ReadLine(), out int newAge))
                {
                    Console.WriteLine("Invalid age.");
                    break;
                }

                Console.Write("New department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int newDepartmentId))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.WriteLine(service.UpdateStudent(
                    studentId, newName, newEmail, newAge, newDepartmentId));
                break;

            case "6":
                Console.Write("Student ID to deactivate: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Invalid student ID.");
                    break;
                }

                Console.WriteLine(service.DeactivateStudent(id));
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}

void DisplayStudents(List<Student> students)
{
    if (students.Count == 0)
    {
        Console.WriteLine("No students found.");
        return;
    }

    foreach (Student student in students)
    {
        Console.WriteLine(
            $"ID: {student.StudentId} | Name: {student.Name} | " +
            $"Email: {student.Email} | Age: {student.Age} | " +
            $"Department: {student.DepartmentId} | " +
            $"Admission: {student.AdmissionDate:g} | " +
            $"Active: {student.IsActive}");
    }
}

void CourseManagementMenu()
{
    CourseService service = new CourseService();

    while (true)
    {
        Console.WriteLine("\n===== COURSE MANAGEMENT =====");
        Console.WriteLine("1. Add Course");
        Console.WriteLine("2. View All Courses");
        Console.WriteLine("3. Search Course");
        Console.WriteLine("4. Filter by Department");
        Console.WriteLine("5. Update Course");
        Console.WriteLine("6. Deactivate Course");
        Console.WriteLine("0. Back");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Course name: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Duration in months: ");
                if (!int.TryParse(Console.ReadLine(), out int duration))
                {
                    Console.WriteLine("Invalid duration.");
                    break;
                }

                Console.Write("Fee: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal fee))
                {
                    Console.WriteLine("Invalid fee.");
                    break;
                }

                Console.Write("Department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int departmentId))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.WriteLine(service.AddCourse(
                    name, duration, fee, departmentId));
                break;

            case "2":
                DisplayCourses(service.GetCourses());
                break;

            case "3":
                Console.Write("Course name to search: ");
                DisplayCourses(service.GetCourses(Console.ReadLine()));
                break;

            case "4":
                Console.Write("Department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int filterId) ||
                    filterId <= 0)
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                DisplayCourses(
                    service.GetCourses(departmentId: filterId));
                break;

            case "5":
                Console.Write("Course ID: ");
                if (!int.TryParse(Console.ReadLine(), out int courseId) ||
                    courseId <= 0)
                {
                    Console.WriteLine("Invalid course ID.");
                    break;
                }

                Console.Write("New course name: ");
                string newName = Console.ReadLine() ?? "";

                Console.Write("New duration in months: ");
                if (!int.TryParse(Console.ReadLine(), out int newDuration))
                {
                    Console.WriteLine("Invalid duration.");
                    break;
                }

                Console.Write("New fee: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal newFee))
                {
                    Console.WriteLine("Invalid fee.");
                    break;
                }

                Console.Write("New department ID: ");
                if (!int.TryParse(Console.ReadLine(), out int newDepartmentId))
                {
                    Console.WriteLine("Invalid department ID.");
                    break;
                }

                Console.WriteLine(service.UpdateCourse(
                    courseId, newName, newDuration, newFee, newDepartmentId));
                break;

            case "6":
                Console.Write("Course ID to deactivate: ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
                {
                    Console.WriteLine("Invalid course ID.");
                    break;
                }

                Console.WriteLine(service.DeactivateCourse(id));
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}

void DisplayCourses(List<Course> courses)
{
    if (courses.Count == 0)
    {
        Console.WriteLine("No courses found.");
        return;
    }

    foreach (Course course in courses)
    {
        Console.WriteLine(
            $"ID: {course.CourseId} | Name: {course.CourseName} | " +
            $"Duration: {course.DurationMonths} months | " +
            $"Fee: {course.Fee:C} | Department: {course.DepartmentId} | " +
            $"Active: {course.IsActive}");
    }
}

void EnrollmentManagementMenu()
{
    EnrollmentService service = new EnrollmentService();

    while (true)
    {
        Console.WriteLine("\n===== ENROLLMENT MANAGEMENT =====");
        Console.WriteLine("1. Enroll Student");
        Console.WriteLine("2. View Enrollments");
        Console.WriteLine("3. Change Enrollment Status");
        Console.WriteLine("0. Back");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Student ID: ");
                if (!int.TryParse(Console.ReadLine(), out int studentId))
                {
                    Console.WriteLine("Invalid student ID.");
                    break;
                }

                Console.Write("Course ID: ");
                if (!int.TryParse(Console.ReadLine(), out int courseId))
                {
                    Console.WriteLine("Invalid course ID.");
                    break;
                }

                Console.WriteLine(
                    service.EnrollStudent(studentId, courseId));
                break;

            case "2":
                List<Enrollment> enrollments = service.GetEnrollments();

                if (enrollments.Count == 0)
                {
                    Console.WriteLine("No enrollments found.");
                    break;
                }

                foreach (Enrollment enrollment in enrollments)
                {
                    Console.WriteLine(
                        $"ID: {enrollment.EnrollmentId} | " +
                        $"Student: {enrollment.StudentId} | " +
                        $"Course: {enrollment.CourseId} | " +
                        $"Date: {enrollment.EnrolledAt:g} | " +
                        $"Status: {enrollment.Status}");
                }

                break;

            case "3":
                Console.Write("Enrollment ID: ");
                if (!int.TryParse(Console.ReadLine(), out int enrollmentId))
                {
                    Console.WriteLine("Invalid enrollment ID.");
                    break;
                }

                Console.Write("New status (Enrolled/Completed/Cancelled): ");
                string status = Console.ReadLine() ?? "";

                Console.WriteLine(service.ChangeStatus(enrollmentId, status));
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}

void ReportsMenu()
{
    ReportService service = new ReportService();

    while (true)
    {
        Console.WriteLine("\n===== REPORTS =====");
        Console.WriteLine("1. Students by Department");
        Console.WriteLine("2. Enrollment Counts by Course");
        Console.WriteLine("3. Student Enrollment Details");
        Console.WriteLine("4. Course Fee Summary");
        Console.WriteLine("0. Back");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                service.StudentsByDepartment();
                break;

            case "2":
                service.EnrollmentCountsByCourse();
                break;

            case "3":
                service.StudentEnrollmentDetails();
                break;

            case "4":
                service.CourseFeeSummary();
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}

void AuditLogsMenu()
{
    AuditLogService service = new AuditLogService();

    while (true)
    {
        Console.WriteLine("\n===== AUDIT LOGS =====");
        Console.WriteLine("1. View Audit Logs");
        Console.WriteLine("0. Back");
        Console.Write("Enter your choice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                List<AuditLog> logs = service.GetAllAuditLogs();

                if (logs.Count == 0)
                {
                    Console.WriteLine("No audit logs found.");
                    break;
                }

                foreach (AuditLog log in logs)
                {
                    Console.WriteLine(
                        $"Audit ID: {log.AuditId} | " +
                        $"Entity: {log.EntityName} | " +
                        $"Entity ID: {log.EntityId} | " +
                        $"Action: {log.Action} | " +
                        $"Details: {log.Details ?? "None"} | " +
                        $"Created: {log.CreatedAt:g}");
                }

                break;

            case "0":
                return;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
}
