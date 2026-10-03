using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PracticalWork4
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public string FieldName { get; }

        public ValidationException(string fieldName, string message) : base(message)
        {
            FieldName = fieldName;
        }
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public T? Value { get; }

        private Result(bool success, T? value, string? error)
        {
            IsSuccess = success;
            Value = value;
            Error = error;
        }

        public static Result<T> Ok(T value) => new(true, value, null);
        public static Result<T> Fail(string error) => new(false, default, error);
    }

    public static class Logger
    {
        private const string LogFilePath = "system_log.txt";

        public static void LogError(string errorType, string message, string? stackTrace = null)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(LogFilePath, true);
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [ERROR] [{errorType}] {message}");
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    writer.WriteLine($"StackTrace: {stackTrace}");
                }
                writer.WriteLine(new string('-', 50));
            }
            catch
            {
                Console.WriteLine("[Critical Logger Error] Не вдалося записати лог у файл.");
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                using StreamWriter writer = new StreamWriter(LogFilePath, true);
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [INFO] {message}");
            }
            catch
            {
                Console.WriteLine("[Critical Logger Error] Не вдалося записати лог у файл.");
            }
        }
    }

    public static class SafeExecutor
    {
        public static void Execute(Action action, string operationName)
        {
            try
            {
                action();
                Logger.LogInfo($"Операція '{operationName}' виконана успішно.");
            }
            catch (DomainException ex)
            {
                Logger.LogError("DomainException", ex.Message, ex.StackTrace);
                Console.WriteLine($"[Доменна помилка]: {ex.Message}");
            }
            catch (ValidationException ex)
            {
                Logger.LogError("ValidationException", $"Поле: {ex.FieldName}. {ex.Message}", ex.StackTrace);
                Console.WriteLine($"[Помилка валідації ({ex.FieldName})]: {ex.Message}");
            }
            catch (IOException ex)
            {
                Logger.LogError("IOException", ex.Message, ex.StackTrace);
                Console.WriteLine($"[Помилка файлового введення/виведення]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError("UnhandledException", ex.Message, ex.StackTrace);
                Console.WriteLine($"[Непередбачувана помилка]: {ex.Message}");
            }
        }
    }

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
        private string _fullName = string.Empty;
        private string _speciality = string.Empty;

        public string FullName
        {
            get => _fullName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException(nameof(FullName), "ПІБ студента не може бути порожнім.");
                _fullName = value;
            }
        }

        public string Speciality
        {
            get => _speciality;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException(nameof(Speciality), "Спеціальність не може бути порожньою.");
                _speciality = value;
            }
        }

        public Student(int id, string fullName, string speciality) : base(id)
        {
            FullName = fullName;
            Speciality = speciality;
        }

        public override string ToString() => $"Student [Id: {Id}, Name: {FullName}, Speciality: {Speciality}]";
    }

    public class Course : Entity
    {
        private string _title = string.Empty;
        private int _hours;

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException(nameof(Title), "Назва курсу не може бути порожньою.");
                _title = value;
            }
        }

        public int Hours
        {
            get => _hours;
            set
            {
                if (value <= 0)
                    throw new DomainException($"Кількість годин курсу має бути більшою за нуль. Передано: {value}");
                _hours = value;
            }
        }

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

    public class Repository<T> where T : Entity
    {
        private readonly List<T> _items = new();

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            _items.Add(item);
        }

        public Result<T> GetByIdSafe(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return Result<T>.Fail($"Сутність з ID {id} не знайдено.");
            }
            return Result<T>.Ok(item);
        }

        public IReadOnlyList<T> GetAll() => _items;
    }

    public static class ReportManager
    {
        public static void ExportReport(string filePath, IEnumerable<Student> students, IEnumerable<Course> courses)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Звіт системи");
                writer.WriteLine(DateTime.Now);

                writer.WriteLine("Студенти:");
                foreach (var s in students)
                {
                    writer.WriteLine($"{s.Id};{s.FullName};{s.Speciality}");
                }

                writer.WriteLine("Курси:");
                foreach (var c in courses)
                {
                    writer.WriteLine($"{c.Id};{c.Title};{c.Hours}");
                }
            }
            Console.WriteLine($"Звіт збережено у файл: {filePath}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Repository<Student> studentRepo = new();
            Repository<Course> courseRepo = new();

            SafeExecutor.Execute(() =>
            {
                var invalidCourse = new Course(IdGenerator.NextId(), "Тестування", -10);
                courseRepo.Add(invalidCourse);
            }, "Створення некоректного курсу");

            SafeExecutor.Execute(() =>
            {
                var invalidStudent = new Student(IdGenerator.NextId(), "", "Кібербезпека");
                studentRepo.Add(invalidStudent);
            }, "Створення некоректного студента");

            SafeExecutor.Execute(() =>
            {
                studentRepo.Add(new Student(IdGenerator.NextId(), "Олександр Петренко", "Кібербезпека"));
                courseRepo.Add(new Course(IdGenerator.NextId(), "Об'єктно-орієнтоване програмування", 120));
            }, "Додавання валідних даних");

            SafeExecutor.Execute(() =>
            {
                ReportManager.ExportReport("system_report.txt", studentRepo.GetAll(), courseRepo.GetAll());
            }, "Експорт звіту");

            var resultOk = studentRepo.GetByIdSafe(1);
            if (resultOk.IsSuccess)
            {
                Console.WriteLine($"Знайдено: {resultOk.Value}");
            }

            var resultFail = studentRepo.GetByIdSafe(999);
            if (!resultFail.IsSuccess)
            {
                Console.WriteLine($"Помилка результату: {resultFail.Error}");
            }
        }
    }
}