using System;
using System.Threading;

namespace PracticalWork7_1
{
    public class UnsafeAccount
    {
        private int _balance = 0;

        public int Balance => _balance;

        public void Deposit(int amount)
        {
            int current = _balance;
            Thread.Sleep(1); 
            _balance = current + amount;
        }
    }

    public class SafeAccount
    {
        private int _balance = 0;
        private readonly object _lockObj = new object();

        public int Balance => _balance;

        public void Deposit(int amount)
        {
            lock (_lockObj)
            {
                int current = _balance;
                Thread.Sleep(1);
                _balance = current + amount;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            int threadCount = 50;
            int depositPerThread = 100;
            int expectedTotal = threadCount * depositPerThread;

            Console.WriteLine("=== 1. ВАРІАНТ БЕЗ СИНХРОНІЗАЦІЇ (Небезпечний) ===");
            UnsafeAccount unsafeAccount = new UnsafeAccount();
            Thread[] unsafeThreads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                unsafeThreads[i] = new Thread(() => unsafeAccount.Deposit(depositPerThread));
                unsafeThreads[i].Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                unsafeThreads[i].Join();
            }

            Console.WriteLine($"Очікуваний баланс: {expectedTotal}");
            Console.WriteLine($"Реальний баланс (з втратою оновлень): {unsafeAccount.Balance}");


            Console.WriteLine("\n=== 2. ПОТОКОБЕЗПЕЧНИЙ ВАРІАНТ (З синхронізацією lock) ===");
            SafeAccount safeAccount = new SafeAccount();
            Thread[] safeThreads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                safeThreads[i] = new Thread(() => safeAccount.Deposit(depositPerThread));
                safeThreads[i].Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                safeThreads[i].Join();
            }

            Console.WriteLine($"Очікуваний баланс: {expectedTotal}");
            Console.WriteLine($"Реальний баланс (коректний): {safeAccount.Balance}");

            Console.WriteLine("\nРоботу програми завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}