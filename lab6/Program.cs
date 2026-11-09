using System;
using System.Collections.Generic;
using System.Linq;

namespace PracticalWork6_1
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public double AverageGrade { get; set; }
        public int CompletedTasks { get; set; }

        public Student(int id, string fullName, string group, double averageGrade, int completedTasks)
        {
            Id = id;
            FullName = fullName;
            Group = group;
            AverageGrade = averageGrade;
            CompletedTasks = completedTasks;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Student> students = new List<Student>
            {
                new Student(1, "Валерія Савлук", "ТЦР-21", 92.5, 6),
                new Student(2, "Олександр Коваль", "ТЦР-21", 74.0, 4),
                new Student(3, "Марія Петренко", "ТЦР-22", 88.0, 5),
                new Student(4, "Андрій Шевченко", "ТЦР-21", 61.5, 3),
                new Student(5, "Ірина Мельник", "ТЦР-22", 95.0, 6),
                new Student(6, "Дмитро Бойко", "ТЦР-22", 82.0, 5)
            };

            int totalStudents = students.Count();
            double overallAverageGrade = students.Average(s => s.AverageGrade);
            int totalCompletedTasks = students.Sum(s => s.CompletedTasks);

            Console.WriteLine("==================================================");
            Console.WriteLine("          КОМПЛЕКСНИЙ АНАЛІТИЧНИЙ ЗВІТ           ");
            Console.WriteLine("==================================================");
            Console.WriteLine($"[Показник 1] Загальна кількість студентів: {totalStudents}");
            Console.WriteLine($"[Показник 2] Середній бал по всіх студентах: {overallAverageGrade:F2}");
            Console.WriteLine($"[Показник 3] Загальна кількість виконаних завдань: {totalCompletedTasks}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("ДЕТАЛІЗАЦІЯ ЗА ГРУПАМИ (Групування та агрегація):");
            Console.WriteLine("--------------------------------------------------");

            var reportByGroup = students.GroupBy(s => s.Group);

            foreach (var group in reportByGroup)
            {
                Console.WriteLine($"Група: {group.Key}");
                Console.WriteLine($"  -> Кількість студентів: {group.Count()}");
                Console.WriteLine($"  -> Середній бал групи: {group.Average(s => s.AverageGrade):F2}");
                Console.WriteLine($"  -> Максимальний бал у групі: {group.Max(s => s.AverageGrade)}");
                Console.WriteLine($"  -> Всього виконано завдань: {group.Sum(s => s.CompletedTasks)}");
                Console.WriteLine("  ----------------------------------------------");
            }

            Console.WriteLine("\nАналітичний звіт успішно сформовано та виведено у консоль.");
            Console.WriteLine("Роботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}