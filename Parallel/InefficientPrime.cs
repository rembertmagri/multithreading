namespace Parallel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    class InefficientPrimeCustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
        public double FirstNumberOfRange;
        public double LastNumberOfRange;
        public double[] PrimeNumbersFound;
    }

    public class InefficientPrime
    {
        public int calculateNumberOfPrimes(int cores, int limit)
        {
            var step = limit / cores;
            Task[] taskArray = new Task[cores];
            for (int i = 0; i < taskArray.Length; i++)
            {
                var firstNumberOfRange = i * step;
                var lastNumberOfRange = (i + 1) * step - 1;
                taskArray[i] = Task.Factory.StartNew(
                    (obj) => FindAllPrimeNumbers(obj),
                    new InefficientPrimeCustomData()
                    {
                        Name = i,
                        CreationTime = DateTime.Now.Ticks,
                        FirstNumberOfRange = firstNumberOfRange,
                        LastNumberOfRange = lastNumberOfRange
                    });
            }
            var totalNumberOfPrimeNumberFound = 0;

            Task.WaitAll(taskArray);
            
            foreach (var task in taskArray)
            {
                var data = task.AsyncState as InefficientPrimeCustomData;
                if (data != null)
                {
                    Console.Write($"Task #{data.Name} created at {new DateTime(data.CreationTime)}, ");
                    Console.Write($"ran on thread #{data.ThreadNum}, and found {data.PrimeNumbersFound.Length} prime numbers ");
                    Console.WriteLine($"within {data.FirstNumberOfRange} and {data.LastNumberOfRange}.");
                    //data.PrimeNumbersFound.ToList().ForEach(i => Console.Write($"{i} "));
                    totalNumberOfPrimeNumberFound += data.PrimeNumbersFound.Length;
                }
            }
            return totalNumberOfPrimeNumberFound;
        }

        private void FindAllPrimeNumbers(object obj)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var data = obj as InefficientPrimeCustomData;
            if (data == null)
                return;
            data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine($"Task {data.Name} started");
            var primeNumbers = new List<double>();

            for (var candidate = data.FirstNumberOfRange; candidate <= data.LastNumberOfRange; candidate++)
            {
                if (IsPrime(candidate))
                {
                    primeNumbers.Add(candidate);
                }
            }

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            data.PrimeNumbersFound = primeNumbers.ToArray();
            Console.WriteLine($"Task {data.Name} finished in {elapsedTime}");
        }

        public bool IsPrime(double number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }
    }
}
