using System;
using System.Collections.Generic;
using System.Linq;

namespace LabWork
{

    public abstract class Entity
    {
        public int Id { get; }
        protected Entity(int id) => Id = id;
    }

    public class Student : Entity
    {
        public string Name { get; set; }
        public Student(int id, string name) : base(id) => Name = name;
    }

    public class Course : Entity
    {
        public string Title { get; set; }
        public Course(int id, string title) : base(id) => Title = title;
    }

    public class Enrollment : Entity
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public Enrollment(int id, int studentId, int courseId) : base(id)
        {
            StudentId = studentId;
            CourseId = courseId;
        }
    }

    public class SchoolService
    {
        private readonly List<Student> _students = new();
        private readonly List<Course> _courses = new();
        private readonly List<Enrollment> _enrollments = new();

        public void AddStudent(Student student)
        {
            if (_students.Any(s => s.Id == student.Id))
                throw new InvalidOperationException("Студент з таким ID вже існує.");
            _students.Add(student);
        }

        public void AddCourse(Course course) => _courses.Add(course);

        public Student FindStudent(int id) => _students.FirstOrDefault(s => s.Id == id);

        public void EnrollStudent(int studentId, int courseId)
        {
            if (FindStudent(studentId) == null)
                throw new ArgumentException("Студента не знайдено.");

            if (_enrollments.Any(e => e.StudentId == studentId && e.CourseId == courseId))
                throw new InvalidOperationException("Студент вже зарахований на цей курс.");

            _enrollments.Add(new Enrollment(_enrollments.Count + 1, studentId, courseId));
        }


        public void RemoveStudent(int id)
        {
            var student = FindStudent(id);
            if (student != null) _students.Remove(student);
        }

        public IReadOnlyList<Student> GetStudents() => _students;
    }

    class Program
    {
        static void Main(string[] args)
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            SchoolService service = new SchoolService();


            service.AddStudent(new Student(1, "Олександр"));
            service.AddCourse(new Course(10, "C# Програмування"));

            service.EnrollStudent(1, 10);

            foreach (var student in service.GetStudents())
            {
                Console.WriteLine($"Студент ID: {student.Id}, Ім'я: {student.Name} успішно зареєстрований.");
            }
        }
    }
}