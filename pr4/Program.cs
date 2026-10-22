using System;
using System.Collections.Generic;

public class Order
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }

    public event EventHandler<string> StatusChanged;

    public void ChangeStatus(string newStatus)
    {
        Status = newStatus;
        StatusChanged?.Invoke(this, $"Замовлення #{Id} змінило статус на: {Status}");
    }
}

public class OrderManager
{
    private List<Order> _orders = new List<Order>();

    public event EventHandler<Order> OrderCreated;

    public void AddOrder(Order order)
    {
        _orders.Add(order);
        OrderCreated?.Invoke(this, order);
    }

    public List<Order> FilterOrders(Predicate<Order> condition)
    {
        return _orders.FindAll(condition);
    }

    public void ProcessOrders(Action<Order> action)
    {
        foreach (var order in _orders)
        {
            action(order);
        }
    }
}

public class Logger
{
    public void Log(object sender, string message)
    {
        Console.WriteLine($"[LOG]: {message}");
    }

    public void LogCreation(object sender, Order order)
    {
        Console.WriteLine($"[LOG]: Створено замовлення #{order.Id} - {order.Title}");
    }
}

public class Statistics
{
    private int _createdCount = 0;

    public void CountCreation(object sender, Order order)
    {
        _createdCount++;
        Console.WriteLine($"[STAT]: Всього створено замовлень: {_createdCount}");
    }

    public void NotifyStatus(object sender, string message)
    {
        Console.WriteLine($"[STAT]: Оновлено статистику статусів.");
    }
}

class Program
{
    static void Main()
    {
        var manager = new OrderManager();
        var logger = new Logger();
        var stats = new Statistics();

        manager.OrderCreated += logger.LogCreation;
        manager.OrderCreated += stats.CountCreation;

        var order1 = new Order { Id = 1, Title = "Ноутбук", Price = 25000, Status = "Новий" };
        var order2 = new Order { Id = 2, Title = "Мишка", Price = 800, Status = "Новий" };

        manager.AddOrder(order1);
        manager.AddOrder(order2);

        order1.StatusChanged += logger.Log;
        order1.StatusChanged += stats.NotifyStatus;

        order1.ChangeStatus("В обробці");

        List<Order> expensiveOrders = manager.FilterOrders(o => o.Price > 1000);

        manager.ProcessOrders(o => o.Price += 100);
    }
}