using System;
using System.Collections;
using System.Collections.Generic;

namespace PracticalWork5_1
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public User(int id, string name, string role)
        {
            Id = id;
            Name = name;
            Role = role;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Ім'я: {Name}, Роль: {Role}";
        }
    }

    public class UserCollection : IEnumerable<User>
    {
        private List<User> _users = new List<User>();

        public void Add(User user)
        {
            if (user != null)
            {
                _users.Add(user);
            }
        }

        public IEnumerable<User> FilterByRole(string role)
        {
            foreach (var user in _users)
            {
                if (user.Role.Equals(role, StringComparison.OrdinalIgnoreCase))
                {
                    yield return user;
                }
            }
        }

        public IEnumerator<User> GetEnumerator()
        {
            foreach (var user in _users)
            {
                yield return user;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            UserCollection collection = new UserCollection();

            collection.Add(new User(1, "Валерія", "Student"));
            collection.Add(new User(2, "Артур Станіславович", "Teacher"));
            collection.Add(new User(3, "Марія", "Student"));

            Console.WriteLine("=== Усі користувачі у власній колекції ===");
            foreach (var user in collection)
            {
                Console.WriteLine(user);
            }

            Console.WriteLine("\n=== Результат фільтрації (тільки Student) ===");
            foreach (var user in collection.FilterByRole("Student"))
            {
                Console.WriteLine(user);
            }

            Console.WriteLine("\nРоботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}