using System;
using System.Collections.Generic;

namespace PracticalWork
{

    public abstract class Entity
    {
        public int Id { get; }
        protected Entity(int id) => Id = id;
    }

    public class Person : Entity
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => _name = string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
        }

        public Person(int id, string name) : base(id) => Name = name;

        public virtual string GetInfo() => $"ID: {Id}, Ім'я: {Name}";
    }

    public class Student : Person
    {
        public string Group { get; set; }

        public Student(int id, string name, string group) : base(id, name)
        {
            Group = group;
        }

        public override string GetInfo() => base.GetInfo() + $", Група: {Group}";
    }

    public class Teacher : Person
    {
        public string Subject { get; set; }

        public Teacher(int id, string name, string subject) : base(id, name)
        {
            Subject = subject;
        }

        public override string GetInfo() => base.GetInfo() + $", Предмет: {Subject}";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Person> people = new List<Person>
            {
                new Student(1, "Валерія", "ТЦР-21"),
                new Teacher(2, "Артур Станіславович", "Об'єктно-орієнтоване програмування С#")
            };

            foreach (var p in people)
            {
                Console.WriteLine(p.GetInfo());
            }
        }
    }
}