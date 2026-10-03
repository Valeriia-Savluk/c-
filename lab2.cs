using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PracticalWork2
{

    public abstract class Entity
    {
        public int Id { get; }

        protected Entity(int id)
        {
            Id = id;
        }
    }

    public class Student : Entity
    {
        public string FullName { get; }
        public string Speciality { get; }

        public Student(int id, string fullName, string speciality) : base(id)
        {
            FullName = fullName;
            Speciality = speciality;
        }

        public override string ToString() => $"Student [Id: {Id}, Name: {FullName}, Speciality: {Speciality}]";
    }

    public class Course : Entity
    {
        public string Title { get; }
        public int Hours { get; }

        public Course(int id, string title, int hours) : base(id)
        {
            Title = title;
            Hours = hours;
        }

        public override string ToString() => $"Course [Id: {Id}, Title: {Title}, Hours: {Hours}]";
    }

    public static class IdGenerator
    {
        private static int _current = 0;
        public static int NextId() => ++_current;
    }

    public static class Validation
    {
        public static bool IsNonEmpty(string value) => !string.IsNullOrWhiteSpace(value);
        public static bool IsPositive(int value) => value > 0;
    }

    public class Repository<T> where T : Entity
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            if (item != null) _items.Add(item);
        }

        public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

        public IReadOnlyList<T> GetAll() => _items;
    }

    public static class ReportManager
    {
        public static void ExportReport(string filePath, IEnumerable<Student> students, IEnumerable<Course> courses)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("ЗВІТ ПРО СТАН СИСТЕМИ ІНДИВІДУАЛЬНОГО ПРОЄКТУ");
                    writer.WriteLine($"Дата формування: {DateTime.Now}");
                    writer.WriteLine(new string('-', 50));

                    writer.WriteLine("\n[СТУДЕНТИ]");
                    foreach (var student in students)
                    {
                        writer.WriteLine($"{student.Id};{student.FullName};{student.Speciality}");
                    }

                    writer.WriteLine("\n[КУРСИ]");
                    foreach (var course in courses)
                    {
                        writer.WriteLine($"{course.Id};{course.Title};{course.Hours} годин");
                    }

                    writer.WriteLine(new string('-', 50));
                    writer.WriteLine($"Загальна кількість студентів: {students.Count()}");
                    writer.WriteLine($"Загальна кількість курсів: {courses.Count()}");
                }

                Console.WriteLine($"[Успіх] Звіт успішно збережено у файл: {filePath}");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("[Помилка] Відсутні права доступу до запису у вказану директорію.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("[Помилка] Вказаний шлях або директорію не знайдено.");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[Помилка введення/виведення (I/O)]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Непередбачувана помилка]: {ex.Message}");
            }
        }

        public static void ReadAndVerifyReport(string filePath)
        {
            Console.WriteLine($"\nПеревірка зчитування файлу звіту: {filePath} ");
            try
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    Console.WriteLine("Вміст файлу звіту:\n");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("[Помилка] Файл звіту не знайдено для зчитування.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка читання файлу]: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("ЛАБОРАТОРНА РОБОТА №2: ЗВІТНІСТЬ ТА ЕКСПОРТ ДАНИХ\n");

            Repository<Student> studentRepo = new();
            Repository<Course> courseRepo = new();

            studentRepo.Add(new Student(IdGenerator.NextId(), "Олександр Петренко", "Кібербезпека"));
            studentRepo.Add(new Student(IdGenerator.NextId(), "Марія Коваль", "Комп'ютерні науки"));

            courseRepo.Add(new Course(IdGenerator.NextId(), "Об'єктно-орієнтоване програмування", 120));
            courseRepo.Add(new Course(IdGenerator.NextId(), "Бази даних", 90));

            string reportFileName = "system_report.txt";

            ReportManager.ExportReport(reportFileName, studentRepo.GetAll(), courseRepo.GetAll());

            ReportManager.ReadAndVerifyReport(reportFileName);

            Console.WriteLine("\nДемонстрація обробки помилки файлового I/O ");
            ReportManager.ReadAndVerifyReport("Z:\\invalid_folder\\non_existent_report.txt");

            Console.WriteLine("\nРоботу програми завершено успішно.");
        }
    }
}