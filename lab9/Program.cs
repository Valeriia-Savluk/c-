using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalWork9_1
{
    public interface IRepository<T>
    {
        void Add(T entity);
        IEnumerable<T> GetAll();
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public User(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Ім'я: {Name}, Email: {Email}";
        }
    }

    public class UserRepository : IRepository<User>
    {
        private readonly List<User> _users = new List<User>();

        public void Add(User entity)
        {
            if (entity != null && !string.IsNullOrWhiteSpace(entity.Email))
            {
                _users.Add(entity);
                Console.WriteLine($"[Репозиторій] Успішно додано користувача: {entity.Name}");
            }
        }

        public IEnumerable<User> GetAll()
        {
            return _users;
        }
    }

    public class Logger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[Лог]: {message} [{DateTime.Now:HH:mm:ss}]");
        }
    }

    public class ReportService
    {
        public async Task SaveReportAsync(string reportContent, CancellationToken token)
        {
            Console.WriteLine("[Звітність] Початок асинхронного збереження звіту у файл...");

            for (int i = 1; i <= 3; i++)
            {
                await Task.Delay(100, token);
                token.ThrowIfCancellationRequested();
                Console.WriteLine($"[Звітність] Запис даних... Етап {i}/3");
            }

            Console.WriteLine("[Звітність] Звіт успішно збережено асинхронно.");
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Logger logger = new Logger();
            UserRepository repository = new UserRepository();
            ReportService reportService = new ReportService();

            logger.Log("Ініціалізація системи...");

            repository.Add(new User(1, "Валерія Савлук", "st8430982@stud.duikt.edu.ua"));
            repository.Add(new User(2, "Олександр Коваль", "oleksandr@example.com"));

            Console.WriteLine("\n--- Перевірка наявних даних у репозиторії ---");
            foreach (var user in repository.GetAll())
            {
                Console.WriteLine(user);
            }

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(300);

            Console.WriteLine("\n--- Запуск асинхронної операції вводу-виводу (Звітність) ---");

            try
            {
                await reportService.SaveReportAsync("Аналітичний звіт індивідуального проєкту", cts.Token);
                logger.Log("Операцію збереження звіту завершено успішно.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n[Повідомлення] Асинхронну операцію збереження звіту було скасовано через CancellationToken.");
                logger.Log("Збереження звіту скасовано користувачем/системою.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Помилка] Стався виняток: {ex.Message}");
                logger.Log($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nУсі попередні модулі працюють стабільно.");
            Console.WriteLine("Роботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}