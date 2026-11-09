using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalWork8_1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(400);

            Console.WriteLine("=== ЗАПУСК ПАКЕТНОЇ ПАРАЛЕЛЬНОЇ ОБРОБКИ ДАНИХ (Task) ===");

            int batchSize = 5;
            List<Task> tasks = new List<Task>();

            for (int i = 1; i <= batchSize; i++)
            {
                int packetId = i;
                tasks.Add(Task.Run(async () =>
                {
                    for (int step = 1; step <= 3; step++)
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        await Task.Delay(100);
                        Console.WriteLine($"[Пакет {packetId}] Виконано крок {step}");
                    }
                    Console.WriteLine($"[Пакет {packetId}] Повністю оброблено.");
                }, cts.Token));
            }

            try
            {
                await Task.WhenAll(tasks);
                Console.WriteLine("\nУсі пакети успішно оброблено.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n[Повідомлення] Пакетну обробку було скасовано за допомогою CancellationToken.");
            }
            catch (AggregateException ex)
            {
                foreach (var inner in ex.InnerExceptions)
                {
                    if (inner is OperationCanceledException)
                    {
                        Console.WriteLine("\n[Повідомлення] Оброблено скасування задач із AggregateException.");
                    }
                }
            }

            Console.WriteLine("\nРоботу програми завершено безпечно. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}
