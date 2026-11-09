using System;
using System.Collections.Generic;

public class GameCharacter
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }

    public event EventHandler<string> CharacterCreated;
    public event EventHandler<string> HealthChanged;
    public event EventHandler<string> RuleViolated;

    public void Create()
    {
        CharacterCreated?.Invoke(this, $"Створено персонажа: {Name} (Рівень: {Level})");
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0)
        {
            Health = 0;
            RuleViolated?.Invoke(this, $"Критична помилка: здоров'я персонажа {Name} впало нижче нуля!");
        }
        HealthChanged?.Invoke(this, $"Персонаж {Name} отримав шкоду. Поточне здоров'я: {Health}");
    }
}

public class MessageNotifier
{
    public void OnEventOccurred(object sender, string message)
    {
        Console.WriteLine($"[ПОВІДОМЛЕННЯ КОРИСТУВАЧУ]: {message}");
    }
}

public class SystemLogger
{
    public void OnEventOccurred(object sender, string message)
    {
        Console.WriteLine($"[ЛОГ СИСТЕМИ]: {DateTime.Now:T} - {message}");
    }
}

class Program
{
    static void Main()
    {
        var character = new GameCharacter { Name = "Вася", Level = 5, Health = 100 };

        var notifier = new MessageNotifier();
        var logger = new SystemLogger();

        character.CharacterCreated += notifier.OnEventOccurred;
        character.CharacterCreated += logger.OnEventOccurred;

        character.HealthChanged += notifier.OnEventOccurred;
        character.HealthChanged += logger.OnEventOccurred;

        character.RuleViolated += notifier.OnEventOccurred;
        character.RuleViolated += logger.OnEventOccurred;

        character.Create();

        character.TakeDamage(40);

        character.TakeDamage(80);
    }
}