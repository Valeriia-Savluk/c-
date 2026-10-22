using System;
using System.Collections.Generic;
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
        public static bool IsNonEmpty(string value) =>
            !string.IsNullOrWhiteSpace(value);

        public static bool IsPositive(int value) =>
            value > 0;
    }

    public class Repository<T> where T : Entity
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            if (item != null)
            {
                _items.Add(item);
            }
        }

        public T? GetById(int id) =>
            _items.FirstOrDefault(x => x.Id == id);

        public IReadOnlyList<T> GetAll() => _items;

        public bool Remove(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                _items.Remove(item);
                return true;
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("ДЕМОНСТРАЦІЯ РОБОТИ УЗАГАЛЬНЕНОГО РЕПОЗИТОРІЮ ТА STATIC МОДУЛІВ\n");

            Repository<Student> studentRepository = new();
            Repository<Course> courseRepository = new();

            string studentName = "Олександр Петренко";
            string studentSpec = "Кібербезпека";

            if (Validation.IsNonEmpty(studentName) && Validation.IsNonEmpty(studentSpec))
            {
                int newStudentId = IdGenerator.NextId();
                Student student = new Student(newStudentId, studentName, studentSpec);
                studentRepository.Add(student);
                Console.WriteLine($"Успішно додано: {student}");
            }

            string courseTitle = "Об'єктно-орієнтоване програмування";
            int courseHours = 120;

            if (Validation.IsNonEmpty(courseTitle) && Validation.IsPositive(courseHours))
            {
                int newCourseId = IdGenerator.NextId();
                Course course = new Course(newCourseId, courseTitle, courseHours);
                courseRepository.Add(course);
                Console.WriteLine($"Успішно додано: {course}");
            }

            int secondStudentId = IdGenerator.NextId();
            Student student2 = new Student(secondStudentId, "Марія Коваль", "Комп'ютерні науки");
            studentRepository.Add(student2);
            Console.WriteLine($"Успішно додано: {student2}");

            Console.WriteLine("\nВивід усіх студентів у репозиторії");
            foreach (var s in studentRepository.GetAll())
            {
                Console.WriteLine(s);
            }

            int searchId = 1;
            Console.WriteLine($"\nПошук студента за ID = {searchId} ");
            var foundStudent = studentRepository.GetById(searchId);
            if (foundStudent != null)
            {
                Console.WriteLine($"Знайдено: {foundStudent}");
            }
            else
            {
                Console.WriteLine("Елемент не знайдено.");
            }

            Console.WriteLine("\nРоботу програми завершено успішно.");
        }
    }
}