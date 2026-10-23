using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EventListener_Examples.Example
{

    public class student
    {
        public List<course>? courses { get; set; }
        public int id { get; set; }
        public string? name { get; set; }

        public student(int id, string? name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class course
    {
        public int CourseId { get; set; }

        public string? CourseName { get; set; }

        public course(int courseId, string? courseName)
        {
            this.CourseId = courseId;
            this.CourseName = courseName;
        }
    }
    class Json_Example
    {
        public void access()
        {
            /*
            student s1 = new student(1, "John Doe");
            //serialize the student object to JSON
            string? json = JsonSerializer.Serialize(s1);
            Console.WriteLine(json);

            //deserialize the JSON string back to a student object
            student? s2 = JsonSerializer.Deserialize<student>(json);
            s2?.name = "Jane Doe";
            Console.WriteLine($"id: {s2?.id}, name: {s2?.name}");
            */

            List<course> courses = new List<course>
            {
                new course(1, "Math"),
                new course(2, "Science"),
                new course(3, "History")
            };
            
            foreach (course c in courses)
            {
                Console.WriteLine($"CourseId: {c.CourseId}, CourseName: {c.CourseName}");
            };
            //How can i implement student objects with courses and serialize and deserialize them to JSON using System.Text.Json in C#?
            List<student> studentsWithCourses = new List<student>
            {
                new student(1, "Abdul") { courses = new List<course> { courses[0], courses[1] } },
                new student(2, "Arun") { courses = new List<course> { courses[1], courses[2] } },
                new student(3, "Bala") { courses = new List<course> { courses[0], courses[2] } }
            };
            foreach (student s in studentsWithCourses)
            {
                Console.WriteLine($"id: {s.id}, name: {s.name}");
                if (s.courses != null)
                {
                    foreach (course c in s.courses)
                    {
                        Console.WriteLine($"CourseId: {c.CourseId}, CourseName: {c.CourseName}");
                    }
                }
            };

            //serialize the list of student objects with courses to JSON
            foreach (student s in studentsWithCourses)
            {
                string? json = JsonSerializer.Serialize(s);
                Console.WriteLine(json);
            };
           

            //List of student objects
            List<student> students = new List<student>
            {
                new student(1, "Abdul"),
                new student(2, "Arun"),
                new student(3, "Bala")
            };

            //serialize the list of student objects to JSON
            Console.WriteLine("Serialized Students:");
            List<string> jsonList = new List<string>();
            foreach (student s in students)
            {
                jsonList.Add(JsonSerializer.Serialize(s));
            };
            Console.WriteLine(string.Join("\n", jsonList));

            //deserialize the JSON strings back to student objects
            List<student> deserializedStudents = new List<student>();
            foreach (string json in jsonList)
            {
                student? s = JsonSerializer.Deserialize<student>(json);
                if (s != null)
                {
                    deserializedStudents.Add(s);
                }
            };
            Console.WriteLine("Deserialized Students:");
            foreach (student s in deserializedStudents)
            {
                Console.WriteLine($"id: {s.id}, name: {s.name}");
            };
        }
    }
}
