using System;
using System.Collections;
using System.Collections.Generic;

namespace PracticalWork5
{
    public interface INotifiable
    {
        void SendNotification(string message);
    }

    public interface IRepository<T>
    {
        void Add(T entity);
        void Remove(T entity);
        int Count { get; }
    }

    public class User : INotifiable
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

        public void SendNotification(string message)
        {
            Console.WriteLine($"[Сповіщення для користувача {Name} ({Email})]: {message}");
        }

        public override string ToString()
        {
            return $"ID: {Id}, Ім'я: {Name}, Email: {Email}";
        }
    }

    public class EntityCollection<T> : IRepository<T>, IEnumerable<T>
    {
        private List<T> _items = new List<T>();

        public int Count => _items.Count;

        public void Add(T entity)
        {
            if (entity != null)
            {
                _items.Add(entity);
            }
        }

        public void Remove(T entity)
        {
            _items.Remove(entity);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class NotificationService
    {
        public void NotifyAll(IEnumerable<INotifiable> notifiables, string text)
        {
            Console.WriteLine("\n--- Запуск розсилки сповіщень через сервіс ---");
            foreach (var item in notifiables)
            {
                item.SendNotification(text);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ ІНТЕРФЕЙСІВ ТА ВЛАСНОЇ КОЛЕКЦІЇ ===");

            EntityCollection<User> usersCollection = new EntityCollection<User>();

            usersCollection.Add(new User(1, "Валерія", "valeriia@example.com"));
            usersCollection.Add(new User(2, "Олександр", "oleksandr@example.com"));
            usersCollection.Add(new User(3, "Марія", "maria@example.com"));

            Console.WriteLine($"\nУ нашій колекції зберігається елементів: {usersCollection.Count}");
            Console.WriteLine("Перелік користувачів через власний ітератор (foreach):");
            
            foreach (var user in usersCollection)
            {
                Console.WriteLine($" -> {user}");
            }

            NotificationService notificationService = new NotificationService();

            List<INotifiable> notifiableList = new List<INotifiable>();
            foreach (var user in usersCollection)
            {
                notifiableList.Add(user);
            }

            notificationService.NotifyAll(notifiableList, "Ваша практична робота успішно перевірена!");

            Console.WriteLine("\nРоботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}