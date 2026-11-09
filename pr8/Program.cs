using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PracticalWork8_2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(500);

            Console.WriteLine("=== ЗАПУСК АСИНХРОННИХ ОПЕРАЦІЙ ТА ОБРОБКИ ЗВІТІВ ===");

            try
            {
                Task<string> externalQueryTask = FetchExternalDataAsync(cts.Token);
                Task reportTask = GenerateReportAsync(cts.Token);

                Console.WriteLine("Основний потоковий потік продовжує роботу без блокування...");

                await Task.WhenAll(externalQueryTask, reportTask);

                Console.WriteLine($"\n[Результат зовнішнього запиту]: {externalQueryTask.Result}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("\n[Повідомлення] Виконання асинхронних операцій було успішно скасовано через CancellationToken.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Помилка] Стався виняток: {ex.Message}");
            }

            Console.WriteLine("\nРоботу програми завершено безпечно. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        public static async Task<string> FetchExternalDataAsync(CancellationToken token)
        {
            Console.WriteLine("[Асинхронно] Початок імітації зовнішнього запиту до бази даних...");
            
            await Task.Delay(300, token);
            token.ThrowIfCancellationRequested();

            return "Дані успішно завантажено із зовнішнього сервісу.";
        }

        public static async Task GenerateReportAsync(CancellationToken token)
        {
            Console.WriteLine("[Асинхронно] Початок обробки та генерації аналітичного звіту...");

            for (int i = 1; i <= 3; i++)
            {
                await Task.Delay(150, token);
                token.ThrowIfCancellationRequested();
                Console.WriteLine($"[Звіт] Оброблено етап {i} з 3...");
            }

            Console.WriteLine("[Звіт] Генерацію звіту повністю завершено.");
        }
    }
}