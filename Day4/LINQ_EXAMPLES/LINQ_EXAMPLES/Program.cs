using System.ComponentModel.DataAnnotations;
using System.Globalization;

public class StoreOrder
{
    public int OrderId { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public decimal Amount { get; set; }
}

public class Student
{
    
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DepartmentId { get; set; }
}

public class Department
{
    public int Id { get; set; }
    public string DepartmentName { get; set; } = "";
}


public static class Program
{
    public static void Main()
    {
        List<StoreOrder> orders = new()
        {
            new() { OrderId = 1, ProductName = "Laptop", Category = "Electronics", Amount = 1200.00m },
            new() { OrderId = 2, ProductName = "Smartphone", Category = "Electronics", Amount = 800.00m },
            new() { OrderId = 3, ProductName = "Desk Chair", Category = "Furniture", Amount = 150.00m },
            new() { OrderId = 4, ProductName = "Headphones", Category = "Electronics", Amount = 200.00m },
            new() { OrderId = 5, ProductName = "Coffee Table", Category = "Furniture", Amount = 300.00m }
        };

        foreach (StoreOrder order in orders)
        {
            Console.WriteLine($"OrderId: {order.OrderId}, ProductName: {order.ProductName}, Category: {order.Category}, Price: {order.Amount.ToString("C", CultureInfo.CurrentCulture)}");
        }

        var expensive = orders.Where(o => o.Amount > 1000m);
        foreach (var order in expensive)
        {
            Console.WriteLine($"Expensive Order - OrderId: {order.OrderId}, ProductName: {order.ProductName}, Category: {order.Category}, Price: {order.Amount.ToString("C", CultureInfo.CurrentCulture)}");
        }

        var names = orders.Select(o => o.ProductName);
        Console.WriteLine("Product Names: " + string.Join(", ", names));

        var summaries = orders.Select(o => new
        {
            o.OrderId,
            o.ProductName,
            o.Amount,
            IsHighValue = o.Amount > 1000m
        });
        Console.WriteLine("Order Summaries:");
        foreach (var summary in summaries)
        {
            Console.WriteLine($"OrderId: {summary.OrderId}, ProductName: {summary.ProductName}, Price: {summary.Amount.ToString("C", CultureInfo.CurrentCulture)}, IsHighValue: {summary.IsHighValue}");
        }

        List<List<string>> courseGroups = new()
        {
            new(){"C#","SQL","LINQ"},
            new(){"Java","Spring","Hibernate"}
        };

        var allCourses = courseGroups.SelectMany(cg => cg);
        Console.WriteLine("All Courses: " + string.Join(", ", allCourses));

        Console.WriteLine("All Courses: " + string.Join(", ", allCourses));

        var highestFirst = orders.OrderByDescending(o => o.Amount);
        Console.WriteLine(highestFirst);
        var categoryThenName = orders
        .OrderBy(o => o.Category)
        .ThenBy(o => o.ProductName);
        Console.WriteLine("Orders sorted by Category then Product Name:");
        foreach (var order in categoryThenName)
        {
            Console.WriteLine($"OrderId: {order.OrderId}, ProductName: {order.ProductName}, Category: {order.Category}, Price: {order.Amount.ToString("C", CultureInfo.CurrentCulture)}");
        }
        int count = orders.Count();
        int electronicsCount = orders.Count(o => o.Category == "Electronics");
        decimal total = orders.Sum(o => o.Amount);
        decimal average = orders.Average(o => o.Amount);
        decimal minimum = orders.Min(o => o.Amount);
        decimal maximum = orders.Max(o => o.Amount);
        Console.WriteLine(count);
        Console.WriteLine(electronicsCount);
        Console.WriteLine(total);
        Console.WriteLine(average);
        Console.WriteLine(minimum);
        Console.WriteLine(maximum);

        bool hasLargeOrder = orders.Any(o => o.Amount > 4000m);
        bool allPositive = orders.All(o => o.Amount > 0m);
        bool notEmpty = orders.Any();
        Console.WriteLine(hasLargeOrder);
        Console.WriteLine(allPositive);
        Console.WriteLine(notEmpty);

        var groups = orders.GroupBy(o => o.Category);
        foreach (var group in groups)
        {
            Console.WriteLine($"Category: {group.Key}");
            foreach (var o in group)
            {
                Console.WriteLine($"OrderId: {o.OrderId}, ProductName: {o.ProductName}, Price: {o.Amount.ToString("C", CultureInfo.CurrentCulture)}");
            }
        }

        var salesByCategory = orders
            .GroupBy(o => o.Category)
            .Select(g => new
            {
                Category = g.Key,
                TotalSales = g.Sum(o => o.Amount),
                OrderCount = g.Count(),
                AverageOrder = g.Average(o => o.Amount)
            });
        Console.WriteLine("Sales by Category:");
        foreach (var category in salesByCategory)
        {
            Console.WriteLine($"Category: {category.Category}, Total Sales: {category.TotalSales.ToString("C", CultureInfo.CurrentCulture)}, Order Count: {category.OrderCount}, Average Order: {category.AverageOrder.ToString("C", CultureInfo.CurrentCulture)}");

        }

        int[] a = {10,20,30, 40, 40 };
        int[] b = { 30, 40, 50, 60 };
        var unique = a.Distinct();
        var common = a.Intersect(b);
        var all = a.Union(b);
        var onlyInA = a.Except(b);
        Console.WriteLine("Unique elements in array a:");
        foreach (var item in unique)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Common elements between arrays a and b:");
        foreach (var item in common)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("All elements in arrays a and b:");
        foreach (var item in all)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Elements in array a but not in array b:");
        foreach (var item in onlyInA)
        {
            Console.WriteLine(item);
        }
        List<Student> students = new()
        {
          new() { Id=1, Name="Arun", DepartmentId=10 },
          new() { Id=2, Name="Priya", DepartmentId=20 },
          new() { Id=3, Name="Kumar", DepartmentId=10 },
          new() { Id=4, Name="Divya", DepartmentId=30 }
        };

        foreach (Student student in students)
        {
            Console.WriteLine($"Id: {student.Id}, Name: {student.Name}, DepartmentId: {student.DepartmentId}");
        }

        List<Department> departments = new()
       {
         new() { Id=10, DepartmentName="CSE" },
         new() { Id=20, DepartmentName="ECE" },
         new() { Id=40, DepartmentName="IT" }
       };

        foreach (Department department in departments)
        {
            Console.WriteLine($"Id: {department.Id}, DepartmentName: {department.DepartmentName}");
        }

        var result = students.Join(
            departments,
            student => student.DepartmentId,
            department => department.Id,
            (student, department) => new
            {
                StudentName = student.Name,
                DepartmentName = department.DepartmentName
            });
        Console.WriteLine("Joined Results:");
        foreach (var item in result)
        {
            Console.WriteLine($"Student: {item.StudentName}, Department: {item.DepartmentName}");
        }

        var result2 = from student in students
                      join department in departments
                      on student.DepartmentId equals department.Id
                      select new
                      {
                          StudentName = student.Name,
                          DepartmentName = department.DepartmentName
                      };

        Console.WriteLine("Joined Results (Query Syntax):");
        foreach (var item in result2)
        {
            Console.WriteLine($"Student: {item.StudentName}, Department: {item.DepartmentName}");
        }
    }   
}
