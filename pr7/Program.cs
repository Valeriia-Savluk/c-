using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalWork7_2
{
    public class OrderSystem
    {
        private int _totalProcessedOrders = 0;
        private readonly object _lockObj = new object();

        public int TotalProcessedOrders => _totalProcessedOrders;

        public void ProcessOrderUnsafe()
        {
            int current = _totalProcessedOrders;
            Thread.Sleep(1);
            _totalProcessedOrders = current + 1;
        }

        public void ProcessOrderSafe()
        {
            lock (_lockObj)
            {
                int current = _totalProcessedOrders;
                Thread.Sleep(1);
                _totalProcessedOrders = current + 1;
            }
        }

        public void Reset()
        {
            _totalProcessedOrders = 0;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            OrderSystem system = new OrderSystem();
            int operationsCount = 50;

            Console.WriteLine("=== 1. КОНКУРЕНТНЕ ВИКОНАННЯ БЕЗ СИНХРОНІЗАЦІЇ (Race Condition) ===");
            Parallel.For(0, operationsCount, i =>
            {
                system.ProcessOrderUnsafe();
            });

            Console.WriteLine($"Очікувана кількість замовлень: {operationsCount}");
            Console.WriteLine($"Реальна кількість (з помилками): {system.TotalProcessedOrders}");

            system.Reset();

            Console.WriteLine("\n=== 2. ПОТОКОБЕЗПЕЧНЕ ВИКОНАННЯ З SYNCHRONIZATION (lock) ===");
            Parallel.For(0, operationsCount, i =>
            {
                system.ProcessOrderSafe();
            });

            Console.WriteLine($"Очікувана кількість замовлень: {operationsCount}");
            Console.WriteLine($"Реальна кількість (коректна): {system.TotalProcessedOrders}");

            Console.WriteLine("\n=== 3. ПАРАЛЕЛЬНЕ ВИКОНАННЯ ЧЕРЕЗ TPL ТА СКАСУВАННЯ (CancellationToken) ===");

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(300);

            try
            {
                await Task.Run(() =>
                {
                    ParallelOptions options = new ParallelOptions
                    {
                        CancellationToken = cts.Token
                    };

                    Parallel.For(0, 1000, options, i =>
                    {
                        options.CancellationToken.ThrowIfCancellationRequested();
                        
                        Thread.Sleep(2);
                        Console.WriteLine($"Оброблено елемент №{i}");
                    });
                }, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n[Успішно] Виконання паралельних задач було скасовано за допомогою CancellationToken.");
            }

            Console.WriteLine("\nРоботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}