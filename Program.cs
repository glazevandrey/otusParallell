using System;
using System.Diagnostics;

namespace otusParallell
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            int count = 100000000;
            int[] ints = new int[count];
            Randomize(ints);

            sw.Start();
            var sum = DefaultIntCalc(ints);
            sw.Stop();
            var intDefault = sw.Elapsed;
            Console.WriteLine("intDefault " + intDefault + " sum-" + sum);

            sw.Restart();
            var sum2 = TreadIntCalc(ints);
            sw.Stop();
            var intTasks = sw.Elapsed;
            Console.WriteLine("intTasks " + intTasks + " sum2-" + sum2);



            sw.Restart();
            var sum3 = ints.AsParallel().Sum();
            sw.Stop();
            var intLinq = sw.Elapsed;
            Console.WriteLine("intLinq " + intLinq + " sum3-" + sum3);

            Console.ReadKey();
        }

        static void Randomize(int[] data)
        {
            Random r = new Random();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = r.Next(1,9999);
            }
        }
        static int DefaultIntCalc(int[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++) { sum += data[i]; }
            return sum;
        }

        static int TreadIntCalc(int[] numbers)
        {
            int numThreads = 15;

            int chunkSize = numbers.Length/numThreads;
            // Список для хранения частичных сумм
            List<int> partialSums = new List<int>();

            // Создание потоков
            Thread[] threads = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                int startIndex = i * chunkSize;
                int endIndex = Math.Min((i + 1) * chunkSize, numbers.Length);

                threads[i] = new Thread(() =>
                {
                    int localSum = 0;
                    for (int j = startIndex; j < endIndex; j++)
                    {
                        localSum += numbers[j];
                    }

                    lock (partialSums)
                    {
                        partialSums.Add(localSum);
                    }
                });
            }

            // Запуск потоков
            foreach (Thread thread in threads)
            {
                thread.Start();
            }

            // Ожидание завершения всех потоков
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Вычисление итоговой суммы
            int totalSum = 0;
            foreach (int partialSum in partialSums)
            {
                totalSum += partialSum;
            }

           return totalSum;
        }

        
    }
}