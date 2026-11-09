using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalWork6_2
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
    }

    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public double AverageGrade { get; set; }
        public int CourseId { get; set; }

        public Student(int id, string fullName, string group, double averageGrade, int courseId)
        {
            Id = id;
            FullName = fullName;
            Group = group;
            AverageGrade = averageGrade;
            CourseId = courseId;
        }

        public override string ToString()
        {
            return $"[ID: {Id}] {FullName} | Група: {Group} | Середній бал: {AverageGrade} | Курс ID: {CourseId}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Course> courses = new List<Course>
            {
                new Course { Id = 1, Title = "Об'єктно-орієнтоване програмування C#", Credits = 5 },
                new Course { Id = 2, Title = "Вебтехнології та вебдизайн", Credits = 4 },
                new Course { Id = 3, Title = "Викладацька та проєктна діяльність", Credits = 3 }
            };

            List<Student> students = new List<Student>
            {
                new Student(1, "Валерія Савлук", "ТЦР-21", 92.5, 1),
                new Student(2, "Олександр Коваль", "ТЦР-21", 74.0, 1),
                new Student(3, "Марія Петренко", "ТЦР-22", 88.0, 2),
                new Student(4, "Андрій Шевченко", "ТЦР-21", 61.5, 1),
                new Student(5, "Ірина Мельник", "ТЦР-22", 95.0, 3),
                new Student(6, "Дмитро Бойко", "ТЦР-22", 82.0, 2)
            };

            Console.WriteLine("=== 1. ФІЛЬТРАЦІЯ (Where) ===");
            var filteredStudents = students.Where(s => s.AverageGrade > 80);
            foreach (var s in filteredStudents)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("\n=== 2. СОРТУВАННЯ (OrderByDescending) ===");
            var sortedStudents = students.OrderByDescending(s => s.AverageGrade);
            foreach (var s in sortedStudents)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("\n=== 3. ПРОЄКЦІЯ (Select з анонімним типом) ===");
            var projectedStudents = students.Select(s => new { s.FullName, Score = s.AverageGrade });
            foreach (var p in projectedStudents)
            {
                Console.WriteLine($"Студент: {p.FullName}, Бад: {p.Score}");
            }

            Console.WriteLine("\n=== 4. АГРЕГУВАННЯ (Average, Max, Min) ===");
            double avgGrade = students.Average(s => s.AverageGrade);
            double maxGrade = students.Max(s => s.AverageGrade);
            double minGrade = students.Min(s => s.AverageGrade);
            Console.WriteLine($"Загальний середній бал по курсу: {avgGrade:F2}");
            Console.WriteLine($"Найвищий бал: {maxGrade}");
            Console.WriteLine($"Найнижчий бал: {minGrade}");

            Console.WriteLine("\n=== 5. ГРУПУВАННЯ (GroupBy з підсумковою статистикою) ===");
            var groupedByGroup = students.GroupBy(s => s.Group);
            foreach (var group in groupedByGroup)
            {
                Console.WriteLine($"Група: {group.Key} (Кількість студентів: {group.Count()}, Середній бал групи: {group.Average(s => s.AverageGrade):F2})");
                foreach (var student in group)
                {
                    Console.WriteLine($"   -> {student.FullName}");
                }
            }

            Console.WriteLine("\n=== ДЕМОНСТРАЦІЯ: Відкладене виконання vs Матеріалізація ===");
            
            var query = students.Where(s => s.AverageGrade > 90);
            Console.WriteLine("Запит створено, але ще не виконано.");

            students.Add(new Student(7, "Новий Студент", "ТЦР-21", 98.0, 1));
            Console.WriteLine("Додали нового студента до оригінального списку ПІСЛЯ створення запиту.");

            Console.WriteLine("Виводимо результати відкладеного запиту (query):");
            foreach (var s in query)
            {
                Console.WriteLine(s);
            }

            var materializedList = students.Where(s => s.AverageGrade > 90).ToList();
            Console.WriteLine($"\nМатеріалізований список (.ToList()) містить елементів: {materializedList.Count}");

            Console.WriteLine("\nРоботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}